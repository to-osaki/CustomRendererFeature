Shader "Custom/CustomRenderTexture/Water"
{
	Properties
	{
		_constant("_constant", float) = 0.2
		_attenuation("_attenuation", float) = 0.99
	}

	HLSLINCLUDE
	#include "UnityCustomRenderTexture.cginc"

	float _constant;
	float _attenuation;

	float4 frag(v2f_customrendertexture i) : SV_Target
	{
		float du = 1.0 / _CustomRenderTextureWidth;
		float dv = 1.0 / _CustomRenderTextureHeight;
		float2 uvu = float2(du, 0);
		float2 uvd = float2(0, dv);
		float2 uv = i.globalTexcoord;
		float4 self = tex2D(_SelfTexture2D, uv); // need double buffering 

		float4 v1 = tex2D(_SelfTexture2D, uv - uvu);
		float4 v2 = tex2D(_SelfTexture2D, uv + uvu);
		float4 v3 = tex2D(_SelfTexture2D, uv - uvd);
		float4 v4 = tex2D(_SelfTexture2D, uv + uvd);

		float v = (2 * self.r - self.g + _constant * (v1.r + v2.r + v3.r + v4.r - 4 * self.r)) * _attenuation;
		return float4(v, self.r, 0, 1);
	}

		float4 frag_color(v2f_customrendertexture i) : SV_Target
	{
		return float4(1, 1, 1, 1);
	}
		float4 frag_color_r(v2f_customrendertexture i) : SV_Target
	{
		return float4(1, 0, 0, 1);
	}
		float4 frag_color_g(v2f_customrendertexture i) : SV_Target
	{
		return float4(0, 1, 0, 1);
	}
		float4 frag_color_b(v2f_customrendertexture i) : SV_Target
	{
		return float4(0, 0, 1, 1);
	}
		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "Update"
			HLSLPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag
			ENDHLSL
		}

			Pass
		{
			Name "White"
			HLSLPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag_color
			ENDHLSL
		}
			Pass
		{
			Name "Red"
			HLSLPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag_color_r
			ENDHLSL
		}
			Pass
		{
			Name "Green"
			HLSLPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag_color_g
			ENDHLSL
		}
			Pass
		{
			Name "Blue"
			HLSLPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag_color_b
			ENDHLSL
		}
	}
}