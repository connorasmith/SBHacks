Shader "Hidden/Fluvio/FluidEffectPreComposite" {
Properties {
	_MainTex ("Base (RGBA)", 2D) = "white" {}
}
SubShader {
	
	Pass{
		 ZTest Always
		 Cull Off
		 ZWrite Off
		 Fog { Mode Off }
		 Tags {"Queue" = "Overlay" }
CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
    half4 _MainTex_TexelSize;
	float _FluidThreshold;
				
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
		// Get input color
		float4 color = tex2D(_MainTex, i.uv);
		
		// Composite clip
		clip(color.a - _FluidThreshold);
		float val = color.a;
		color.a = 1;		

		// Final color
		return color;
	}
	
ENDCG

	}
}
Fallback Off
}
