Shader "Unlit/MenuBG"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
	}
		SubShader
		{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
				float bright = 0.15f;
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color + fixed4(bright, bright, bright, 0);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				float contrast = 0.5f * sin((i.uv.x + i.uv.y) * 6.28 - _Time.y * 2) + 0.5f;
				float mid = 0.7f;
				if (Luminance(col.rgb) > mid)
				{
					col *= 1 + contrast;
				}

				float x_cutoff = 0.07f;
				float y_cutoff = 0.04f;
				if (i.uv.x % (1 - x_cutoff) < x_cutoff || i.uv.y % (1 - y_cutoff) < y_cutoff)
				{
					float noise = Luminance(tex2D(_MainTex, i.uv * 5).xyz);
					if (noise < mid) noise /= 2;
					else if (noise >= mid) noise /= 1.3f;
					col.a = noise;
				}
				else
				{
					col.a = 0.1f * contrast + 0.7f;
				}
				return col;
				}

				ENDCG
			}
		}
}
