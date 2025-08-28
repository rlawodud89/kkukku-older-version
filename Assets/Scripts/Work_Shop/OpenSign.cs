using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSign : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite closeSprite;
    private SpriteRenderer spriteRenderer;


    private GameManager gm;

    // ───────────────── 라이프사이클 ─────────────────
    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        // 씬 로드 콜백은 딱 한 번만 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        HookAndApply();
    }

    private void OnDisable()
    {
        Unhook();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Unhook();
    }

    // ───────────────── 씬 로드 ──────────────────────
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!this) return; // 파괴 직전 호출 보호

        // 새 인스턴스거나 참조 끊겼을 수 있으니 다시 확보
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

        HookAndApply();
    }

    // ───────────────── 이벤트 구독/해제 ──────────────
    private void HookAndApply()
    {
        gm = GameManager.getInstance();
        if (gm == null) return;

        // 중복 구독 방지: 항상 빼고 다시 등록
        gm.OnOpenChanged -= UpdateSign;
        gm.OnOpenChanged += UpdateSign;

        // 현재 상태 즉시 반영
        Apply(gm.Get_IsOpen());
    }

    private void Unhook()
    {
        if (gm != null)
            gm.OnOpenChanged -= UpdateSign;
    }

    // ───────────────── 핸들러 ───────────────────────
    private void UpdateSign(bool isOpen)
    {
        Apply(isOpen);
    }

    private void Apply(bool isOpen)
    {
        if (!this) return;                     // Unity의 'fake null' 케이스 보호
        if (!spriteRenderer)                   // 런타임에 빠졌을 수 있음
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (!spriteRenderer) return;
        }

        // 같은 스프라이트면 불필요한 재할당 방지
        var target = isOpen ? openSprite : closeSprite;
        if (spriteRenderer.sprite != target)
            spriteRenderer.sprite = target;
    }

}
