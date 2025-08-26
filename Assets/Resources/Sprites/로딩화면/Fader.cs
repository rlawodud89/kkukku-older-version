using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    // ---------- 설정 ----------
    [Header("Timings (sec)")]
    public float fadeOut = 0.45f;
    public float fadeIn = 0.45f;
    public float minDarkHold = 0.05f;

    [Header("Options")]
    [Range(0f, 1f)] public float maxAlpha = 1f;   // 완전히 가림 = 1
    public bool useUnscaledTime = true;         // 타임스케일 0에서도 동작
    public int overlaySortingOrder = 32760;     // 최상단 가까이
    public Color overlayColor = new Color(0, 0, 0, 1); // 색상(RGB만 사용, A는 애니로)

    // ---------- 내부 ----------
    static Fader _I;
    Canvas _canvas;
    CanvasGroup _cg;
    Image _img;
    bool _busy;

    

    // ----------------- 부트스트랩: 씬에 없어도 자동 준비 -----------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (_I != null) return;
        var go = new GameObject("~Fader(Auto)");
        _I = go.AddComponent<Fader>();
        DontDestroyOnLoad(go);
        _I.EnsureOverlay();
        _I.HideInstant();                // alpha=0 & 클릭 통과 & 캔버스 비활성
        Debug.Log("[Fader] Auto-initialized");
    }

    // ----------------- 싱글톤 보강(중복 방지) -----------------
    void Awake()
    {
        if (_I && _I != this) { Destroy(gameObject); return; }
        _I = this;
        DontDestroyOnLoad(gameObject);
    }

    // ----------------- 외부 API -----------------
    /// <summary>페이드아웃 → 씬 로드 → 페이드인</summary>
    public static void Go(string sceneName)
    {
        Ensure().StartCoroutine(Ensure().CoFadeScene(sceneName));
    }

    /// <summary>씬 로드 없이 화면만 페이드</summary>
    public static void FadeOnly(float outT, float inT, Action onDark = null)
    {
        Ensure().StartCoroutine(Ensure().CoFadeOnly(outT, inT, onDark));
    }

    /// <summary>간단 테스트(Alt+F도 가능)</summary>
    public static void Test() => FadeOnly(0.3f, 0.3f, () => Debug.Log("[Fader] Dark"));

    // Alt+F로 테스트(선택)
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F)) Test();
    }

    // ----------------- 구현부 -----------------
    static Fader Ensure()
    {
        if (_I == null)
        {
            var go = new GameObject("~Fader(Auto)");
            _I = go.AddComponent<Fader>();
            DontDestroyOnLoad(go);
            _I.EnsureOverlay();
            _I.HideInstant();
            Debug.Log("[Fader] Lazy-initialized");
        }
        return _I;
    }

    void EnsureOverlay()
    {
        if (_canvas == null)
        {
            var go = new GameObject("FadeCanvas");
            go.transform.SetParent(transform, false);
            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay; // 무조건 최상단
            _canvas.sortingOrder = overlaySortingOrder;
            go.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(go);
        }
        if (_cg == null)
        {
            _cg = _canvas.GetComponent<CanvasGroup>();
            if (_cg == null) _cg = _canvas.gameObject.AddComponent<CanvasGroup>();
        }
        if (_img == null)
        {
            var imgGo = new GameObject("Overlay");
            imgGo.transform.SetParent(_canvas.transform, false);
            _img = imgGo.AddComponent<Image>();
            _img.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
            _img.raycastTarget = false; // 처음엔 통과
            var rt = _img.rectTransform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }
        ForceTopMost();
    }

    void BringToFront()
    {
        if (_canvas) _canvas.sortingOrder = overlaySortingOrder;
        if (_img) _img.transform.SetAsLastSibling();
    }

    // 캔버스 on/off로 간섭 최소화
    void EnableOverlayCanvas(bool on)
    {
        if (_canvas) _canvas.enabled = on;
        var gr = _canvas ? _canvas.GetComponent<GraphicRaycaster>() : null;
        if (gr) gr.enabled = on;
    }

    IEnumerator CoFadeScene(string sceneName)
    {
        if (_busy) yield break;
        _busy = true;

        EnsureOverlay();
        EnableOverlayCanvas(true);
        BringToFront();

        yield return Lerp01(fadeOut, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));
        yield return WaitSeconds(minDarkHold);

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        yield return Lerp01(fadeIn, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));

        HideInstant(); // alpha=0 & 클릭 통과 & 캔버스 비활성
        _busy = false;
    }

    IEnumerator CoFadeOnly(float outT, float inT, Action onDark)
    {
        if (_busy) yield break;
        _busy = true;

        EnsureOverlay();
        EnableOverlayCanvas(true);
        BringToFront();

        yield return Lerp01(outT, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));
        onDark?.Invoke();
        yield return WaitSeconds(minDarkHold);
        yield return Lerp01(inT, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));

        HideInstant();
        _busy = false;
    }

    IEnumerator Lerp01(float d, Action<float> on)
    {
        float t = 0f;
        while (t < d)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            on?.Invoke(Mathf.Clamp01(t / d));
            yield return null;
        }
        on?.Invoke(1f);
    }

    // Realtime API 없이 호환성 보장
    IEnumerator WaitSeconds(float seconds)
    {
        if (useUnscaledTime)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(seconds);
        }
    }

    // 🔑 알파에 따라 자동으로 Raycast 차단/통과
    void SetAlpha(float a)
    {
        if (_cg) _cg.alpha = a;
        if (_img)
        {
            var c = _img.color; c.a = a; _img.color = c;

            bool block = a > 0.001f;     // 거의 투명이면 통과
            _img.raycastTarget = block;  // Image 차단
            if (_cg) _cg.blocksRaycasts = block; // CanvasGroup 차단
        }
    }

    // 끝난 뒤 완전 통과 + 캔버스 자체도 off
    void HideInstant()
    {
        SetAlpha(0f);
        EnableOverlayCanvas(false);
    }

    // A) 최상단 강제
    void ForceTopMost()
    {
        if (_canvas == null) return;

        // 무조건 Overlay로 올려서 카메라 렌더 순서/레이어 영향 제거
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // 루트 Canvas라도 안전빵으로 overrideSorting 켜고 최댓값 근처로
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 32766;  // 32767 근처 (다 이김)

        // 혹시 하위에 Nested Canvas가 있으면 전부 override 켜기
        var childCanvases = _canvas.GetComponentsInChildren<Canvas>(true);
        foreach (var c in childCanvases)
        {
            if (c == _canvas) continue;
            c.overrideSorting = true;
            c.sortingOrder = 32766;
        }

        // 같은 캔버스 내에서도 최상단
        if (_img) _img.transform.SetAsLastSibling();
    }

    // B) 보이는지 1초만 확실히 보여주는 디버그(보라 60%)
    [ContextMenu("DEBUG: Flash Overlay")]
    public void DebugFlashOverlay()
    {
        StartCoroutine(CoFlash());
    }
    IEnumerator CoFlash()
    {
        EnableOverlayCanvas(true);
        ForceTopMost();
        var prev = _img ? _img.color : Color.black;
        if (_img)
        {
            _img.color = new Color(1f, 0f, 1f, 0.6f); // 보라 60% (눈에 확 띔)
            _img.raycastTarget = false;               // 테스트 중 클릭 통과
        }
        yield return WaitSeconds(1f);
        if (_img) _img.color = new Color(prev.r, prev.g, prev.b, 0f);
        EnableOverlayCanvas(false);
    }
    // 1) 외부 API 하나 추가
    public static void GoConcurrent(string sceneName, float minDarkTime = 0.15f, bool waitUntilReady = true)
    {
        // minDarkTime: 완전히 가린 상태에서 최소로 유지할 시간(연출상 여유)
        // waitUntilReady: true면 0.9까지 준비될 때까지 어둡게 가린 채 기다렸다가 활성화
        Ensure().StartCoroutine(Ensure().CoFadeSceneConcurrent(sceneName, minDarkTime, waitUntilReady));
    }

    // 2) 본체 코루틴
    IEnumerator CoFadeSceneConcurrent(string sceneName, float minDarkTime, bool waitUntilReady)
    {
        if (_busy) yield break;
        _busy = true;

        EnsureOverlay();
        EnableOverlayCanvas(true);
        BringToFront();

        // (A) 로딩 시작 — 활성화 보류
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        // (B) 페이드아웃 진행하면서 동시에 로딩
        yield return Lerp01(fadeOut, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));

        // (C) 완전히 가린 뒤, 필요하면 로딩이 준비될 때(0.9)까지 기다림
        if (waitUntilReady)
            while (op.progress < 0.9f) yield return null;

        // (D) 미니멈 가림 유지(연출 여유)
        yield return WaitSeconds(minDarkTime);

        // (E) 활성화 → 씬 전환 완료
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        // (F) 페이드 인
        yield return Lerp01(fadeIn, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));

        HideInstant();
        _busy = false;
    }

}
