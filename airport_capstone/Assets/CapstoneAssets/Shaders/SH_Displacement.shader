// This shader adds tessellation in URP
Shader "Custom/Displacement"
{
 
	// The properties block of the Unity shader. In this example this block is empty
	// because the output color is predefined in the fragment shader code.
	Properties
	{
		_SnowTex("SnowTex", 2D) = "gray" {}
		_NormalMap("NormalMap", 2D) = "bump" {}
		_NormalStrenght("Normal Strenght", Range(0,2)) = 1
		_Tilling("Tilling", Range(1,200)) = 1
		_Tess("Tessellation", Range(1, 10000)) = 20
		_MaxTessDistance("Max Tess Distance", Range(1, 10000)) = 20

		_RenderTexture("RenderTexture", 2D) = "gray" {} 
		_Weight("Displacement Amount", Range(0, 3)) = 0
		_WeightAndains("Displacement Amount Andains", Range(0, 1)) = 0
		_TopColor("TopColor", color) = (1,1,1,1)
		_MidColor("MidColor", color) = (1,1,1,1)
		_BotColor("BotColor", color) = (1,1,1,1)
		_GradientCenter("_GradientCenter", Range(-2,2)) = 0
		_GradientCenter2("_GradientCenter2", Range(-2,2)) = 0
		_Spread("Spread", Range(0.001,0.999)) = 0
	}
 
	// The SubShader block containing the Shader code. 
	SubShader
	{
		// SubShader Tags define when and under which conditions a SubShader block or
		// a pass is executed.
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
 
		Pass
		{
			Tags{ "LightMode" = "UniversalForward" }
			Lighting On
 
			// The HLSL code block. Unity SRP uses the HLSL language.
			HLSLPROGRAM
			// The Core.hlsl file contains definitions of frequently used HLSL
			// macros and functions, and also contains #include references to other
			// HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"    
			#include "Assets/HEC/Shaders/Snow/CustomTessellation.shader"			
			//#include "UnityCG.cginc"
 
			// This line defines the name of the hull shader. 
			#pragma hull hull
			// This line defines the name of the domain shader. 
			#pragma domain domain
			// This line defines the name of the vertex shader. 
			#pragma vertex TessellationVertexProgram
			// This line defines the name of the fragment shader. 
			#pragma fragment frag

			sampler2D _SnowTex;			
			sampler2D _NormalMap;
			float _NormalStrenght;
			float _Tilling;
			sampler2D _RenderTexture;
			float _Weight;
			float _WeightAndains;
			float4 _TopColor;
			float4 _MidColor;
			float4 _BotColor;
			float _Spread;
			float _GradientCenter;
			float _GradientCenter2;
			
			// pre tesselation vertex program
			ControlPoint TessellationVertexProgram(Attributes v)
			{
				ControlPoint p;
 
				p.vertex = v.vertex;
				p.uv = v.uv;
				p.normal = v.normal;
				p.color = v.color;
 
				return p;
			}
 
			// after tesselation
			Varyings vert(Attributes input)
			{
				Varyings output;				
 
				float r = tex2Dlod(_RenderTexture, float4(input.uv.xy, 0, 0)).r;// andins + sol
				// float4 g = tex2Dlod(_RenderTexture, float4(input.uv.xy, 0, 0)).g;// andins + sol mais plus d'andins
				// float4 b = tex2Dlod(_RenderTexture, float4(input.uv.xy, 0, 0)).b;// andins + sol
				// float4 a = tex2Dlod(_RenderTexture, float4(input.uv.xy, 0, 0)).a;// andins + trou

				// float4 DiggedMask = a - r;
				// float4 WindrowMask = r - DiggedMask;
				// float4 d = r - WindrowMask;
 
				input.vertex.y += (r>0.5f)?r * _Weight*(1+_WeightAndains): r * _Weight;
				output.vertex = TransformObjectToHClip(input.vertex.xyz); 
				output.color = mul(UNITY_MATRIX_I_M, input.vertex); //TransformObjectToHClip(input.vertex.xyz);
				output.uv = input.uv;				
				output.normal = input.normal;

				return output;
			}
 
			[UNITY_domain("tri")]
			Varyings domain(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
			{
				Attributes v;
				// interpolate the new positions of the tessellated mesh
				Interpolate(vertex)
				Interpolate(uv)
				Interpolate(color)
				Interpolate(normal)
 
				return vert(v);
			}

			void InitNormalMap(inout Varyings i)
			{								
				i.normal.xy = tex2D(_NormalMap, i.uv * _Tilling).wy * 2 - 1;
				i.normal.xy *= _NormalStrenght;
				i.normal.z = sqrt(1 - saturate(dot(i.normal.xy, i.normal.xy)));
				i.normal = i.normal.xzy;
				i.normal = normalize(i.normal);
				
			}
 
			// The fragment shader definition.            
			half4 frag(Varyings IN) : SV_Target
			{	

				float4 a = lerp(_MidColor,_TopColor, clamp((IN.color.y - _GradientCenter) / _Spread, 0, 1)); //* step(IN.uv.y, IN.vertex.y - _Spread);
				float4 b = lerp(_BotColor, a, clamp((IN.color.y - _GradientCenter2) / _Spread, 0, 1)); // * step(_Spread, IN.vertex.y - IN.uv.y);

				float4 Noise = tex2D(_SnowTex, IN.uv * _Tilling);

				InitNormalMap(IN);				
				
				return (Noise * IN.normal.g) * b; // * float4(IN.normal, 0); //* tex2D(_RenderTexture, IN.uv);
			}
			ENDHLSL
		}
	}
}
