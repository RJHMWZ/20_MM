
#version 100

uniform 	vec4 _Time;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec4 _MainTex_ST;
uniform 	vec4 _Flipbook;
uniform 	float _Swattch;
uniform 	float _Frame;
uniform 	vec4 _uv_speed;
attribute highp vec4 in_POSITION0;
attribute highp vec4 in_COLOR0;
attribute highp vec2 in_TEXCOORD0;
varying mediump vec4 vs_COLOR0;
varying highp vec2 vs_TEXCOORD0;
vec4 u_xlat0;
vec4 u_xlat1;
float u_xlat2;
vec2 u_xlat4;
bool u_xlatb4;
float unity_roundEven(float x) { float y = floor(x + 0.5); return (y - x == 0.5) ? floor(0.5*y) * 2.0 : y; }
vec2 unity_roundEven(vec2 a) { a.x = unity_roundEven(a.x); a.y = unity_roundEven(a.y); return a; }
vec3 unity_roundEven(vec3 a) { a.x = unity_roundEven(a.x); a.y = unity_roundEven(a.y); a.z = unity_roundEven(a.z); return a; }
vec4 unity_roundEven(vec4 a) { a.x = unity_roundEven(a.x); a.y = unity_roundEven(a.y); a.z = unity_roundEven(a.z); a.w = unity_roundEven(a.w); return a; }

float trunc(float x) { return sign(x)*floor(abs(x)); }
vec2 trunc(vec2 x) { return sign(x)*floor(abs(x)); }
vec3 trunc(vec3 x) { return sign(x)*floor(abs(x)); }
vec4 trunc(vec4 x) { return sign(x)*floor(abs(x)); }

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
    u_xlat0.x = unity_roundEven(u_xlat0.x);
    u_xlatb4 = u_xlat0.x<0.0;
    u_xlat2 = u_xlatb4 ? u_xlat2 : float(0.0);
    u_xlat0.x = u_xlat2 + u_xlat0.x;
    u_xlat2 = u_xlat0.x / _Flipbook.x;
    u_xlatb4 = u_xlat2>=(-u_xlat2);
    u_xlat2 = fract(abs(u_xlat2));
    u_xlat2 = (u_xlatb4) ? u_xlat2 : (-u_xlat2);
    u_xlat2 = u_xlat2 * _Flipbook.x;
    u_xlat0.y = unity_roundEven(u_xlat2);
    u_xlat0.x = (-u_xlat0.y) + u_xlat0.x;
    u_xlat0.x = u_xlat0.x / _Flipbook.x;
    u_xlat0.x = u_xlat0.x / _Flipbook.y;
    u_xlatb4 = u_xlat0.x>=(-u_xlat0.x);
    u_xlat0.x = fract(abs(u_xlat0.x));
    u_xlat0.x = (u_xlatb4) ? u_xlat0.x : (-u_xlat0.x);
    u_xlat0.x = u_xlat0.x * _Flipbook.y;
    u_xlat0.x = unity_roundEven(u_xlat0.x);
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

