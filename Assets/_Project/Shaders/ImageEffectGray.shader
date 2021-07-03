Shader "Custom/Image Effect/Gray"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" { }
		_GrayScale("_GrayScale", Vector) = (0.3, 0.6, 0.1, 1)
		_ColorWeight("_ColorWeight", float) = 0
		_GrayWeight("_GrayWeight", float) = 1
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
			float4 _GrayScale;
			float _ColorWeight;
			float _GrayWeight;
			
			fixed4 frag(v2f_img i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				float gray = dot(color.rgb, _GrayScale.rgb) * _GrayWeight;
				color = float4(gray, gray, gray, 0) + float4(color.rgb * _ColorWeight, color.a);
				return color;
			}
			ENDHLSL
		}
	}

		Fallback off
}