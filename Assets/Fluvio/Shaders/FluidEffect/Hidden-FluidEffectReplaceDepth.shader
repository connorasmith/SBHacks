Shader "Hidden/Fluvio/FluidEffectReplaceDepth"
{
    Properties
    {
        // Diffuse/alpha
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0.001, 1.0)) = 0.5
		_Cutoff2("Shadow/Depth Alpha Cutoff", Range(0.001, 1.0)) = 0.25

        // Smoothness/metallic
        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap ("Metallic", 2D) = "white" {}
        
        // Normal
        _BumpScale("Scale", Float) = 1.0
        _BumpMap ("Normal Map", 2D) = "bump" {}
        
        // Emission
        _EmissionColor("Color", Color) = (0,0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        // Vertex colors
        [KeywordEnum(None, Albedo, Specular, Emission)] _VertexColorMode ("Vertex color mode", Float) = 1

		// Culling
		[HideInInspector] [PerRendererData]_CullFluid("Cull Fluid", Float) = -1.0
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Fluvio" "PerformanceChecks"="False" }
        
		Pass
		{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
			};
			uniform float4 _MainTex_ST;
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}
			uniform sampler2D _MainTex;
			uniform sampler2D _BumpMap;
			uniform float _BumpScale;
			uniform fixed _Cutoff2;
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - _Cutoff2);
				float3 normal = i.nz.xyz + (UnpackNormal(tex2D(_BumpMap, i.uv)) *_BumpScale);
				return EncodeDepthNormal(i.nz.w, normal);
			}
			ENDCG
		}
    }
}