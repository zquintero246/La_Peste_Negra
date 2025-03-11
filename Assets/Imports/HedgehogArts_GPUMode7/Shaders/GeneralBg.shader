// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FuzzyLogic Studios/General_Bg"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Background" "PreviewType" = "Skybox" }
		Pass
		{
			Cull Off ZWrite Off ZTest Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			half _Cutoff;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv + float2(1,0.5);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				return col;
			}
			ENDCG
		}
	}
}
