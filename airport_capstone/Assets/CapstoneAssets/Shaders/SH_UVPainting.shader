Shader "Custom/SH_UVPainting"
{
    Properties{
        _PainterColor ("Painter Color", Color) = (0, 0, 0, 0)
    }

    SubShader{
        Cull Off ZWrite Off ZTest Off

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _MaskTex;
            
            float3 _PainterPosition;
            float _Width;
            float _Height;
            float _Rotation;
            float _Strength; // Can be seen as the transparency level modifier for the paint - color lerp value between 0 and 1 * _Strength
            float4 _PainterColor;
            float _PrepareUV;
            float2 _TexResolution;
            fixed4 _PixelColor;

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float4 color : COLOR;
            };

            
            float rectangleMask( float rotation, float2 pos, float2 center, float width, float height)
            { 
                float cosY = cos(rotation);
                float sinY = sin(rotation);
                float2x2 rot = float2x2(cosY, -sinY, sinY, cosY);

                float2 posNew = pos.xy;// *2 - 1;
                float2 centerNew = center.xy;// *2 - 1;
                posNew.xy = mul(rot, posNew.xy);
                centerNew.xy = mul(rot, centerNew.xy);

                // pas certain de ce qui est fait ici.  bricolage pour ajuster.  et height est-il toujours correct après rot?
                float2 d = abs(posNew - float2(centerNew.x - width, centerNew.y - height/2)) - float2(width, height);
                d = 1 - d /fwidth(d);
                

                float Out = saturate(min(d.x, d.y));                
                
                return Out;
            }

            v2f vert (appdata v)
            {
                v2f o;

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);                
                o.uv = v.uv;                
				float4 uv = float4(0, 0, 0, 1);

                o.color = v.color;

                uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * float2( 2, 2) - float2(1, 1));  
                o.vertex = uv; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                if(_PrepareUV > 0 ){
                    return float4(0, 0, 1, 1);
                } 

                float4 col = tex2D(_MainTex, i.uv);
                float f = rectangleMask(_Rotation, i.worldPos.xz, _PainterPosition.xz, _Width, _Height);
                float edge = f * _Strength;
                
                return lerp(col, _PainterColor, edge);
            }
            ENDCG
        }
    }
}
