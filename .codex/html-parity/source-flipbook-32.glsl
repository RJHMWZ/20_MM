
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 0
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 0
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 _Time;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec4 _MainTex_ST;
uniform 	vec4 _Flipbook;
uniform 	float _Swattch;
uniform 	float _Frame;
uniform 	vec4 _uv_speed;
in highp vec4 in_POSITION0;
in highp vec4 in_COLOR0;
in highp vec2 in_TEXCOORD0;
out mediump vec4 vs_COLOR0;
out highp vec2 vs_TEXCOORD0;
vec4 u_xlat0;
vec4 u_xlat1;
float u_xlat2;
vec2 u_xlat4;
bool u_xlatb4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    u_xlat0.x = _Frame + -1.0;
    u_xlat0.x = u_xlat0.x + (-_Time.y);
    u_xlat0.x = _Swattch * u_xlat0.x + _Time.y;
    u_xlat0.x = u_xlat0.x * _Flipbook.z + _Flipbook.w;
    u_xlat0.x = u_xlat0.x + -1.0;
    u_xlat2 = _Flipbook.y * _Flipbook.x;
    u_xlat0.x = u_xlat0.x / u_xlat2;
    u_xlatb4 = u_xlat0.x>=(-u_xlat0.x);
    u_xlat0.x = fract(abs(u_xlat0.x));
    u_xlat0.x = (u_xlatb4) ? u_xlat0.x : (-u_xlat0.x);
    u_xlat0.x = u_xlat2 * u_xlat0.x;
    u_xlat0.x = roundEven(u_xlat0.x);
    u_xlatb4 = u_xlat0.x<0.0;
    u_xlat2 = u_xlatb4 ? u_xlat2 : float(0.0);
    u_xlat0.x = u_xlat2 + u_xlat0.x;
    u_xlat2 = u_xlat0.x / _Flipbook.x;
    u_xlatb4 = u_xlat2>=(-u_xlat2);
    u_xlat2 = fract(abs(u_xlat2));
    u_xlat2 = (u_xlatb4) ? u_xlat2 : (-u_xlat2);
    u_xlat2 = u_xlat2 * _Flipbook.x;
    u_xlat0.y = roundEven(u_xlat2);
    u_xlat0.x = (-u_xlat0.y) + u_xlat0.x;
    u_xlat0.x = u_xlat0.x / _Flipbook.x;
    u_xlat0.x = u_xlat0.x / _Flipbook.y;
    u_xlatb4 = u_xlat0.x>=(-u_xlat0.x);
    u_xlat0.x = fract(abs(u_xlat0.x));
    u_xlat0.x = (u_xlatb4) ? u_xlat0.x : (-u_xlat0.x);
    u_xlat0.x = u_xlat0.x * _Flipbook.y;
    u_xlat0.x = roundEven(u_xlat0.x);
    u_xlat4.x = _Flipbook.y + -1.0;
    u_xlat4.x = trunc(u_xlat4.x);
    u_xlat0.x = (-u_xlat0.x) + u_xlat4.x;
    u_xlat4.xy = vec2(1.0, 1.0) / _Flipbook.xy;
    u_xlat1.xy = u_xlat4.xy * u_xlat0.yx;
    u_xlat0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    u_xlat0.xy = _Time.yy * _uv_speed.xy + u_xlat0.xy;
    vs_TEXCOORD0.xy = u_xlat0.xy * u_xlat4.xy + u_xlat1.xy;
    return;
}

