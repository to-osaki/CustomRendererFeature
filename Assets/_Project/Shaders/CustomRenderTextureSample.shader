Shader "Custom/CustomRenderTexture/Sample"
{
	Properties
	{
	}

	HLSLINCLUDE
	#include "UnityCustomRenderTexture.cginc"

	float4 frag(v2f_customrendertexture i) : SV_Target
	{
		float width = _CustomRenderTextureWidth;
		float height = _CustomRenderTextureHeight;
		float4 color = tex2D(_SelfTexture2D, i.globalTexcoord); // _SelfTexture2D needs double buffering 
		return color;
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