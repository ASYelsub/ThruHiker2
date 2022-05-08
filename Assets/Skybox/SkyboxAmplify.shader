// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SkyboxAmplify"
{
	Properties
	{
		_Cubemap("Cubemap", CUBE) = "white" {}
		_Moon("Moon", 2D) = "white" {}
		_Offset("Offset", Float) = 0
		_Contrast("Contrast", Float) = 0
		_ColorHigh("Color High", Color) = (0,0,0,0)
		_ColorLow("Color Low", Color) = (0,0,0,0)
		_Moonoffset("Moon offset", Vector) = (0,0,0,0)
		_MoonSize("Moon Size", Float) = 5
		_ColorSteps("Color Steps", Float) = 16
		_SunColor("SunColor", Color) = (1,0.967224,0.3066038,1)
		_Sunparam1("Sun param 1", Float) = 0
		_Sunparam3("Sun param 3", Float) = 2
		_Sunparam2("Sun param 2", Float) = 0
		_FogColor("Fog Color", Color) = (0,0,0,0)
		_FogIntensity("Fog Intensity", Float) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			//This is a late directive
			
			uniform float _ColorSteps;
			uniform float4 _ColorLow;
			uniform float4 _ColorHigh;
			uniform float _Offset;
			uniform float _Contrast;
			uniform samplerCUBE _Cubemap;
			uniform sampler2D _Moon;
			uniform float _MoonSize;
			uniform float3 _Moonoffset;
			uniform float _Sunparam1;
			uniform float4 _SunColor;
			uniform float _Sunparam3;
			uniform float _Sunparam2;
			uniform float _FogIntensity;
			uniform float4 _FogColor;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1 = v.vertex;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float clampResult33 = clamp( ( i.ase_texcoord1.xyz.y + _Offset ) , 0.0 , 1.0 );
				float lerpResult82 = lerp( 1.0 , 0.0 , pow( (0.0 + (distance( i.ase_texcoord1.xyz , _WorldSpaceLightPos0.xyz ) - 0.0) * (1.0 - 0.0) / (2.0 - 0.0)) , 1.0 ));
				float4 lerpResult38 = lerp( _ColorLow , _ColorHigh , ( pow( clampResult33 , _Contrast ) + lerpResult82 ));
				float4 texCUBENode29 = texCUBE( _Cubemap, i.ase_texcoord1.xyz );
				float4 tex2DNode30 = tex2D( _Moon, ( ( i.ase_texcoord1.xyz * _MoonSize ) - _Moonoffset ).xy );
				float3 worldSpaceLightDir = UnityWorldSpaceLightDir(WorldPosition);
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(WorldPosition);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult147 = dot( -worldSpaceLightDir , ase_worldViewDir );
				float4 temp_cast_2 = (_Sunparam2).xxxx;
				float4 temp_output_151_0 = pow( saturate( ( pow( dotResult147 , _Sunparam1 ) * _SunColor * _Sunparam3 ) ) , temp_cast_2 );
				float grayscale155 = Luminance(temp_output_151_0.rgb);
				float div77=256.0/float((int)_ColorSteps);
				float4 posterize77 = ( floor( ( ( saturate( lerpResult38 ) * ( 1.0 - ( texCUBENode29.a + tex2DNode30.a + grayscale155 ) ) ) + ( texCUBENode29 * texCUBENode29.a ) + ( tex2DNode30 * tex2DNode30.a ) + temp_output_151_0 ) * div77 ) / div77 );
				
				
				finalColor = ( ( posterize77 * ( 1.0 - _FogIntensity ) ) + ( _FogIntensity * _FogColor ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18935
137;73;1199;877;736.3696;-258.5256;1.341951;True;False
Node;AmplifyShaderEditor.CommentaryNode;157;-3281.483,1590.105;Inherit;False;1810.569;658.258;Comment;13;144;145;146;147;153;137;150;149;148;154;152;151;155;Procedural Sun;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;144;-3231.483,1640.105;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;44;-2444.136,-578.1746;Inherit;False;1108.806;791.195;Comment;12;32;33;36;34;35;43;82;83;84;87;88;91;Atmosphere color weight;1,1,1,1;0;0
Node;AmplifyShaderEditor.NegateNode;146;-2968.417,1771.087;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;145;-3231.049,1783.982;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightPos;87;-2410.993,86.99002;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.PosVertexDataNode;84;-2410.717,-92.43599;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;153;-2773.589,1927.394;Inherit;False;Property;_Sunparam1;Sun param 1;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;147;-2795.894,1772.302;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;43;-2394.136,-528.1748;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;35;-2233.204,-382.3323;Inherit;False;Property;_Offset;Offset;3;0;Create;True;0;0;0;False;0;False;0;-0.008;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;83;-2128.627,-63.68406;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-2557.037,1935.716;Inherit;False;Property;_Sunparam3;Sun param 3;13;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;137;-2513.578,2036.363;Inherit;False;Property;_SunColor;SunColor;11;0;Create;True;0;0;0;False;0;False;1,0.967224,0.3066038,1;1,0.8938043,0.2971698,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;149;-2625.408,1679.334;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;15;-2627.7,550.5481;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;54;-2196.679,823.4462;Inherit;False;Property;_MoonSize;Moon Size;8;0;Create;True;0;0;0;False;0;False;5;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;91;-1964.731,-66.83165;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;2;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-2344.646,1722.482;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-2092.804,-461.6321;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;88;-1767.035,-25.00267;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;33;-1828.904,-431.7319;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;58;-1893.199,835.051;Inherit;False;Property;_Moonoffset;Moon offset;7;0;Create;True;0;0;0;False;0;False;0,0,0;3,3.45,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;152;-2177.441,1760.842;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-2034.352,1931.667;Inherit;False;Property;_Sunparam2;Sun param 2;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-2054.73,746.262;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1818.504,-264.0323;Inherit;False;Property;_Contrast;Contrast;4;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;82;-1599.962,-108.0044;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;151;-2014.36,1758.385;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;32;-1607.903,-433.0319;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;57;-1867.731,746.2986;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;41;-1206.862,-273.8273;Inherit;False;Property;_ColorHigh;Color High;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4666667,0.9176471,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-1326.879,-118.5495;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-1692.537,719.9607;Inherit;True;Property;_Moon;Moon;1;0;Create;True;0;0;0;False;0;False;-1;9f8d0a5311f9c414ebc15c563a315124;9f8d0a5311f9c414ebc15c563a315124;True;0;False;white;LockedToTexture2D;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;-1686.021,509.558;Inherit;True;Property;_Cubemap;Cubemap;0;0;Create;True;0;0;0;False;0;False;-1;None;4d28ab2e211a0114782d77fe06adcaaa;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;39;-1210.763,-475.3265;Inherit;False;Property;_ColorLow;Color Low;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.7450981,0.2862745,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;155;-1682.914,1724.787;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;-830.2277,82.40605;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-1287.702,400.6676;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;37;-632.9736,281.0454;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;48;-662.9854,505.289;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-467.4423,369.1071;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1287.353,552.5332;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1262.802,720.2987;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;160;109.0595,926.8173;Inherit;False;Property;_FogIntensity;Fog Intensity;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-159.36,777.7807;Inherit;False;Property;_ColorSteps;Color Steps;10;0;Create;True;0;0;0;False;0;False;16;16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-351.9007,591.3891;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;163;63.43314,1085.168;Inherit;False;Property;_FogColor;Fog Color;15;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;159;188.2346,843.6163;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosterizeNode;77;-1.222662,582.9713;Inherit;False;1;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;491.5155,942.9205;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;156;-4798.926,600.5018;Inherit;False;1710.594;672.0271;Comment;13;31;104;69;100;76;105;68;106;107;74;55;56;102;Old sun code;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;314.378,594.0136;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-3419.417,799.757;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;31;-3705.373,804.0768;Inherit;True;Property;_Sun;Sun;2;0;Create;True;0;0;0;False;0;False;-1;None;af8d5cda404b54949a39441da3855da3;True;0;False;white;LockedToTexture2D;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;104;-4225.323,1072.824;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-4243.156,727.7248;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;69;-4483.625,887.908;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-4233.656,878.9469;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;102;-3241.332,650.5018;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;56;-4495.814,761.1791;Inherit;False;Property;_SunSize;Sun Size;9;0;Create;True;0;0;0;False;0;False;5;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-3967.322,800.4556;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0.5,0.5,0.5;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;107;-3872.098,935.442;Inherit;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;-1,-1,-1;False;2;FLOAT3;1,1,1;False;3;FLOAT3;0,0,0;False;4;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;106;-3839.558,1113.528;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;68;-4748.927,888.0409;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;105;-4038.249,1120.314;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;161;502.2512,592.6714;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;27;666.7742,576.0958;Float;False;True;-1;2;ASEMaterialInspector;100;1;SkyboxAmplify;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;146;0;144;0
WireConnection;147;0;146;0
WireConnection;147;1;145;0
WireConnection;83;0;84;0
WireConnection;83;1;87;1
WireConnection;149;0;147;0
WireConnection;149;1;153;0
WireConnection;91;0;83;0
WireConnection;148;0;149;0
WireConnection;148;1;137;0
WireConnection;148;2;150;0
WireConnection;34;0;43;2
WireConnection;34;1;35;0
WireConnection;88;0;91;0
WireConnection;33;0;34;0
WireConnection;152;0;148;0
WireConnection;53;0;15;0
WireConnection;53;1;54;0
WireConnection;82;2;88;0
WireConnection;151;0;152;0
WireConnection;151;1;154;0
WireConnection;32;0;33;0
WireConnection;32;1;36;0
WireConnection;57;0;53;0
WireConnection;57;1;58;0
WireConnection;80;0;32;0
WireConnection;80;1;82;0
WireConnection;30;1;57;0
WireConnection;29;1;15;0
WireConnection;155;0;151;0
WireConnection;38;0;39;0
WireConnection;38;1;41;0
WireConnection;38;2;80;0
WireConnection;46;0;29;4
WireConnection;46;1;30;4
WireConnection;46;2;155;0
WireConnection;37;0;38;0
WireConnection;48;0;46;0
WireConnection;47;0;37;0
WireConnection;47;1;48;0
WireConnection;50;0;29;0
WireConnection;50;1;29;4
WireConnection;51;0;30;0
WireConnection;51;1;30;4
WireConnection;49;0;47;0
WireConnection;49;1;50;0
WireConnection;49;2;51;0
WireConnection;49;3;151;0
WireConnection;159;0;160;0
WireConnection;77;1;49;0
WireConnection;77;0;78;0
WireConnection;162;0;160;0
WireConnection;162;1;163;0
WireConnection;158;0;77;0
WireConnection;158;1;159;0
WireConnection;100;0;31;0
WireConnection;100;1;106;0
WireConnection;31;1;107;0
WireConnection;104;0;15;0
WireConnection;104;1;68;0
WireConnection;55;0;15;0
WireConnection;55;1;56;0
WireConnection;69;0;68;0
WireConnection;76;0;69;0
WireConnection;76;1;56;0
WireConnection;102;0;100;0
WireConnection;74;0;55;0
WireConnection;74;2;76;0
WireConnection;107;0;74;0
WireConnection;106;0;105;0
WireConnection;105;0;104;0
WireConnection;161;0;158;0
WireConnection;161;1;162;0
WireConnection;27;0;161;0
ASEEND*/
//CHKSM=6D7888324FD9D2CC067B507688A861A01F61998F