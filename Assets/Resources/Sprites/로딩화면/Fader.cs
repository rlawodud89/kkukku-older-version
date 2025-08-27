using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    // ---------- 설정 ----------
    [Header("Timings (sec)")]
    public float fadeOut = 0.45f;          // 길게: 0.45 ~ 0.6 추천
    public float fadeIn = 0.45f;
    public float minDarkHold = 0.10f;      // 완전 가려진 상태 최소 유지

    [Header("Options")]
    [Range(0f, 1f)] public float maxAlpha = 1f;    // 1=완전 가림
    public bool useUnscaledTime = true;           // 타임스케일 0에서도 동작
    public int overlaySortingOrder = 32766;       // 최상단 근처
    public Color overlayColor = new Color(0, 0, 0, 1); // RGB만 사용 (A는 애니)

    // ---------- 내부 ----------
    static Fader _I;
    Canvas _canvas;
    CanvasGroup _cg;
    Image _img;
    bool _busy;

    // 씬에 없어도 자동 준비
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (_I != null) return;
        var go = new GameObject("~Fader(Auto)");
        _I = go.AddComponent<Fader>();
        UnityEngine.Object.DontDestroyOnLoad(go);
        _I.EnsureOverlay();
        _I.HideInstant();
        Debug.Log("[Fader] Auto-initialized");
    }

    void Awake()
    {
        if (_I && _I != this) { Destroy(gameObject); return; }
        _I = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- 외부 API ----------
    /// 페이드아웃 → 씬 로드 → 페이드인 (동시 로딩 없이)
    public static void Go(string sceneName)
    {
        Ensure().StartCoroutine(Ensure().CoFadeScene(sceneName));
    }

    /// 페이드아웃과 **다음 씬 로딩을 동시에** 진행, 어두워지면 활성화
    public static void GoConcurrent(string sceneName, float minDarkTime = 0.15f, bool waitUntilReady = true)
    {
        Ensure().StartCoroutine(Ensure().CoFadeSceneConcurrent(sceneName, minDarkTime, waitUntilReady));
    }

    /// 씬 로드 없이 화면만 페이드
    public static void FadeOnly(float outT, float inT, Action onDark = null)
    {
        Ensure().StartCoroutine(Ensure().CoFadeOnly(outT, inT, onDark));
    }

    /// 간단 테스트(플래시) — 실행 중 F9로도 호출됨
    public static void Test() => FadeOnly(0.3f, 0.3f, () => Debug.Log("[Fader] Dark"));

    // F9로 플래시 테스트
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) Test();
    }

    // ---------- 구현 ----------
    static Fader Ensure()
    {
        if (_I == null)
        {
            var go = new GameObject("~Fader(Auto)");
            _I = go.AddComponent<Fader>();
            UnityEngine.Object.DontDestroyOnLoad(go);
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
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay; // 카메라/레이어 영향 제거
            _canvas.overrideSorting = true;
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

    void ForceTopMost()
    {
        if (_canvas == null) return;
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = overlaySortingOrder;
        if (_img) _img.transform.SetAsLastSibling();

        var childCanvases = _canvas.GetComponentsInChildren<Canvas>(true);
        foreach (var c in childCanvases)
        {
            if (c == _canvas) continue;
            c.overrideSorting = true;
            c.sortingOrder = overlaySortingOrder;
        }
    }

    void BringToFront()
    {
        if (_canvas) _canvas.sortingOrder = overlaySortingOrder;
        if (_img) _img.transform.SetAsLastSibling();
    }

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
        ForceTopMost();

        yield return Lerp01(fadeOut, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));
        yield return WaitSeconds(minDarkHold);

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        yield return Lerp01(fadeIn, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));

        HideInstant();
        _busy = false;
    }

    // ★ 페이드아웃과 로딩을 동시에 진행하는 버전
    IEnumerator CoFadeSceneConcurrent(string sceneName, float minDarkTime, bool waitUntilReady)
    {
        if (_busy) yield break;
        _busy = true;

        EnsureOverlay();
        EnableOverlayCanvas(true);
        ForceTopMost();

        // (A) 로딩 시작(활성화 보류)
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        // (B) 페이드아웃 진행하면서 백그라운드 로딩
        yield return Lerp01(fadeOut, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));

        // (C) 필요 시 0.9까지 준비될 때까지 대기
        if (waitUntilReady)
            while (op.progress < 0.9f) yield return null;

        // (D) 완전히 가린 상태 최소 유지(연출 여유)
        yield return WaitSeconds(minDarkTime);

        // (E) 활성화 → 씬 전환
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        // (F) 페이드 인
        yield return Lerp01(fadeIn, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));

        HideInstant();
        _busy = false;
    }

    IEnumerator CoFadeOnly(float outT, float inT, Action onDark)
    {
        if (_busy) yield break;
        _busy = true;

        EnsureOverlay();
        EnableOverlayCanvas(true);
        ForceTopMost();

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

    // 구버전 호환: Realtime API 없이도 동작
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

    // 알파에 따라 자동으로 클릭 차단/통과
    void SetAlpha(float a)
    {
        if (_cg) _cg.alpha = a;
        if (_img)
        {
            var c = _img.color; c.a = a; _img.color = c;
            bool block = a > 0.001f;     // 거의 투명이면 통과
            _img.raycastTarget = block;
            if (_cg) _cg.blocksRaycasts = block;
        }
    }

    // 끝난 뒤 완전 통과 + 캔버스 off
    void HideInstant()
    {
        SetAlpha(0f);
        EnableOverlayCanvas(false);
    }

    // 디버그 컨텍스트 메뉴: 보라색 플래시 1초
    [ContextMenu("DEBUG: Flash Overlay")]
    public void DebugFlashOverlay() { StartCoroutine(CoFlash()); }
    IEnumerator CoFlash()
    {
        EnsureOverlay(); EnableOverlayCanvas(true); ForceTopMost();
        var prev = _img ? _img.color : Color.black;
        if (_img) { _img.color = new Color(1f, 0f, 1f, 0.6f); _img.raycastTarget = false; }
        yield return WaitSeconds(1f);
        if (_img) _img.color = new Color(prev.r, prev.g, prev.b, 0f);
        HideInstant();
    }


}
