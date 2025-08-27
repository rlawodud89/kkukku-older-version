using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GradientNightSky : MonoBehaviour
{
    [Header("Target RawImage (full-screen)")]
    public RawImage target;

    [Header("Colors (Bottom → Top)")]
    // 몽환 보라 팔레트
    public Color bottom = new Color32(26, 17, 40, 255);  // #1A1128 (아래 짙은 보라)
    public Color mid = new Color32(45, 31, 71, 255);  // #2D1F47 (중간)
    public Color top = new Color32(63, 52, 112, 255); // #3F3470 (위쪽 보라/남색)

    [Header("Texture")]
    [Range(64, 2048)] public int texHeight = 512;  // 1 x H이면 충분
    public bool regenerateOnValidate = true;

    Texture2D _tex;

    void OnEnable() { Build(); }
    void OnValidate() { if (regenerateOnValidate) Build(); }
    void OnDisable() { DestroyTex(); }

    void DestroyTex()
    {
        if (_tex)
        {
            if (Application.isPlaying) Destroy(_tex);
            else DestroyImmediate(_tex);
            _tex = null;
        }
    }

    public void Build()
    {
        if (!target) return;

        if (_tex == null || _tex.height != texHeight)
        {
            DestroyTex();
            _tex = new Texture2D(1, texHeight, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
        }

        for (int y = 0; y < texHeight; y++)
        {
            float t = (float)y / (texHeight - 1);   // 0(bottom) → 1(top)
            // 부드러운 2단 그라데이션: bottom→mid(0.6)→top
            Color c = (t < 0.6f)
                ? Color.Lerp(bottom, mid, t / 0.6f)
                : Color.Lerp(mid, top, (t - 0.6f) / 0.4f);

            _tex.SetPixel(0, y, c);
        }
        _tex.Apply();

        target.texture = _tex;
        // RawImage가 화면 전체일 때 수직으로만 늘려 보이도록 UV 보정
        target.uvRect = new Rect(0, 0, 1, (float)Screen.height / texHeight);
        target.color = Color.white;
    }
}