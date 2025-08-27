using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Fader : MonoBehaviour
{
    [Header("Timings (sec)")]
    public float fadeOut = 0.45f;
    public float fadeIn = 0.45f;
    public float minDarkHold = 0.10f;

    [Header("Options")]
    [Range(0f, 1f)] public float maxAlpha = 1f;
    public bool useUnscaledTime = true;
    public int overlaySortingOrder = 32766;
    public Color overlayColor = new Color(0, 0, 0, 1);

    static Fader _I;
    Canvas _canvas;
    CanvasGroup _cg;
    Image _img;
    bool _busy;

    // ---------- Public API (비활성이어도 스스로 켜서 실행) ----------
    public static void Go(string sceneName)
    {
        var f = Ensure();
        if (!f.gameObject.activeSelf) f.gameObject.SetActive(true);
        f.StartCoroutine(f.CoFadeScene(sceneName));
    }

    public static void GoConcurrent(string sceneName, float minDarkTime = 0.15f, bool waitUntilReady = true)
    {
        var f = Ensure();
        if (!f.gameObject.activeSelf) f.gameObject.SetActive(true);
        f.StartCoroutine(f.CoFadeSceneConcurrent(sceneName, minDarkTime, waitUntilReady));
    }

    public static void FadeOnly(float outT, float inT, Action onDark = null)
    {
        var f = Ensure();
        if (!f.gameObject.activeSelf) f.gameObject.SetActive(true);
        f.StartCoroutine(f.CoFadeOnly(outT, inT, onDark));
    }

    public static void Test() => FadeOnly(0.3f, 0.3f, null);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) Test();
    }

    // ---------- Ensure: 비활성까지 검색 ----------
    static Fader Ensure()
    {
        if (_I != null) return _I;

        var all = Resources.FindObjectsOfTypeAll<Fader>(); // 비활성 포함
        if (all != null && all.Length > 0)
        {
            _I = all[0];
            return _I;
        }

        // 완전 없으면 즉석 생성
        var go = new GameObject("~Fader(Auto)");
        _I = go.AddComponent<Fader>();
        return _I;
    }

    void Awake()
    {
        if (_I == null) _I = this;
    }

    void OnEnable()
    {
        EnsureOverlay();
        SetAlpha(0f);
        if (_cg) _cg.blocksRaycasts = false;
        if (_img) _img.raycastTarget = false;
        EnableOverlayCanvas(true);
        ForceTopMost();
    }

    void OnDisable()
    {
        EnableOverlayCanvas(false);
    }

    // ---------- Overlay ----------
    void EnsureOverlay()
    {
        if (_canvas == null)
        {
            var go = new GameObject("FadeCanvas");
            go.transform.SetParent(transform, false);
            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = overlaySortingOrder;
            go.AddComponent<GraphicRaycaster>();
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
            var c = overlayColor; c.a = 0f;
            _img.color = c;
            _img.raycastTarget = false;
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

    void EnableOverlayCanvas(bool on)
    {
        if (_canvas) _canvas.enabled = on;
        var gr = _canvas ? _canvas.GetComponent<GraphicRaycaster>() : null;
        if (gr) gr.enabled = on;
    }

    // ---------- Core Coroutines (끝나면 자동 비활성) ----------
    IEnumerator CoFadeScene(string sceneName)
    {
        if (_busy) yield break;
        _busy = true;

        try
        {
            BeginBlock();
            yield return Lerp01(fadeOut, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));
            yield return WaitSeconds(minDarkHold);

            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            op.allowSceneActivation = true;
            while (!op.isDone) yield return null;

            yield return Lerp01(fadeIn, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));
        }
        finally
        {
            EndBlock();
            _busy = false;
            gameObject.SetActive(false); // ← 자동 비활성
        }
    }

    IEnumerator CoFadeSceneConcurrent(string sceneName, float minDarkTime, bool waitUntilReady)
    {
        if (_busy) yield break;
        _busy = true;

        try
        {
            BeginBlock();

            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            op.allowSceneActivation = false;

            yield return Lerp01(fadeOut, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));

            if (waitUntilReady)
                while (op.progress < 0.9f) yield return null;

            yield return WaitSeconds(minDarkTime);

            op.allowSceneActivation = true;
            while (!op.isDone) yield return null;

            yield return Lerp01(fadeIn, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));
        }
        finally
        {
            EndBlock();
            _busy = false;
            gameObject.SetActive(false); // ← 자동 비활성
        }
    }

    IEnumerator CoFadeOnly(float outT, float inT, Action onDark)
    {
        if (_busy) yield break;
        _busy = true;

        try
        {
            BeginBlock();
            yield return Lerp01(outT, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));
            onDark?.Invoke();
            yield return WaitSeconds(minDarkHold);
            yield return Lerp01(inT, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));
        }
        finally
        {
            EndBlock();
            _busy = false;
            gameObject.SetActive(false); // ← 자동 비활성
        }
    }

    // ---------- Helpers ----------
    void BeginBlock()
    {
        if (_cg) _cg.blocksRaycasts = true;
        if (_img) _img.raycastTarget = true;
        EnableOverlayCanvas(true);
        ForceTopMost();
    }

    void EndBlock()
    {
        SetAlpha(0f);
        if (_cg) _cg.blocksRaycasts = false;
        if (_img) _img.raycastTarget = false;
        EnableOverlayCanvas(false);
    }

    void SetAlpha(float a)
    {
        if (_cg) _cg.alpha = a;
        if (_img)
        {
            var c = _img.color; c.a = a; _img.color = c;
        }
        bool block = a > 0.001f; // 투명하면 통과
        if (_cg) _cg.blocksRaycasts = block;
        if (_img) _img.raycastTarget = block;
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

    IEnumerator WaitSeconds(float s)
    {
        if (useUnscaledTime)
        {
            float t = 0f;
            while (t < s) { t += Time.unscaledDeltaTime; yield return null; }
        }
        else
        {
            yield return new WaitForSeconds(s);
        }
    }

    [ContextMenu("DEBUG: Flash Overlay")]
    public void DebugFlashOverlay() { StartCoroutine(CoFlash()); }
    IEnumerator CoFlash()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        BeginBlock();
        var prev = _img ? _img.color : Color.black;
        if (_img) { _img.color = new Color(1f, 0f, 1f, 0.6f); _img.raycastTarget = false; }
        yield return WaitSeconds(1f);
        if (_img) _img.color = new Color(prev.r, prev.g, prev.b, 0f);
        EndBlock();
        gameObject.SetActive(false);
    }
}
