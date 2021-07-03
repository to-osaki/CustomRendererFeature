Shader "Custom/Image Effect/UVbyTexture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" { }
		_UVTex("Texture", 2D) = "white" { }
		_UVOriginalRate("_UVOriginalRate", float) = 1
		_UVTexRate("_UVTexRate", float) = 1
		_UVValueOffset("_UVValueOffset", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _UVTex;
			float _UVOriginalRate;
			float _UVTexRate;
			float2 _UVValueOffset;

			fixed4 frag(v2f_img i) : SV_Target
			{
				float2 uv = tex2D(_UVTex, i.uv) + _UVValueOffset;
				fixed4 color = tex2D(_MainTex, i.uv * _UVOriginalRate + uv * _UVTexRate);
				return color;
			}
			ENDHLSL
		}
	}

		Fallback off
}