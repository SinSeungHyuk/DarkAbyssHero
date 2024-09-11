using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(IdentifiedObject), true)]
public class IdentifiedObjectEditor : Editor  // 커스텀 에디터를 위한 클래스
{
    // SerializedProperty : 직렬화 된 변수들을 참조하여 값을 조작할 수 있는 타입
    private SerializedProperty categoriesProperty;
    private SerializedProperty iconProperty;
    private SerializedProperty idProperty;
    private SerializedProperty codeNameProperty;
    private SerializedProperty displayNameProperty;
    private SerializedProperty descriptionProperty;

    // Inspector 상에서 순서를 편집할 수 있는 List
    private ReorderableList categories;

    // text를 넓게 보여주는 Style(=Skin) 지정을 위한 변수
    private GUIStyle textAreaStyle;

    // Title의 Foldout Expand 상태를 저장하는 변수
    private readonly Dictionary<string, bool> isFoldoutExpandedesByTitle = new();


    protected virtual void OnEnable()
    {
        // 인스펙터에서 다른 인스펙터 뷰로 넘어가도 포커스가 안넘어가는 문제 수정
        GUIUtility.keyboardControl = 0;

        // IdentifiedObject 객체의 변수들 가져오기
        categoriesProperty = serializedObject.FindProperty("categories");
        iconProperty = serializedObject.FindProperty("icon");
        idProperty = serializedObject.FindProperty("id");
        codeNameProperty = serializedObject.FindProperty("codeName");
        displayNameProperty = serializedObject.FindProperty("displayName");
        descriptionProperty = serializedObject.FindProperty("description");

        categories = new(serializedObject, categoriesProperty);
        // List의 Prefix Label을 어떻게 그릴지 정함
        categories.drawHeaderCallback = rect => EditorGUI.LabelField(rect, categoriesProperty.displayName);
        // List의 Element를 어떻게 그릴지 정함
        categories.drawElementCallback = (rect, index, isActive, isFocused) => {
            rect = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rect, categoriesProperty.GetArrayElementAtIndex(index), GUIContent.none);
        };
    }

    private void StyleSetup()
    {
        // 텍스트를 여러줄 쓸 수 있는 textArea의 스타일 지정
        if (textAreaStyle == null)
        {
            // Style의 기본 양식은 textArea.
            textAreaStyle = new(EditorStyles.textArea);
            // 문자열이 TextBox 밖으로 못 빠져나가게 함.
            textAreaStyle.wordWrap = true;
        }
    }

    protected bool DrawFoldoutTitle(string text)
    => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedesByTitle, text);

    public override void OnInspectorGUI()
    {
        StyleSetup();

        // 객체의 Serialize 변수들의 값을 업데이트함.
        serializedObject.Update();

        // List를 그려줌
        categories.DoLayoutList();

        if (DrawFoldoutTitle("Infomation"))
        {
            // (1) 지금부터 그릴 객체를 가로로 정렬하며, 배경을 테두리 있는 회색으로 채움(=HelpBox는 유니티 내부에 정의되어 있는 Style임)
            // 중괄호는 작성할 필요는 없지만 명확한 구분을 위해 넣어준 것이기 때문에 스타일에 따라 중괄호는 안넣어도 됨.
            EditorGUILayout.BeginHorizontal("HelpBox");
            {
                //Sprite를 Preview로 볼 수 있게 변수를 그려줌
                iconProperty.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none, iconProperty.objectReferenceValue,
                    typeof(Sprite), false, GUILayout.Width(65));

                // (2) 지금부터 그릴 객체는 세로로 정렬한다.
                // 위 icon 변수는 왼쪽에 그려지고, 지금부터 그릴 변수들은 오른쪽에 세로로 그려짐.
                EditorGUILayout.BeginVertical();
                {
                    // (3) 지금부터 그릴 객체는 가로로 정렬한다.
                    // id 변수의 prefix(= inspector에서 보이는 변수의 이름)을 따로 지정해주기 위해 변수 Line을 직접 만듬.
                    EditorGUILayout.BeginHorizontal();
                    {
                        // 변수 편집 Disable, ID는 Database에서 직접 Set해줄 것이기 때문에 사용자가 직접 편집하지 못하도록 함.
                        GUI.enabled = false;
                        // 변수의 선행 명칭(Prefix) 지정
                        EditorGUILayout.PrefixLabel("ID");
                        // id 변수를 그리되 Prefix는 그리지않음(=GUIContent.none); 
                        EditorGUILayout.PropertyField(idProperty, GUIContent.none);
                        // 변수 편집 Enable
                        GUI.enabled = true;
                    }
                    // (3) 가로 정렬 종료
                    EditorGUILayout.EndHorizontal();

                    // 지금부터 변수가 수정되었는지 검사한다.
                    EditorGUI.BeginChangeCheck();
                    var prevCodeName = codeNameProperty.stringValue;
                    // codeName 변수를 그리되, 사용자가 Enter 키를 누를 때까지 값 변경은 보류함.
                    EditorGUILayout.DelayedTextField(codeNameProperty);
                    // 변수가 수정되었는지 확인, codeName 변수가 수정되었다면 수정된 값으로 현재 객체의 이름을 바꿔줌.
                    if (EditorGUI.EndChangeCheck())
                    {
                        // 현재 객체의 유니티 프로젝트상의 주소를 가져옴.
                        // target == IdentifiedObject, var identifiedObject = target as IdentifiecObject 이런 식으로 사용할 수 있음.
                        // serializeObject.targetObject == target
                        var assetPath = AssetDatabase.GetAssetPath(target);
                        // 새로운 이름은 '(변수의 Type)_(codeName)'
                        var newName = $"{target.GetType().Name.ToUpper()}_{codeNameProperty.stringValue}";

                        // Serialize 변수들의 값 변화를 적용함(=디스크에 저장함)
                        // 이 작업을 해주지 않으면 바뀐 값이 적용되지 않아서 이전 값으로 돌아감
                        serializedObject.ApplyModifiedProperties();

                        // 객체의 Project View에서 보이는 이름을 수정함. 만약 같은 이름을 가진 객체가 있을 경우 실패함.
                        var message = AssetDatabase.RenameAsset(assetPath, newName);
                        // 성공했을 경우 객체의 내부 이름도 바꿔줌. 외부 이름과 내부 이름이 다를 시 유니티에서 경고를 띄우고,
                        // 실제 프로젝트에서도 문제를 일으킬 가능성이 높기에 항상 이름을 일치시켜줘야함
                        if (string.IsNullOrEmpty(message))
                            target.name = newName;
                        else
                            codeNameProperty.stringValue = prevCodeName;
                    }

                    // displayName 변수를 그려줌
                    EditorGUILayout.PropertyField(displayNameProperty);
                }
                // (2) 세로 정렬 종료
                EditorGUILayout.EndVertical();
            }
            // (1) 가로 정렬 종료
            EditorGUILayout.EndHorizontal();

            // 세로 정렬 시작, 기본적으로 세로 정렬이 Default 정렬이기 때문에 가로 정렬 내부에 사용하는게 아니라면
            // 직접 세로 정렬을 해줄 필요가 없지만 이 경우에는 HelpBox로 내부를 회색으로 채우기위해 직접 세로 정렬을 함
            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Description이라는 Lebel을 띄워줌
                EditorGUILayout.LabelField("Description");
                // TextField를 넓은 형태(TextArea)로 그려줌
                descriptionProperty.stringValue = EditorGUILayout.TextArea(descriptionProperty.stringValue,
                    textAreaStyle, GUILayout.Height(60));
            }
            EditorGUILayout.EndVertical();
            // 세로 정렬 종료
        }

        // Serialize 변수들의 값 변화를 적용함(=디스크에 저장함)
        // 이 작업을 해주지 않으면 바뀐 값이 적용되지 않아서 이전 값으로 돌아감
        serializedObject.ApplyModifiedProperties();
    }


    // Data의 Level과 Data 삭제를 위한 X Button을 그려주는 Foldout Title을 그려줌
    protected bool DrawRemovableLevelFoldout(SerializedProperty datasProperty, SerializedProperty targetProperty,
        int targetIndex, bool isDrawRemoveButton)
    {
        /// 게임 설계상 X버튼을 사용할 일은 없어졌으나 
        /// DeleteArrayElementAtIndex 유니티 내장함수를 통해 배열 프로퍼티의 원소를 제거가능하다는 것은 중요

        // Data를 삭제했는지에 대한 결과
        bool isRemoveButtonClicked = false;

        EditorGUILayout.BeginHorizontal();
        {
            GUI.color = Color.green;
            var level = targetProperty.FindPropertyRelative("level").intValue;
            // Data의 Level을 보여주는 Foldout GUI를 그려줌
            targetProperty.isExpanded = EditorGUILayout.Foldout(targetProperty.isExpanded, $"Level {level}");
            GUI.color = Color.white;

            if (isDrawRemoveButton)
            {
                GUI.color = Color.red;
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    isRemoveButtonClicked = true;
                    // EffectDatas에서 현재 Data를 Index를 이용해 지움
                    datasProperty.DeleteArrayElementAtIndex(targetIndex);
                }
                GUI.color = Color.white;
            }
        }
        EditorGUILayout.EndHorizontal();

        return isRemoveButtonClicked;
    }
    
    // 스킬, 이펙트 등 '레벨'이 있는 데이터를 레벨 순으로 정렬
    protected void DrawAutoSortLevelProperty(SerializedProperty datasProperty, SerializedProperty levelProperty,
        int index, bool isEditable)
    {
        /// 게임 설계상 레벨정렬은 필요가 없어졌으나
        /// MoveArrayElement 함수로 배열의 인덱스 위치를 옮길 수 있는것은 중요

        if (!isEditable)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(levelProperty);
            GUI.enabled = true;
        }
        else
        {
            // Property가 수정되었는지 감시 시작
            EditorGUI.BeginChangeCheck();
            // 수정전 Level을 기록해둠
            var prevValue = levelProperty.intValue;
            // levelProperty를 Delayed 방식으로 그려줌
            // 키보드 Enter Key를 눌러야 입력한 값이 반영됨, Enter Key를 누르지않고 빠져나가면 원래 값으로 돌아옴.
            EditorGUILayout.DelayedIntField(levelProperty);
            // Property가 수정되었을 경우 true 반환
            if (EditorGUI.EndChangeCheck())
            {
                if (levelProperty.intValue <= 1)
                    levelProperty.intValue = prevValue;
                else
                {
                    // EffectDatas를 순회하여 같은 level을 가진 data가 이미 있으면 수정 전 level로 되돌림
                    for (int i = 0; i < datasProperty.arraySize; i++)
                    {
                        // 확인해야하는 Data가 현재 Data와 동일하다면 Skip
                        if (index == i)
                            continue;

                        var element = datasProperty.GetArrayElementAtIndex(i);
                        // Level이 똑같으면 현재 Data의 Level을 수정 전으로 되돌림
                        if (element.FindPropertyRelative("level").intValue == levelProperty.intValue)
                        {
                            levelProperty.intValue = prevValue;
                            break;
                        }
                    }

                    // Level이 정상적으로 수정되었다면 오름차순 정렬 작업 실행
                    if (levelProperty.intValue != prevValue)
                    {
                        // 현재 Data의 Level이 i번째 Data의 Level보다 작으면, 현재 Data를 i번째로 옮김
                        // ex. 1 2 4 5 (3) => 1 2 (3) 4 5
                        for (int moveIndex = 1; moveIndex < datasProperty.arraySize; moveIndex++)
                        {
                            if (moveIndex == index)
                                continue;

                            var element = datasProperty.GetArrayElementAtIndex(moveIndex).FindPropertyRelative("level");
                            if (levelProperty.intValue < element.intValue || moveIndex == (datasProperty.arraySize - 1))
                            {
                                datasProperty.MoveArrayElement(index, moveIndex);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
