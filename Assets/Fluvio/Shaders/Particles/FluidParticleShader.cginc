#include "UnityCG.cginc"
uniform sampler2D _MainTex;uniform float4 _MainTex_ST;uniform half _InvFade;
#ifdef SOFTPARTICLES_ON
sampler2D _CameraDepthTexture;
#endif
struct v2fB {float4 pos : SV_POSITION;half2 uv : TEXCOORD0;half4 uvScreen : TEXCOORD1;half4 color : COLOR0;};v2fB vertB (appdata_full v){v2fB o;o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);o.pos = mul (UNITY_MATRIX_MVP, v.vertex);o.uvScreen = ComputeGrabScreenPos (o.pos);
#ifdef SOFTPARTICLES_ON
COMPUTE_EYEDEPTH(o.uvScreen.z);
#endif
o.color = v.color;return o;}