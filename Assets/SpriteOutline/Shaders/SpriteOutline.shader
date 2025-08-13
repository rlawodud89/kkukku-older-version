Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Main texture Tint", Color) = (1,1,1,1)

        // 공통
        [MaterialToggle] _OutlineEnabled ("Outline Enabled", Float) = 1
        _SolidOutline ("Outline Color", Color) = (0,1,0,1)
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.2

        // 외곽선 두께(픽셀) → 이 값만큼 이웃을 샘플해 경계 판정
        _Thickness ("Outline Thickness (px)", Range(0,16)) = 2

        // 잘림 방지: 패스2에서 쿼드를 화면 픽셀 기준으로 확장
        _ExpandPixels ("Expand Quad (px)", Range(0,16)) = 2

        // 누출/안쪽선 제어
        [MaterialToggle] _IgnoreUVBorder ("Ignore UV Border", Float) = 1
        [MaterialToggle] _OutsideOnly   ("Outline Outside Only", Float) = 1
        _BorderMargin ("UV Border Margin (px)", Range(0,8)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        // -------- Pass 1: 원본 스프라이트 --------
        Pass
        {
            CGPROGRAM
            #pragma vertex vertBase
            #pragma fragment fragBase
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata { float4 vertex:POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; };
            struct v2f     { float4 pos:SV_POSITION; fixed4 col:COLOR; float2 uv:TEXCOORD0; };

            sampler2D _MainTex, _AlphaTex; float _AlphaSplitEnabled;
            float4 _MainTex_TexelSize;
            fixed4 _Color;

            v2f vertBase(appdata v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                o.col = v.color * _Color;
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }

            fixed4 SampleSprite(float2 uv){
                fixed4 c = tex2D(_MainTex, uv);
                #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if(_AlphaSplitEnabled) c.a = tex2D(_AlphaTex, uv).r;
                #endif
                return c;
            }

            fixed4 fragBase(v2f i):SV_Target{
                fixed4 c = SampleSprite(i.uv) * i.col;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }

        // -------- Pass 2: 확장 쿼드 + 외곽선 --------
        Pass
        {
            // sprite 위에 그릴 거라 ZTest Always
            ZTest Always
            CGPROGRAM
            #pragma vertex vertExpand
            #pragma fragment fragOutline
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata { float4 vertex:POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; };
            struct v2f     { float4 pos:SV_POSITION; fixed4 col:COLOR; float2 uv:TEXCOORD0; float2 uvFromCenter:TEXCOORD1; };

            sampler2D _MainTex, _AlphaTex; float _AlphaSplitEnabled;
            float4 _MainTex_TexelSize;
            fixed4 _Color;

            // props
            fixed  _OutlineEnabled;
            fixed4 _SolidOutline;
            half   _AlphaThreshold;
            half   _Thickness;
            half   _ExpandPixels;
            fixed  _IgnoreUVBorder;
            fixed  _OutsideOnly;
            half   _BorderMargin;

            // 0~1 범위 + 경계여유 체크
            inline bool In01(float2 uv){ return all(uv >= 0) && all(uv <= 1); }
            inline bool AwayFromBorder(float2 uv, float2 texel, half marginPx){
                float2 m = texel * max(marginPx, 0.0);
                return (uv.x > m.x) && (uv.x < 1.0 - m.x) && (uv.y > m.y) && (uv.y < 1.0 - m.y);
            }

            fixed4 SampleSprite(float2 uv){
                fixed4 c = tex2D(_MainTex, uv);
                #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if(_AlphaSplitEnabled) c.a = tex2D(_AlphaTex, uv).r;
                #endif
                return c;
            }

            // 화면 픽셀 기준으로 쿼드 확장
            v2f vertExpand(appdata v)
            {
                v2f o;
                float4 pos = UnityObjectToClipPos(v.vertex);

                // UV 기준 방향(코너는 대각, 엣지는 수평/수직)
                float2 dir = normalize(max(abs(v.uv - 0.5), 1e-5) * sign(v.uv - 0.5));

                // 픽셀 → NDC 보정: 2/_ScreenParams.xy
                float2 ndc = pos.xy / pos.w;
                ndc += dir * (_ExpandPixels * 2.0 / _ScreenParams.xy);
                pos.xy = ndc * pos.w;

                o.pos = pos;
                o.uv  = v.uv;
                o.col = v.color * _Color;
                o.uvFromCenter = v.uv - 0.5;
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }

            bool NeighborHasAlpha(float2 uv, float2 texel, half th, half marginPx)
            {
                const int N = 8;
                float2 dir[N] = {
                    float2( 1, 0), float2(-1, 0), float2(0, 1), float2(0,-1),
                    float2( 1, 1), float2(-1, 1), float2(1,-1), float2(-1,-1)
                };
                [unroll] for(int k=0;k<N;k++){
                    float2 uvn = uv + dir[k]*texel;
                    if(_IgnoreUVBorder!=0 && !AwayFromBorder(uvn, texel, _BorderMargin)) continue;
                    if(!In01(uvn)) continue;
                    if(SampleSprite(uvn).a > th) return true;
                }
                return false;
            }

            bool NeighborIsZero(float2 uv, float2 texel, half marginPx)
            {
                const int N = 4;
                float2 dir[N] = { float2(1,0), float2(-1,0), float2(0,1), float2(0,-1) };
                [unroll] for(int k=0;k<N;k++){
                    float2 uvn = uv + dir[k]*texel;
                    if(_IgnoreUVBorder!=0 && !AwayFromBorder(uvn, texel, marginPx)) continue;
                    if(!In01(uvn)) continue;
                    if(SampleSprite(uvn).a == 0) return true;
                }
                return false;
            }

            fixed4 fragOutline(v2f i):SV_Target
            {
                if(_OutlineEnabled == 0 || _Thickness <= 0 || _ExpandPixels <= 0)
                    return 0;

                fixed4 src = SampleSprite(i.uv) * i.col;
                src.rgb *= src.a;

                // 픽셀 두께 → UV 오프셋
                float2 texel = float2(_Thickness / _MainTex_TexelSize.z,
                                      _Thickness / _MainTex_TexelSize.w);

                // 바깥 윤곽(투명 → 이웃이 불투명)
                bool edgeOutside = (src.a == 0) && NeighborHasAlpha(i.uv, texel, _AlphaThreshold, _BorderMargin);

                // 안쪽 윤곽(옵션)
                bool edgeInside  = (_OutsideOnly==0) && (src.a > 0) && NeighborIsZero(i.uv, texel, _BorderMargin);

                if(edgeOutside || edgeInside){
                    fixed4 oc = _SolidOutline; oc.rgb *= oc.a;
                    return fixed4(oc.rgb, oc.a); // sprite 위에 바로 그림
                }
                // outline pass는 outline만 그리고 투명 반환
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }

    Fallback Off
}
