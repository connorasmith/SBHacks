// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#include "UnityStandardCore.cginc"
#ifdef UNITY_5_2_PLUS
#define FLUVIO_FRAGMENTGI(s, occlusion, i_ambientOrLightmapUV, atten, light) FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light)
#else 
#define FLUVIO_FRAGMENTGI(s, occlusion, i_ambientOrLightmapUV, atten, light) FragmentGI(s.posWorld, occlusion, i_ambientOrLightmapUV, atten, s.oneMinusRoughness, s.normalWorld, s.eyeVec, light)
#endif
#define FLUVIO_FRAGMENT_SETUP(x) FragmentCommonData x = FluvioFragmentSetup(i.tex, i.eyeVec, WorldNormal(i.tangentToWorldAndColor), ExtractTangentToWorldPerPixel(i.tangentToWorldAndColor), IN_WORLDPOS(i), vertex_color);
#define FLUVIO_FRAGMENT_SETUP_FWDADD(x) FragmentCommonData x = FluvioFragmentSetup(i.tex, i.eyeVec, WorldNormal(i.tangentToWorldAndLightDir), ExtractTangentToWorldPerPixel(i.tangentToWorldAndLightDir), half3(0,0,0), half3(0,0,0));
#ifndef FLUVIO_SETUP_BRDF_INPUT
#define FLUVIO_SETUP_BRDF_INPUT FluvioSpecularSetup
#endif
struct FluvioVertexInput{float4 vertex : POSITION;half3 normal : NORMAL;float4 color : COLOR;float2 uv0  : TEXCOORD0;
#ifdef _TANGENT_TO_WORLD
half4 tangent : TANGENT;
#endif
};float _Fluvio_DepthPassCount;float _Fluvio_DepthPassIndex;float _Fluvio_DepthPassCellSpace;
#define FLUVIO_DEPTHPASS_BUCKET_SIZE 1.0f
int _Fluvio_OverrideNormals = 1;float _CullFluid;float4x4 _Fluvio_FluidToObject;float3 FluvioGetWorldNormal(float3 norm){if (_Fluvio_OverrideNormals)norm = mul((float3x3)_Fluvio_FluidToObject, float3(0,0,-1));return UnityObjectToWorldNormal(norm);}float4 FluvioGetWorldTangent(float4 dir){if (_Fluvio_OverrideNormals)dir = float4(mul((float3x3)_Fluvio_FluidToObject, float3(1,0,0)), -1);return float4(UnityObjectToWorldDir(dir.xyz), dir.w);}half FluvioAlpha(float2 texcoords){return tex2D(_MainTex, texcoords).a;}half3 FluvioEmission(float2 uv, float3 color){
#ifndef _EMISSION
return 0;
#else 
return tex2D(_EmissionMap, uv).rgb * _EmissionColor.rgb * color;
#endif
}half3 FluvioAlbedo(float2 texcoords, float3 color){return _Color.rgb * tex2D (_MainTex, texcoords).rgb * color;}float2 FluvioTexCoords(FluvioVertexInput v){return TRANSFORM_TEX(v.uv0, _MainTex);}half4 FluvioOutputForward (half3 color, half alpha){return half4(color, alpha);}inline FragmentCommonData FluvioSpecularSetup (float2 i_tex, half3 vertex_color){
#ifdef _VERTEXCOLORMODE_ALBEDO
half3 albedo = FluvioAlbedo(i_tex, vertex_color);
#else 
half3 albedo = FluvioAlbedo(i_tex, float3(1,1,1));
#endif
half4 specGloss = SpecularGloss(i_tex);
#ifdef _VERTEXCOLORMODE_SPECULAR
half3 specColor = specGloss.rgb * vertex_color;
#else 
half3 specColor = specGloss.rgb;
#endif
half oneMinusRoughness = specGloss.a;half oneMinusReflectivity;half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (albedo, specColor,  oneMinusReflectivity);FragmentCommonData o = (FragmentCommonData)0;o.diffColor = diffColor;o.specColor = specColor;o.oneMinusReflectivity = oneMinusReflectivity;o.oneMinusRoughness = oneMinusRoughness;return o;}inline FragmentCommonData FluvioMetallicSetup(float2 i_tex, half3 vertex_color){
#ifdef _VERTEXCOLORMODE_ALBEDO
half3 albedo = FluvioAlbedo(i_tex, vertex_color);
#else 
half3 albedo = FluvioAlbedo(i_tex, float3(1, 1, 1));
#endif
half2 metallicGloss = MetallicGloss(i_tex);half metallic = metallicGloss.x;half oneMinusRoughness = metallicGloss.y;half oneMinusReflectivity;half3 specColor;half3 diffColor = DiffuseAndSpecularFromMetallic(albedo, metallic,  specColor,  oneMinusReflectivity);
#ifdef _VERTEXCOLORMODE_SPECULAR
specColor *= vertex_color;
#endif
FragmentCommonData o = (FragmentCommonData)0;o.diffColor = diffColor;o.specColor = specColor;o.oneMinusReflectivity = oneMinusReflectivity;o.oneMinusRoughness = oneMinusRoughness;return o;}
#ifdef _NORMALMAP
half3 FluvioNormalInTangentSpace(float2 texcoords){return UnpackScaleNormal(tex2D (_BumpMap, texcoords), _BumpScale);}
#endif
inline FragmentCommonData FluvioFragmentSetup (float2 i_tex, half3 i_eyeVec, half3 i_normalWorld, half3x3 i_tanToWorld, half3 i_posWorld, half3 vertex_color){half alpha = FluvioAlpha(i_tex);
#ifdef _NORMALMAP
half3 normalWorld = NormalizePerPixelNormal(mul(FluvioNormalInTangentSpace(i_tex), i_tanToWorld));
#else 
half3 normalWorld = i_normalWorld;
#endif
half3 eyeVec = i_eyeVec;eyeVec = NormalizePerPixelNormal(eyeVec);FragmentCommonData o = FLUVIO_SETUP_BRDF_INPUT(i_tex, vertex_color);o.normalWorld = normalWorld;o.eyeVec = eyeVec;o.posWorld = i_posWorld;o.alpha = alpha;return o;}inline float invlerp(float from, float to, float value){return (value - from) / (to - from);}struct FluvioVertexOutputForwardBase{float4 pos       : SV_POSITION;float2 tex       : TEXCOORD0;half3 eyeVec       : TEXCOORD1;half4 tangentToWorldAndColor[3]  : TEXCOORD2;half4 ambientOrLightmapUV   : TEXCOORD5;SHADOW_COORDS(6)UNITY_FOG_COORDS(7)
#if UNITY_SPECCUBE_BOX_PROJECTION
float3 posWorld     : TEXCOORD8;
#endif
};FluvioVertexOutputForwardBase FluvioVertForwardBase (FluvioVertexInput v){FluvioVertexOutputForwardBase o;UNITY_INITIALIZE_OUTPUT(FluvioVertexOutputForwardBase, o);if (_CullFluid < 0){o.pos = 0;}else {float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
#if UNITY_SPECCUBE_BOX_PROJECTION
o.posWorld = posWorld.xyz;
#endif
float3 objPosWorld = mul(unity_ObjectToWorld, float4(0,0,0,1));float3 dir = normalize(objPosWorld - _WorldSpaceCameraPos) * _Fluvio_DepthPassCellSpace;float3 nearPoint = objPosWorld + dir;float3 farPoint = objPosWorld - dir;float nearDepth = -mul(UNITY_MATRIX_VP, float4(nearPoint, 1)).z;float farDepth = -mul(UNITY_MATRIX_VP, float4(farPoint,1)).z;float depth = abs(invlerp(nearDepth, farDepth, -mul(UNITY_MATRIX_MVP, v.vertex).z)) * _Fluvio_DepthPassCellSpace;if (((depth / _Fluvio_DepthPassCellSpace % _Fluvio_DepthPassCount) - _Fluvio_DepthPassIndex) > FLUVIO_DEPTHPASS_BUCKET_SIZE){o.pos = 0;}else {o.pos = mul(UNITY_MATRIX_MVP, v.vertex);o.tex = FluvioTexCoords(v);o.eyeVec = NormalizePerVertexNormal(posWorld - _WorldSpaceCameraPos);float3 normalWorld = FluvioGetWorldNormal(v.normal);
#ifdef _TANGENT_TO_WORLD
float4 tangentWorld = FluvioGetWorldTangent(v.tangent);float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);o.tangentToWorldAndColor[0].xyz = tangentToWorld[0];o.tangentToWorldAndColor[1].xyz = tangentToWorld[1];o.tangentToWorldAndColor[2].xyz = tangentToWorld[2];
#else 
o.tangentToWorldAndColor[0].xyz = 0;o.tangentToWorldAndColor[1].xyz = 0;o.tangentToWorldAndColor[2].xyz = normalWorld;
#endif
#ifdef _VERTEXCOLORMODE_NONE
o.tangentToWorldAndColor[0].w = 1;o.tangentToWorldAndColor[1].w = 1;o.tangentToWorldAndColor[2].w = 1;
#else 
o.tangentToWorldAndColor[0].w = v.color.r;o.tangentToWorldAndColor[1].w = v.color.g;o.tangentToWorldAndColor[2].w = v.color.b;
#endif
#if UNITY_SHOULD_SAMPLE_SH
#if UNITY_SAMPLE_FULL_SH_PER_PIXEL
o.ambientOrLightmapUV.rgb = 0;
#elif (SHADER_TARGET < 30)
o.ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
#else 
o.ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
#endif
#ifdef VERTEXLIGHT_ON
o.ambientOrLightmapUV.rgb += Shade4PointLights (unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,unity_4LightAtten0, posWorld, normalWorld);
#endif
#endif
TRANSFER_SHADOW(o);UNITY_TRANSFER_FOG(o,o.pos);}}return o;}half4 FluvioFragForwardBase (FluvioVertexOutputForwardBase i) : SV_Target{half3 vertex_color = half3(i.tangentToWorldAndColor[0].w, i.tangentToWorldAndColor[1].w, i.tangentToWorldAndColor[2].w);FLUVIO_FRAGMENT_SETUP(s)UnityLight mainLight = MainLight (s.normalWorld);half atten = SHADOW_ATTENUATION(i);half occlusion = 1;UnityGI gi = FLUVIO_FRAGMENTGI(s, occlusion, i.ambientOrLightmapUV, atten, mainLight);half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);c.rgb += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);
#ifdef _VERTEXCOLORMODE_EMISSION
c.rgb += FluvioEmission(i.tex, vertex_color);
#else 
c.rgb += FluvioEmission(i.tex, float3(1,1,1));
#endif
UNITY_APPLY_FOG(i.fogCoord, c.rgb);return FluvioOutputForward (c, s.alpha);}struct FluvioVertexOutputForwardAdd{float4 pos       : SV_POSITION;float2 tex       : TEXCOORD0;half3 eyeVec       : TEXCOORD1;half4 tangentToWorldAndLightDir[3] : TEXCOORD2;LIGHTING_COORDS(5,6)UNITY_FOG_COORDS(7)};FluvioVertexOutputForwardAdd FluvioVertForwardAdd (FluvioVertexInput v){FluvioVertexOutputForwardAdd o;UNITY_INITIALIZE_OUTPUT(FluvioVertexOutputForwardAdd, o);if (_CullFluid < 0){o.pos = 0;}else {float4 posWorld = mul(unity_ObjectToWorld, v.vertex);o.pos = mul(UNITY_MATRIX_MVP, v.vertex);o.tex = FluvioTexCoords(v);o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);float3 normalWorld = FluvioGetWorldNormal(v.normal);
#ifdef _TANGENT_TO_WORLD
float4 tangentWorld = FluvioGetWorldTangent(v.tangent);float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
#else 
o.tangentToWorldAndLightDir[0].xyz = 0;o.tangentToWorldAndLightDir[1].xyz = 0;o.tangentToWorldAndLightDir[2].xyz = normalWorld;
#endif
TRANSFER_VERTEX_TO_FRAGMENT(o);float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
#ifndef USING_DIRECTIONAL_LIGHT
lightDir = NormalizePerVertexNormal(lightDir);
#endif
o.tangentToWorldAndLightDir[0].w = lightDir.x;o.tangentToWorldAndLightDir[1].w = lightDir.y;o.tangentToWorldAndLightDir[2].w = lightDir.z;UNITY_TRANSFER_FOG(o,o.pos);}return o;}half4 FluvioFragForwardAdd (FluvioVertexOutputForwardAdd i) : SV_Target{FLUVIO_FRAGMENT_SETUP_FWDADD(s)UnityLight light = AdditiveLight (s.normalWorld, IN_LIGHTDIR_FWDADD(i), LIGHT_ATTENUATION(i));UnityIndirect noIndirect = ZeroIndirect ();half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, light, noIndirect);UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0));return FluvioOutputForward (c, s.alpha);}struct FluvioVertexOutputDeferred{float4 pos       : SV_POSITION;float2 tex       : TEXCOORD0;half3 eyeVec       : TEXCOORD1;half4 tangentToWorldAndColor[3]  : TEXCOORD2;half4 ambientOrLightmapUV   : TEXCOORD5;
#if UNITY_SPECCUBE_BOX_PROJECTION
float3 posWorld     : TEXCOORD6;
#endif
};FluvioVertexOutputDeferred FluvioVertDeferred (FluvioVertexInput v){FluvioVertexOutputDeferred o;UNITY_INITIALIZE_OUTPUT(FluvioVertexOutputDeferred, o);if (_CullFluid < 0){o.pos = 0;}else {float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
#if UNITY_SPECCUBE_BOX_PROJECTION
o.posWorld = posWorld;
#endif
o.pos = mul(UNITY_MATRIX_MVP, v.vertex);float3 objPosWorld = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));float3 dir = normalize(objPosWorld - _WorldSpaceCameraPos) * _Fluvio_DepthPassCellSpace;float3 nearPoint = objPosWorld + dir;float3 farPoint = objPosWorld - dir;float nearDepth = -mul(UNITY_MATRIX_VP, float4(nearPoint, 1)).z;float farDepth = -mul(UNITY_MATRIX_VP, float4(farPoint, 1)).z;float depth = abs(invlerp(nearDepth, farDepth, -mul(UNITY_MATRIX_MVP, v.vertex).z)) * _Fluvio_DepthPassCellSpace;if (((depth / _Fluvio_DepthPassCellSpace % _Fluvio_DepthPassCount) - _Fluvio_DepthPassIndex) > FLUVIO_DEPTHPASS_BUCKET_SIZE){o.pos = 0;}else {o.tex = FluvioTexCoords(v);o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);float3 normalWorld = FluvioGetWorldNormal(v.normal);
#ifdef _TANGENT_TO_WORLD
float4 tangentWorld = FluvioGetWorldTangent(v.tangent);float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);o.tangentToWorldAndColor[0].xyz = tangentToWorld[0];o.tangentToWorldAndColor[1].xyz = tangentToWorld[1];o.tangentToWorldAndColor[2].xyz = tangentToWorld[2];
#else 
o.tangentToWorldAndColor[0].xyz = 0;o.tangentToWorldAndColor[1].xyz = 0;o.tangentToWorldAndColor[2].xyz = normalWorld;
#endif
#ifdef _VERTEXCOLORMODE_NONE
o.tangentToWorldAndColor[0].w = 1;o.tangentToWorldAndColor[1].w = 1;o.tangentToWorldAndColor[2].w = 1;
#else 
o.tangentToWorldAndColor[0].w = v.color.r;o.tangentToWorldAndColor[1].w = v.color.g;o.tangentToWorldAndColor[2].w = v.color.b;
#endif
#if UNITY_SHOULD_SAMPLE_SH
#if (SHADER_TARGET < 30)
o.ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
#else 
o.ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
#endif
#endif
}}return o;}void FluvioFragDeferred (FluvioVertexOutputDeferred i,out half4 outDiffuse : SV_Target0,out half4 outSpecSmoothness : SV_Target1,out half4 outNormal : SV_Target2,out half4 outEmission : SV_Target3){
#if (SHADER_TARGET < 30)
outDiffuse = 1;outSpecSmoothness = 1;outNormal = 0;outEmission = 0;return;
#endif
half3 vertex_color = half3(i.tangentToWorldAndColor[0].w, i.tangentToWorldAndColor[1].w, i.tangentToWorldAndColor[2].w);FLUVIO_FRAGMENT_SETUP(s)half alpha = FluvioAlpha(i.tex);clip(alpha - _Cutoff);UnityLight dummyLight = DummyLight (s.normalWorld);half atten = 1;half occlusion = 1;UnityGI gi = FLUVIO_FRAGMENTGI(s, occlusion, i.ambientOrLightmapUV, atten, dummyLight);half3 color = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;color += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);
#ifdef _VERTEXCOLORMODE_EMISSION
color += FluvioEmission(i.tex, vertex_color);
#else 
color += FluvioEmission(i.tex, float3(1,1,1));
#endif
#ifndef UNITY_HDR_ON
color.rgb = exp2(-color.rgb);
#endif
outDiffuse = half4(s.diffColor * alpha, 1);outSpecSmoothness = half4(s.specColor * alpha, s.oneMinusRoughness);outNormal = half4(s.normalWorld,1);outEmission = half4(color, 1);}