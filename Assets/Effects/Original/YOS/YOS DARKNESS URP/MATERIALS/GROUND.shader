// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "YOS/DARKNESS/GROUND"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_MASK("MASK", 2D) = "white" {}
		_NOISE("NOISE", 2D) = "white" {}
		_NOISEUV("NOISEUV", 2D) = "white" {}
		_LERPNOISEUV("LERPNOISEUV", Range( 0 , 1)) = 0.1904571
		_GRADIENTE("GRADIENTE", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (1,1,1,0)
		_NOISEVELXYSCALEZPOWERW("NOISE VEL XY SCALE Z POWER W", Vector) = (0,-0.1,1,1)
		_UVDISTVELXYSCALEZPOWERW("UV DIST VEL XY SCALE Z POWER W", Vector) = (0,-0.1,1,1)
		_opacidad("opacidad", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha , One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _GRADIENTE;
			sampler2D _NOISE;
			sampler2D _NOISEUV;
			sampler2D _MASK;
			CBUFFER_START( UnityPerMaterial )
			float4 _NOISEVELXYSCALEZPOWERW;
			float4 _UVDISTVELXYSCALEZPOWERW;
			float _LERPNOISEUV;
			float4 _Color0;
			float4 _MASK_ST;
			float _opacidad;
			CBUFFER_END


			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif
				float4 appendResult44 = (float4(_NOISEVELXYSCALEZPOWERW.x , _NOISEVELXYSCALEZPOWERW.y , 0.0 , 0.0));
				float2 uv021 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CenteredUV15_g16 = ( uv021 - float2( 0.5,0.5 ) );
				float2 break17_g16 = CenteredUV15_g16;
				float2 appendResult23_g16 = (float2(( length( CenteredUV15_g16 ) * _NOISEVELXYSCALEZPOWERW.z * 2.0 ) , ( atan2( break17_g16.x , break17_g16.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				float2 temp_output_8_0 = appendResult23_g16;
				float4 appendResult29 = (float4((temp_output_8_0).y , (temp_output_8_0).x , 0.0 , 0.0));
				float2 panner37 = ( 1.0 * _Time.y * appendResult44.xy + appendResult29.xy);
				float4 appendResult50 = (float4(_UVDISTVELXYSCALEZPOWERW.x , _UVDISTVELXYSCALEZPOWERW.y , 0.0 , 0.0));
				float2 uv047 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CenteredUV15_g15 = ( uv047 - float2( 0.5,0.5 ) );
				float2 break17_g15 = CenteredUV15_g15;
				float2 appendResult23_g15 = (float2(( length( CenteredUV15_g15 ) * _UVDISTVELXYSCALEZPOWERW.z * 2.0 ) , ( atan2( break17_g15.x , break17_g15.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				float2 temp_output_33_0 = appendResult23_g15;
				float4 appendResult35 = (float4((temp_output_33_0).y , (temp_output_33_0).x , 0.0 , 0.0));
				float2 panner36 = ( 1.0 * _Time.y * appendResult50.xy + appendResult35.xy);
				float2 temp_cast_4 = (pow( tex2D( _NOISEUV, panner36 ).r , _UVDISTVELXYSCALEZPOWERW.w )).xx;
				float2 lerpResult25 = lerp( panner37 , temp_cast_4 , _LERPNOISEUV);
				float4 tex2DNode6 = tex2D( _NOISE, lerpResult25 );
				float2 temp_cast_5 = (pow( tex2DNode6.r , _NOISEVELXYSCALEZPOWERW.w )).xx;
				
				float2 uv_MASK = IN.ase_texcoord3.xy * _MASK_ST.xy + _MASK_ST.zw;
				float clampResult54 = clamp( ( tex2DNode6.r * tex2D( _MASK, uv_MASK ).r * _opacidad ) , 0.0 , 1.0 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( tex2D( _GRADIENTE, temp_cast_5 ) * _Color0 ).rgb;
				float Alpha = ( clampResult54 * IN.ase_color.a );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _NOISE;
			sampler2D _NOISEUV;
			sampler2D _MASK;
			CBUFFER_START( UnityPerMaterial )
			float4 _NOISEVELXYSCALEZPOWERW;
			float4 _UVDISTVELXYSCALEZPOWERW;
			float _LERPNOISEUV;
			float4 _Color0;
			float4 _MASK_ST;
			float _opacidad;
			CBUFFER_END


			
			float3 _LightDirection;

			VertexOutput ShadowPassVertex( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir( v.ase_normal );

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;

				return o;
			}

			half4 ShadowPassFragment(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 appendResult44 = (float4(_NOISEVELXYSCALEZPOWERW.x , _NOISEVELXYSCALEZPOWERW.y , 0.0 , 0.0));
				float2 uv021 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CenteredUV15_g16 = ( uv021 - float2( 0.5,0.5 ) );
				float2 break17_g16 = CenteredUV15_g16;
				float2 appendResult23_g16 = (float2(( length( CenteredUV15_g16 ) * _NOISEVELXYSCALEZPOWERW.z * 2.0 ) , ( atan2( break17_g16.x , break17_g16.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				float2 temp_output_8_0 = appendResult23_g16;
				float4 appendResult29 = (float4((temp_output_8_0).y , (temp_output_8_0).x , 0.0 , 0.0));
				float2 panner37 = ( 1.0 * _Time.y * appendResult44.xy + appendResult29.xy);
				float4 appendResult50 = (float4(_UVDISTVELXYSCALEZPOWERW.x , _UVDISTVELXYSCALEZPOWERW.y , 0.0 , 0.0));
				float2 uv047 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CenteredUV15_g15 = ( uv047 - float2( 0.5,0.5 ) );
				float2 break17_g15 = CenteredUV15_g15;
				float2 appendResult23_g15 = (float2(( length( CenteredUV15_g15 ) * _UVDISTVELXYSCALEZPOWERW.z * 2.0 ) , ( atan2( break17_g15.x , break17_g15.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				float2 temp_output_33_0 = appendResult23_g15;
				float4 appendResult35 = (float4((temp_output_33_0).y , (temp_output_33_0).x , 0.0 , 0.0));
				float2 panner36 = ( 1.0 * _Time.y * appendResult50.xy + appendResult35.xy);
				float2 temp_cast_4 = (pow( tex2D( _NOISEUV, panner36 ).r , _UVDISTVELXYSCALEZPOWERW.w )).xx;
				float2 lerpResult25 = lerp( panner37 , temp_cast_4 , _LERPNOISEUV);
				float4 tex2DNode6 = tex2D( _NOISE, lerpResult25 );
				float2 uv_MASK = IN.ase_texcoord2.xy * _MASK_ST.xy + _MASK_ST.zw;
				float clampResult54 = clamp( ( tex2DNode6.r * tex2D( _MASK, uv_MASK ).r * _opacidad ) , 0.0 , 1.0 );
				
				float Alpha = ( clampResult54 * IN.ase_color.a );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _NOISE;
			sampler2D _NOISEUV;
			sampler2D _MASK;
			CBUFFER_START( UnityPerMaterial )
			float4 _NOISEVELXYSCALEZPOWERW;
			float4 _UVDISTVELXYSCALEZPOWERW;
			float _LERPNOISEUV;
			float4 _Color0;
			float4 _MASK_ST;
			float _opacidad;
			CBUFFER_END


			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 appendResult44 = (float4(_NOISEVELXYSCALEZPOWERW.x , _NOISEVELXYSCALEZPOWERW.y , 0.0 , 0.0));
				float2 uv021 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CenteredUV15_g16 = ( uv021 - float2( 0.5,0.5 ) );
				float2 break17_g16 = CenteredUV15_g16;
				float2 appendResult23_g16 = (float2(( length( CenteredUV15_g16 ) * _NOISEVELXYSCALEZPOWERW.z * 2.0 ) , ( atan2( break17_g16.x , break17_g16.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				float2 temp_output_8_0 = appendResult23_g16;
				float4 appendResult29 = (float4((temp_output_8_0).y , (temp_output_8_0).x , 0.0 , 0.0));
				float2 panner37 = ( 1.0 * _Time.y * appendResult44.xy + appendResult29.xy);
				float4 appendResult50 = (float4(_UVDISTVELXYSCALEZPOWERW.x , _UVDISTVELXYSCALEZPOWERW.y , 0.0 , 0.0));
				float2 uv047 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CenteredUV15_g15 = ( uv047 - float2( 0.5,0.5 ) );
				float2 break17_g15 = CenteredUV15_g15;
				float2 appendResult23_g15 = (float2(( length( CenteredUV15_g15 ) * _UVDISTVELXYSCALEZPOWERW.z * 2.0 ) , ( atan2( break17_g15.x , break17_g15.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				float2 temp_output_33_0 = appendResult23_g15;
				float4 appendResult35 = (float4((temp_output_33_0).y , (temp_output_33_0).x , 0.0 , 0.0));
				float2 panner36 = ( 1.0 * _Time.y * appendResult50.xy + appendResult35.xy);
				float2 temp_cast_4 = (pow( tex2D( _NOISEUV, panner36 ).r , _UVDISTVELXYSCALEZPOWERW.w )).xx;
				float2 lerpResult25 = lerp( panner37 , temp_cast_4 , _LERPNOISEUV);
				float4 tex2DNode6 = tex2D( _NOISE, lerpResult25 );
				float2 uv_MASK = IN.ase_texcoord2.xy * _MASK_ST.xy + _MASK_ST.zw;
				float clampResult54 = clamp( ( tex2DNode6.r * tex2D( _MASK, uv_MASK ).r * _opacidad ) , 0.0 , 1.0 );
				
				float Alpha = ( clampResult54 * IN.ase_color.a );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18000
616;452;999;567;-1978.156;457.3339;1.172041;True;False
Node;AmplifyShaderEditor.Vector4Node;46;-635.0923,62.55375;Inherit;False;Property;_UVDISTVELXYSCALEZPOWERW;UV DIST VEL XY SCALE Z POWER W;7;0;Create;True;0;0;False;0;0,-0.1,1,1;0,-0.1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-732.5482,-323.7794;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;33;-438.6328,-308.0132;Inherit;True;Polar Coordinates;-1;;15;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;43;-56.84304,-858.8713;Inherit;False;Property;_NOISEVELXYSCALEZPOWERW;NOISE VEL XY SCALE Z POWER W;6;0;Create;True;0;0;False;0;0,-0.1,1,1;0,-0.1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-82.28339,-561.1307;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;32;-96.6412,-316.1354;Inherit;False;True;False;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;34;-77.07691,-231.4246;Inherit;False;False;True;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;8;253.7896,-598.6536;Inherit;True;Polar Coordinates;-1;;16;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;50;-191.2571,14.85678;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;151.1586,-271.2787;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;36;387.0895,-229.3988;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.05;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;28;587.2432,-465.6342;Inherit;False;False;True;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;27;569.5066,-550.3448;Inherit;False;True;False;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;406.8714,-795.1741;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;839.8035,-540.2863;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;7;514.7117,7.421883;Inherit;True;Property;_NOISEUV;NOISEUV;2;0;Create;True;0;0;False;0;-1;4e3951c538fc8a647a4a10a99b480987;4e3951c538fc8a647a4a10a99b480987;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;854.428,55.74786;Inherit;False;Property;_LERPNOISEUV;LERPNOISEUV;3;0;Create;True;0;0;False;0;0.1904571;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;37;1224.939,-388.6412;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;49;830.3071,348.3896;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;25;1167.512,30.60589;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;6;1520.095,-537.2999;Inherit;True;Property;_NOISE;NOISE;1;0;Create;True;0;0;False;0;-1;7e29b90f8efb65f49b24c11e95f63117;7e29b90f8efb65f49b24c11e95f63117;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;1574.844,21.30156;Inherit;True;Property;_MASK;MASK;0;0;Create;True;0;0;False;0;-1;b8e81f89fcf35134991472fb895535c8;b8e81f89fcf35134991472fb895535c8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;53;2122.818,196.6143;Inherit;False;Property;_opacidad;opacidad;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;2227.706,-186.7945;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;51;2506.047,217.8483;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;54;2516.123,-94.58725;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;45;1861.225,-598.1398;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;39;2204.226,-504.9066;Inherit;True;Property;_GRADIENTE;GRADIENTE;4;0;Create;True;0;0;False;0;-1;d5f532af20f3aee4ca216b757d0e7dbb;d5f532af20f3aee4ca216b757d0e7dbb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;2405.838,-668.3208;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;2823.771,-82.39935;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;41;1731.723,-811.8937;Inherit;False;Property;_Color0;Color 0;5;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;3017.781,-411.7426;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;YOS/DARKNESS/GROUND;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;7;False;False;False;True;2;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;0;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;11;Surface;1;  Blend;0;Two Sided;0;Cast Shadows;1;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;0;Meta Pass;0;Extra Pre Pass;0;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;True;True;False;False;;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-44.3926,372.0522;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
WireConnection;33;1;47;0
WireConnection;33;3;46;3
WireConnection;32;0;33;0
WireConnection;34;0;33;0
WireConnection;8;1;21;0
WireConnection;8;3;43;3
WireConnection;50;0;46;1
WireConnection;50;1;46;2
WireConnection;35;0;34;0
WireConnection;35;1;32;0
WireConnection;36;0;35;0
WireConnection;36;2;50;0
WireConnection;28;0;8;0
WireConnection;27;0;8;0
WireConnection;44;0;43;1
WireConnection;44;1;43;2
WireConnection;29;0;28;0
WireConnection;29;1;27;0
WireConnection;7;1;36;0
WireConnection;37;0;29;0
WireConnection;37;2;44;0
WireConnection;49;0;7;1
WireConnection;49;1;46;4
WireConnection;25;0;37;0
WireConnection;25;1;49;0
WireConnection;25;2;26;0
WireConnection;6;1;25;0
WireConnection;38;0;6;1
WireConnection;38;1;5;1
WireConnection;38;2;53;0
WireConnection;54;0;38;0
WireConnection;45;0;6;1
WireConnection;45;1;43;4
WireConnection;39;1;45;0
WireConnection;40;0;39;0
WireConnection;40;1;41;0
WireConnection;52;0;54;0
WireConnection;52;1;51;4
WireConnection;1;2;40;0
WireConnection;1;3;52;0
ASEEND*/
//CHKSM=FC363972D71B43DB97D04AE34AE3766F53D241F8