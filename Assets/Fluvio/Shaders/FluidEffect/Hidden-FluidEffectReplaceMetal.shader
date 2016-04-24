Shader "Hidden/Fluvio/FluidEffectReplace"
{
    Properties
    {
        // Diffuse/alpha
        _Color ("Color", Color) = (0.75,0.75,0.75,0.5)
        _MainTex ("Albedo", 2D) = "white" {}
		_Cutoff ("Alpha Cutoff", Range(0.001, 1.0)) = 0.5
		_Cutoff2 ("Shadow/Depth Alpha Cutoff", Range(0.001, 1.0)) = 0.05

        // Smoothness/metallic
        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic ("Metallic", Range(0.0, 1.0)) = 0.8
        _MetallicGlossMap ("Metallic", 2D) = "white" {}
        
        // Normal
        _BumpScale ("Scale", Float) = 0.5
        _BumpMap ("Normal Map", 2D) = "bump" {}
        
        // Emission
        _EmissionColor ("Color", Color) = (0,0,0,0)
        _EmissionMap ("Emission", 2D) = "white" {}

        // Vertex colors
        [KeywordEnum(None, Albedo, Specular, Emission)] _VertexColorMode ("Vertex color mode", Float) = 1
        
		// Culling
		[HideInInspector] _CullFluid ("Cull Fluid", Float) = -1.0
    }

	CGINCLUDE
        #define _GLOSSYENV 1
        #define FLUVIO_SETUP_BRDF_INPUT FluvioMetallicSetup
    ENDCG
  
    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Fluvio" "PerformanceChecks"="False" }
        
        // ------------------------------------------------------------------
        // Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD" 
            Tags { "LightMode" = "ForwardBase" }

            Blend SrcAlpha OneMinusSrcAlpha, One One
            ZWrite Off ZTest LEqual Cull Off
            
            CGPROGRAM
            #pragma target 3.0
            // GLES2.0 disabled to prevent errors spam on devices without textureCubeLodEXT
            #pragma exclude_renderers gles
            
            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _EMISSION
            // ALWAYS ON shader_feature _GLOSSYENV
            #pragma shader_feature _METALLICGLOSSMAP 
            
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
                
            #pragma vertex FluvioVertForwardBase
            #pragma fragment FluvioFragForwardBase

            #include "FluidEffect.cginc"

            ENDCG
        }
		// ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend SrcAlpha One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off ZTest LEqual Cull Off
           
            CGPROGRAM
            #pragma target 3.0
            // GLES2.0 disabled to prevent errors spam on devices without textureCubeLodEXT
            #pragma exclude_renderers gles
            
            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

            #pragma shader_feature _NORMALMAP
            // ALWAYS ON shader_feature _GLOSSYENV
            #pragma shader_feature _METALLICGLOSSMAP
            
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            
            #pragma vertex FluvioVertForwardAdd
            #pragma fragment FluvioFragForwardAdd

            #include "FluidEffect.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        //Pass {
        //    Name "ShadowCaster"
        //    Tags { "LightMode" = "ShadowCaster" }
        //    
        //    ZWrite On ZTest LEqual
        //    
        //    CGPROGRAM
        //    #pragma target 3.0
        //    // TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
        //    #pragma exclude_renderers gles
        //    
        //    // -------------------------------------

        //    #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

        //    #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON
        //    #pragma multi_compile_shadowcaster

        //    #pragma vertex vertShadowCaster
        //    #pragma fragment fragShadowCaster

        //    #include "FluidEffectShadow.cginc"

        //    ENDCG
        //}
		// ------------------------------------------------------------------
        //  TODO: Deferred pass
        // Pass
        // {
        //     Name "DEFERRED"
        //     Tags { "LightMode" = "Deferred" }
		// 
        //     ZWrite On ZTest LEqual Cull Off
        //     
        //     CGPROGRAM
		// 	#pragma target 3.0
		// 	// GLES2.0 disabled to prevent errors spam on devices without textureCubeLodEXT
		// 	#pragma exclude_renderers nomrt gles
        //     
        //  #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION
		// 
		// 	#pragma shader_feature _NORMALMAP
		// 	#pragma shader_feature _EMISSION
		// 	// ALWAYS ON shader_feature _GLOSSYENV
		// 	#pragma shader_feature _METALLICGLOSSMAP
        //     
		// 	#pragma multi_compile ___ UNITY_HDR_ON
		// 	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
		// 	#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
		// 	#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        //     
		// 	#pragma vertex FluvioVertDeferred
		// 	#pragma fragment FluvioFragDeferred
		// 
		// 	#include "FluidEffect.cginc"
        //     ENDCG
        // }
    }
}