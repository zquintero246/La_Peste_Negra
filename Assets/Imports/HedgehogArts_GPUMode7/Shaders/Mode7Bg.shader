// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
* Hedgehog Arts
* Mode7 Scaling Engine
*
* This shader file is specifically
* made to faithfully recreate the
* mode7 scaling that was present on
* the SNES and GBA.
*
* The major limitations of this algorithm
* when paired with Unity:
* 1) there is no way to adjust pitch and not
* 	 break the sprite projection (aka. anything
* 	 other than 0 will cause a "swimming" artifact
*	 on the sprites)
* 2) Specific parameters are needed to match
* 	 up the dummy 10x10 plane with the
* 	 actual shader result.
* 3) due to the above limit, at the moment
* 	 the size of the area rendered in mode7
* 	 is approx. 512x512 (255 to -255 on both X and Y) from
*	 Unity's world origin.
* 4) Actual projection doesn't look straight down
* 	 if camera is pointing straight down.
* 
* In future, a tiling trick could be used to
* maximize the area to be bigger than 500x500
* Games like F-ZERO did this to make the tracks
* nice and big, and still have fair amounts of
* detail.
*/



Shader "Hedgehog Arts/Mode7_Bg"
{
	Properties
	{
		_Mode7Map ("Texture", 2D) = "black" {}
		_Backdrop("Backdrop", 2D) = "black" {}
		_fov ("Field of View", Range(0, 1000)) = 200
		_rot ("rotation (Y-axis)", float) = 0
		_scaling ("scale/distance From ground", Range(0.005, 75)) = 2
		_offset ("X/Y pos", Vector) = (1,1,1,1)
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
		_Order ("Rendering Order", int) = 1
		_AtmoScatter ("Background?", int) = 0
		_Fog ("Fog?", int) = 0
		_FogColor ("Fog Color", Color) = (1,1,1,1)
		_usingPanorama ("Panorama? (Squishes image)", float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Geometry" "PreviewType" = "Plane" }

		Pass
		{
			ZTest Always Cull Off ZWrite Off

			//this is used to change draw order. 
			//(In other words; doesn't work on 
			//platforms that don't support stencil
			//buffers!)

			Stencil {
				Ref [_Order]
				Comp GEqual
				Pass Replace
				Fail Keep
				ZFail Keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 ld : TEXCOORD1;
				float3 normal : TEXCOORD2;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeScreenPos(o.vertex);
				
				o.ld = WorldSpaceViewDir(v.vertex);
				o.normal = v.normal;

				return o;
			}
			
			sampler2D _Mode7Map;
			sampler2D _Backdrop;
			fixed _fov;
			fixed _Fog;
			fixed4 _FogColor;
			uniform fixed _horizon;
			uniform fixed _floorHeight;
			half _rot;
			half _scaling;
			half4 _offset;
			fixed _usingPanorama;
			
			//these are set by scripts.
			fixed _sine;
			fixed _cosine;

			half _Cutoff;
			int _AtmoScatter;
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed2 uv = i.uv.xy / i.uv.w;
				fixed horz = _horizon;
				
				fixed px = (uv.x * 2 - 1) * (_ScreenParams.x / _ScreenParams.y) * 2;
				fixed py = _fov;
				fixed pz = (uv.y * 2 - 1) - (horz*1.5);

				//projection
				fixed sx = px / pz;
				fixed sy = py / pz;
				
				fixed ax, ay;
				ax = sx;
				ay = sy;
				
				sx = ax * _cosine - ay * _sine;
				sy = ax * _sine + ay * _cosine;

				fixed ssx = sx; //store this for sun
				
				sx *= _scaling * (_floorHeight);
				sy *= _scaling * (_floorHeight);

				sx += _offset.x/256;
				sy += _offset.y/256;
				
				fixed2 coords = half2(sx, sy);

				fixed2 bgOffset = float2((_rot / 180)*0.5, (-horz));
				fixed2 newUV = uv * float2((_ScreenParams.x/_ScreenParams.y)*0.5, 1) + bgOffset;
				if (_usingPanorama > 0) {
					newUV.y *= 2;
					newUV.y -= 1;
				}
				newUV.y = clamp(newUV.y, 0.0001, 0.99);
				
				fixed4 atmoScatter = _AtmoScatter > 0 ? tex2D(_Backdrop, newUV) : half4(0,0,0,0);

				//sample texture
				fixed4 map = tex2D(_Mode7Map, coords*0.5 + 0.5);

				fixed4 col = fixed4(1,1,1,1);

				if (_floorHeight >= 0)
					col = pz > _horizon-_horizon || map.a < _Cutoff ? atmoScatter : map;
				else
					col = pz < _horizon-_horizon || map.a < _Cutoff ? atmoScatter : map;

				clip(col.a - _Cutoff);

				//apply fog if chosen.
				fixed pzF = pz < _horizon-_horizon ? pz * 0.5 + 0.5 : -pz * 0.5 + 0.5;
				col = _Fog > 0 ? lerp(col, _FogColor, (pzF+_FogColor.a)*0.5) : col;

				//return pixel value
				return col;
			}
			ENDCG
		}
	}
}
