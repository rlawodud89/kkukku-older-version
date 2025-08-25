using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public float fadeOut = 0.25f;
    public float fadeIn = 0.25f;
    public float minDarkHold = 0.05f;
    [Range(0f, 1f)] public float maxAlpha = 1f;
    public bool useUnscaledTime = true;
    public int overlaySortingOrder = 32760;

    static Fader _I;
    Canvas _canvas;
    CanvasGroup _cg;
    Image _img;
    bool _busy;

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

    // 외부 API
    public static void Go(string sceneName) { Ensure().StartCoroutine(Ensure().CoFadeScene(sceneName)); }
    public static void FadeOnly(float outT, float inT, Action onDark = null) { Ensure().StartCoroutine(Ensure().CoFadeOnly(outT, inT, onDark)); }
    public static void Test() { FadeOnly(0.3f, 0.3f, () => Debug.Log("[Fader] Dark")); }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F)) Test();
    }

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
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = overlaySortingOrder;
            go.AddComponent<GraphicRaycaster>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }
        if (_cg == null)
        {
            _cg = _canvas.gameObject.GetComponent<CanvasGroup>();
            if (_cg == null) _cg = _canvas.gameObject.AddComponent<CanvasGroup>();
        }
        if (_img == null)
        {
            var imgGo = new GameObject("Overlay");
            imgGo.transform.SetParent(_canvas.transform, false);
            _img = imgGo.AddComponent<Image>();
            _img.color = new Color(0, 0, 0, 0);
            _img.raycastTarget = true;
            var rt = _img.rectTransform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }
    }

    void BringToFront()
    {
        if (_canvas) _canvas.sortingOrder = overlaySortingOrder;
        if (_img) _img.transform.SetAsLastSibling();
    }

    IEnumerator CoFadeScene(string sceneName)
    {
        if (_busy) yield break;
        _busy = true;

        EnsureOverlay();
        BringToFront();
        _cg.blocksRaycasts = true;

        yield return Lerp01(fadeOut, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));
        yield return WaitSeconds(minDarkHold);   // ← 여기!

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        yield return Lerp01(fadeIn, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));

        _cg.blocksRaycasts = false;
        _busy = false;
    }

    IEnumerator CoFadeOnly(float outT, float inT, Action onDark)
    {
        if (_busy) yield break;
        _busy = true;

        EnsureOverlay();
        BringToFront();
        _cg.blocksRaycasts = true;

        yield return Lerp01(outT, a => SetAlpha(Mathf.Lerp(0f, maxAlpha, a)));
        onDark?.Invoke();
        yield return WaitSeconds(minDarkHold);   // ← 여기!

        yield return Lerp01(inT, a => SetAlpha(Mathf.Lerp(maxAlpha, 0f, a)));

        _cg.blocksRaycasts = false;
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

    // ✅ Wait를 IEnumerator로 통일 (Realtime API 불필요)
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

    void SetAlpha(float a)
    {
        if (_cg) _cg.alpha = a;
        if (_img)
        {
            var c = _img.color; c.a = a; _img.color = c;
        }
    }

    void HideInstant() => SetAlpha(0f);
}
