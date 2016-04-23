Shader "Hidden/Fluvio/FluidEffectComposite" {
Properties {
	_MainTex ("Base (RGBA)", 2D) = "white" {}
}
SubShader{

	Pass{
		 ZTest Always
		 Cull Off
		 ZWrite Off
		 Fog { Mode Off }
		 Tags {"Queue" = "Overlay" }
CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile _DEPTHTEX _DEPTHNORMALSTEX
	#include "UnityCG.cginc"

	#if _DEPTHNORMALSTEX
	#define DEPTHTEXNAME _CameraDepthNormalsTexture
	#else
	#define DEPTHTEXNAME _CameraDepthTexture
	#endif
	sampler2D _MainTex;
    half4 _MainTex_TexelSize;	
	sampler2D _BGCameraTex;
	half4 _BGCameraTex_TexelSize;
	sampler2D _FluidDepthTex;
	half4 _FluidDepthTex_TexelSize;
	#if _DEPTHNORMALSTEX
	sampler2D _CameraDepthNormalsTexture;
	half4 _CameraDepthNormalsTexture_TexelSize;
	#else
	sampler2D _CameraDepthTexture;
	half4 _CameraDepthTexture_TexelSize;
	#endif

	float _FluidThreshold;
	float _FluidSpecular;
	float _FluidSpecularScale;
	float _FluidOpacity;
	float4 _FluidTint;
				
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
		#if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0)
            o.uv.y = 1-o.uv.y;
        #endif
		return o;
	}
	
	
	float4 frag (v2f i) : COLOR
	{
		// Get depth texture information
		float fluidDepth;
		float3 fluidNormal;

		float sceneDepth;

		float4 sceneDepthTex = tex2D(DEPTHTEXNAME, i.uv);

		#if _DEPTHNORMALSTEX
		float3 sceneNormal;
		DecodeDepthNormal(sceneDepthTex, sceneDepth, sceneNormal);
		#else
		sceneDepth = Linear01Depth(sceneDepthTex.r);
		#endif
		DecodeDepthNormal(tex2D(_FluidDepthTex, i.uv), fluidDepth, fluidNormal);
		
		clip(sceneDepth - fluidDepth);

		// Get input color
		float4 color = tex2D(_MainTex, i.uv) * float4(_FluidTint.rgb,1);
		
		// Composite clip
		clip(color.a - _FluidThreshold);
		float val = color.a;
		color.a = 1;
		
		// Refraction
		float4 screen = tex2D(_BGCameraTex, i.uv); // +fluidNormal.xy); // TODO: Refraction normal offset

		// Fake Specular
		float4 spec = pow(color, _FluidSpecular/max(val, 0.01)) * _FluidSpecularScale * _FluidTint.a;

		// Final color
		return lerp(color, screen, _FluidOpacity) + spec;
	}
	
ENDCG

	}
}
Fallback Off
}
