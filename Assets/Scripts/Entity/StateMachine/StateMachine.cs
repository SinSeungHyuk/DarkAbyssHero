using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StateMachine<TOwnerType> 
{
    // ������Ʈ ����ɶ� ȣ��� �̺�Ʈ
    // <������Ʈ�ӽ�, �� ������Ʈ, ���� ������Ʈ, ���̾�>
    public event Action
        <StateMachine<TOwnerType>, State<TOwnerType>, State<TOwnerType>, int> OnStateChanged;

    // State�� ������ ��Ƽ� ������ innerŬ����
    private class StateData
    {
        // ������Ʈ�� ���� ���̾� 
        public int Layer {  get; private set; }
        // ������Ʈ�� ��� ���� (����� ������� �켱���� �ο�)
        public int Priority { get; private set; }
        // �� data�� ���� ������Ʈ
        public State<TOwnerType> State { get; private set; }
        // �� ������Ʈ�� ����� Ʈ�����ǵ�
        public List<StateTransition<TOwnerType>> Transitions { get; private set; } = new(); 

        // StateData ������
        public StateData(int layer, int priority, State<TOwnerType> state)
        {
            Layer = layer;
            Priority = priority;
            State = state;
        }
    }

    // Layer�� ������ �ִ� StateDatas(=Layer Dictionary), Dictionary�� Key�� Value�� StateData�� ���� State�� Type
    // ��, State�� Type�� ���� �ش� State�� ���� StateData�� ã�ƿ� �� ����
    private readonly Dictionary<int, Dictionary<Type, StateData>> stateDatasByLayer = new();
    // Layer�� Any Tansitions(���Ǹ� �����ϸ� �������� ToState�� ���̵Ǵ� Transition)
    private readonly Dictionary<int, List<StateTransition<TOwnerType>>> anyTransitionsByLayer = new();

    // Layer�� ���� �������� StateData(=���� �������� State)
    private readonly Dictionary<int, StateData> currentStateDatasByLayer = new();

    // StatMachine�� �����ϴ� Layer��, Layer�� �ߺ����� �ʾƾ��ϰ�, �ڵ� ������ ���ؼ� SortedSet�� �����
    private readonly SortedSet<int> layers = new();

    // StateMachine�� ������ (Player, Skill)
    public TOwnerType TOwner { get; private set; }


    // MonoBehaviour�� ���� ������ ���� Update���� ����
    // �̰� ��ӹ޴� �ڽ� ������Ʈ�ӽ��� Update�� ���� ȣ��
    public void Update()
    {
        // �� ���̾�� ���� �������� ������Ʈ�� ��ȸ�ϸ鼭 ����,������Ʈ ������Ʈ

        foreach (var layer in layers)
        {
            // Layer���� �������� ���� StateData�� ������
            var currentStateData = currentStateDatasByLayer[layer];

            // Layer�� ���� AnyTransitions�� ã�ƿ�
            bool hasAnyTransitions = anyTransitionsByLayer.TryGetValue(layer, out var anyTransitions);

            // AnyTansition�� �����ϸ�ٸ� AnyTransition���� ToState ���̸� �õ��ϰ�,
            // ������ ���� �ʾ� �������� �ʾҴٸ�, ���� StateData�� Transition�� �̿��� ���̸� �õ���
            if ((hasAnyTransitions && TryTransition(anyTransitions, layer)) ||
                TryTransition(currentStateData.Transitions, layer))
                continue;

            // �������� ���ߴٸ� ���� State�� Update�� ������
            currentStateData.State.Update();
        }
    }

    public void SetUp(TOwnerType owner)
    {
        Debug.Assert(owner != null, $"Setup - owner�� null�� �� �� �����ϴ�.");

        TOwner = owner;

        AddStates();
        MakeTransitions();
        SetUpLayers();
    }

    // Layer���� Current State�� �������ִ� ���ִ� �Լ�
    public void SetUpLayers()
    {
        foreach ((int layer, var statDatasByType) in stateDatasByLayer)
        {
            // State�� �����ų Layer�� �������
            currentStateDatasByLayer[layer] = null;

            // �켱 ������ ���� ���� StateData�� ã�ƿ�
            StateData firstStateData = statDatasByType.Values.First(x => x.Priority == 0);
            // ã�ƿ� StateData�� State�� ���� Layer�� Current State�� ��������
            ChangeState(firstStateData);
        }
    }

    // ���� �������� CurrentStateData�� �����ϴ� �Լ�
    private void ChangeState(StateData newStateData)
    {
        // Layer�� �´� ���� �������� CurrentStateData�� ������
        StateData prevState = currentStateDatasByLayer[newStateData.Layer];

        prevState?.State.Exit();
        // ���� �������� CurrentStateData�� ���ڷ� ���� newStateData�� ��ü����
        currentStateDatasByLayer[newStateData.Layer] = newStateData;
        newStateData.State.Enter();

        // State�� ���̵Ǿ����� �˸�
        OnStateChanged?.Invoke(this, newStateData.State, prevState.State, newStateData.Layer);
    }

    // newState�� Type�� �̿��� StateData�� ã�ƿͼ� ���� �������� CurrentStateData�� �����ϴ� �Լ�
    private void ChangeState(State<TOwnerType> newState, int layer)
    {
        // Layer�� ����� StateDatas�� newState�� ���� StateData�� ã�ƿ�
        StateData newStateData = stateDatasByLayer[layer][newState.GetType()];
        ChangeState(newStateData);
    }

    // �� ������Ʈ�� ���� Ʈ�����ǵ��� Ȯ���Ͽ� ���̸� �õ�
    private bool TryTransition(IReadOnlyList<StateTransition<TOwnerType>> transtions, int layer)
    {
        foreach (var transition in transtions)
        {
            // Command�� �����Ѵٸ�, Command�� �޾��� ���� ���� �õ��� �ؾ������� �Ѿ
            // Command�� �������� �ʾƵ�, ���� ������ �������� ���ϸ� �Ѿ
            if (transition.TransitionCommand != StateTransition<TOwnerType>.NullCommand || !transition.IsTransferable)
                continue;

            // CanTrainsitionToSelf(�ڱ� �ڽ����� ���� ���� �ɼ�)�� false�� �����ؾ��� ToState�� CurrentState�� ���ٸ� �Ѿ
            if (!transition.CanTransitionToSelf && currentStateDatasByLayer[layer].State == transition.ToState)
                continue;

            // ��� ������ �����Ѵٸ� ToState�� ����
            ChangeState(transition.ToState, layer);
            return true;
        }
        return false;
    }


    // Generic�� ���� StateMachine�� State�� �߰��ϴ� �Լ�
    // T�� State<TOwnerType> class�� ��ӹ��� Type�̿�����
    public void AddState<T>(int layer = 0) where T : State<TOwnerType>
    {
        // Layer �߰�, Set�̹Ƿ� �̹� Layer�� �����Ѵٸ� �߰����� ����
        layers.Add(layer);

        // Type�� ���� State�� ����
        var newState = Activator.CreateInstance<T>();
        newState.SetUp(this, TOwner, layer);

        // ���� stateDatasByLayer�� �߰����� ���� Layer��� Layer�� ��������
        if (!stateDatasByLayer.ContainsKey(layer))
        {
            // Layer�� StateData ����� Dictionary<Type, StateData> ����
            stateDatasByLayer[layer] = new();
            // Layer�� AnyTransitions ����� List<StateTransition<TOwnerType>> ����
            anyTransitionsByLayer[layer] = new();
        }

        Debug.Assert(!stateDatasByLayer[layer].ContainsKey(typeof(T)),
            $"StateMachine::AddState<{typeof(T).Name}> - �̹� ���°� �����մϴ�.");

        var stateDatasByType = stateDatasByLayer[layer];
        // StateData�� ���� Layer�� �߰�
        stateDatasByType[typeof(T)] = new StateData(layer, stateDatasByType.Count, newState);
    }

    // Transition�� �����ϴ� �Լ�
    // FromStateType�� ���� State�� Type
    // ToStateType�� ������ State�� Type
    // �� Tpye ��� State<TOwnerType> class�� �ڽ��̿�����
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<TOwnerType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<TOwnerType>
        where ToStateType : State<TOwnerType>
    {
        var stateDatas = stateDatasByLayer[layer];
        // StateDatas���� FromStateType�� State�� ���� StateData�� ã�ƿ�
        StateData fromStateData = stateDatas[typeof(FromStateType)];
        // StateDatas���� ToStateType�� State�� ���� StateData�� ã�ƿ�
        StateData toStateData = stateDatas[typeof(ToStateType)];

        // ���ڿ� ã�ƿ� Data�� ������ Transition�� ����
        // AnyTransition�� �ƴ� �Ϲ� Transition�� canTransitionToSelf ���ڰ� ������ true
        var newTransition = new StateTransition<TOwnerType>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition, true);
        // ������ Transition�� FromStateData�� Transition���� �߰�
        fromStateData.Transitions.Add(newTransition);
    }
    #region Make Transition Wrapping
    // MakeTransition �Լ��� Enum Command ����
    // Enum������ ���� Command�� Int�� ��ȯ�Ͽ� ���� �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<TOwnerType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<TOwnerType>
        where ToStateType : State<TOwnerType>
        => MakeTransition<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer);

    // Enum Ŀ�ǵ�� Null, ��� Func�� ���� ���������� �˻��ϴ� ����
    public void MakeTransition<FromStateType, ToStateType>(Func<State<TOwnerType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<TOwnerType>
        where ToStateType : State<TOwnerType>
        => MakeTransition<FromStateType, ToStateType>(StateTransition<TOwnerType>.NullCommand, transitionCondition, layer);

    // MakeTransition �Լ��� Condition ���ڰ� ���� ����
    // Condition���� null�� �־ �ֻ���� MakeTransition �Լ��� ȣ���� 
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<TOwnerType>
        where ToStateType : State<TOwnerType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    // �� �Լ��� Enum ����(Command ���ڰ� Enum���̰� Condition ���ڰ� ����)
    // ���� ���ǵ� Enum���� MakeTransition �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<TOwnerType>
        where ToStateType : State<TOwnerType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);
    #endregion

    // AnyTransition�� ����� �Լ�
    // ������ �����ϸ� ToStateType ������Ʈ�� ������ �ٷ� ���̵Ǿ����
    public void MakeAnyTransition<ToStateType>(int transitionCommand,
        Func<State<TOwnerType>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<TOwnerType>
    {
        var stateDatasByType = stateDatasByLayer[layer];
        // StateDatas���� ToStateType�� State�� ���� StateData�� ã�ƿ�
        var state = stateDatasByType[typeof(ToStateType)].State;
        // Transition ����, �������� ���Ǹ� ������ ������ ���̹Ƿ� FromState�� �������� ����
        var newTransition = new StateTransition<TOwnerType>(null, state, transitionCommand, transitionCondition, canTransitonToSelf);
        // Layer�� AnyTransition���� �߰�
        anyTransitionsByLayer[layer].Add(newTransition);
    }
    #region Make AnyTransition Wrapping
    // MakeAnyTransition �Լ��� Enum Command ����
    // Enum������ ���� Command�� Int�� ��ȯ�Ͽ� ���� �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
        Func<State<TOwnerType>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<TOwnerType>
        => MakeAnyTransition<ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer, canTransitonToSelf);

    // MakeAnyTransition �Լ��� Command ���ڰ� ���� ����
    // NullCommand�� �־ �ֻ���� MakeTransition �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Func<State<TOwnerType>, bool> transitionCondition,
        int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<TOwnerType>
        => MakeAnyTransition<ToStateType>(StateTransition<TOwnerType>.NullCommand, transitionCondition, layer, canTransitonToSelf);

    // MakeAnyTransiiton�� Condition ���ڰ� ���� ����
    // Condition���� null�� �־ �ֻ���� MakeTransition �Լ��� ȣ���� 
    public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitonToSelf = false)
    where ToStateType : State<TOwnerType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);

    // �� �Լ��� Enum ����(Command ���ڰ� Enum���̰� Condition ���ڰ� ����)
    // ���� ���ǵ� Enum���� MakeAnyTransition �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<TOwnerType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);
    #endregion

    // Command�� �޾Ƽ� Transition�� �����ϴ� �Լ�
    public bool ExecuteCommand(int transitionCommand, int layer)
    {
        // AnyTransition���� Command�� ��ġ�ϰ�, ���� ������ �����ϴ� Transiton�� ã�ƿ�
        var transition = anyTransitionsByLayer[layer].Find(x =>
        x.TransitionCommand == transitionCommand && x.IsTransferable);

        // AnyTransition���� Transtion�� �� ã�ƿԴٸ� ���� �������� CurrentStateData�� Transitions����
        // Command�� ��ġ�ϰ�, ���� ������ �����ϴ� Transition�� ã�ƿ�
        transition ??= currentStateDatasByLayer[layer].Transitions.Find(x =>
        x.TransitionCommand == transitionCommand && x.IsTransferable);

        // ������ Transtion�� ã�ƿ��� ���ߴٸ� ��� ������ ����
        if (transition == null)
            return false;

        // ������ Transiton�� ã�ƿԴٸ� �ش� Transition�� ToState�� ����
        ChangeState(transition.ToState, layer);
        return true;
    }
    #region ExecuteCommand Wrapping
    // ExecuteCommand�� Enum Command ����
    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => ExecuteCommand(Convert.ToInt32(transitionCommand), layer);

    // ��� Layer�� ������� ExecuteCommand �Լ��� �����ϴ� �Լ�
    // �ϳ��� Layer�� ���̿� �����ϸ� true�� ��ȯ 
    public bool ExecuteCommand(int transitionCommand)
    {
        bool isSuccess = false;

        foreach (int layer in layers)
        {
            if (ExecuteCommand(transitionCommand, layer))
                isSuccess = true;
        }

        return isSuccess;
    }
    // �� ExecuteCommand �Լ��� Enum Command ����
    public bool ExecuteCommand(Enum transitionCommand)
        => ExecuteCommand(Convert.ToInt32(transitionCommand));
    #endregion

    // ���� �������� CurrentStateData�� Message�� ������ �Լ�
    // ������Ʈ�ӽſ��� �޼����� ������ �� ������Ʈ�� OnReceiveMessage �Լ� ȣ��
    public bool SendMessage(int message, int layer, object extraData = null)
        => currentStateDatasByLayer[layer].State.OnReceiveMessage(message, extraData);
    #region SendMessage Wrapping
    // SendMessage �Լ��� Enum Message ����
    public bool SendMessage(Enum message, int layer, object extraData = null)
        => SendMessage(Convert.ToInt32(message), layer, extraData);

    // ��� Layer�� ���� �������� CurrentStateData�� ������� SendMessage �Լ��� �����ϴ� �Լ�
    // �ϳ��� CurrentStateData�� ������ Message�� �����ߴٸ� true�� ��ȯ
    public bool SendMessage(int message, object extraData = null)
    {
        bool isSuccess = false;
        foreach (int layer in layers)
        {
            if (SendMessage(message, layer, extraData))
                isSuccess = true;
        }
        return isSuccess;
    }
    // �� SendMessage �Լ��� Enum Message ����
    public bool SendMessage(Enum message, object extraData = null)
        => SendMessage(Convert.ToInt32(message), extraData);
    #endregion

    // ��� Layer�� ���� �������� CurrentState�� Ȯ���Ͽ�, ���� State�� T Type�� State���� Ȯ���ϴ� �Լ�
    // CurrentState�� T Type�ΰ� Ȯ�εǸ� ��� true�� ��ȯ��
    public bool IsInState<T>() where T : State<TOwnerType>
    {
        foreach ((_, StateData data) in currentStateDatasByLayer)
        {
            // ex) if (IsInState<State<InActionState>>) 
            // => ���� ���� �������� ������Ʈ�� InActionState ������Ʈ��� true
            if (data.State.GetType() == typeof(T))
                return true;
        }
        return false;
    }

    // layer���� ���� �������� ������Ʈ�� ��������
    public State<TOwnerType> GetCurrentState(int layer = 0) => currentStateDatasByLayer[layer].State;

    // layer���� ���� �������� State�� Type�� ������
    public Type GetCurrentStateType(int layer = 0) => GetCurrentState(layer).GetType();


    // ������Ʈ, Ʈ������ ������ �ڽ� ������Ʈ�ӽſ��� ����
    protected virtual void AddStates() { }
    protected virtual void MakeTransitions() { }
}
