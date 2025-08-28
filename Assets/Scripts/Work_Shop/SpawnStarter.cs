using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnStarter : MonoBehaviour
{
    public GameObject spawner;
    private GameManager gameManager;


    private void Hook()
    {
        gameManager = GameManager.getInstance();
        if (gameManager == null) return;

        // 혹시 이미 구독돼 있으면 먼저 해제 후 다시 구독 (중복 방지)
        gameManager.OnOpenChanged -= OpenChanged;
        gameManager.OnOpenChanged += OpenChanged;

        // 현재 상태 한번 반영
        OpenChanged(gameManager.Get_IsOpen());
    }

    private void Unhook()
    {
        if (gameManager != null)
            gameManager.OnOpenChanged -= OpenChanged;
    }

    // ── 라이프사이클 ──────────────────────────────────
    private void Awake()
    {
        // 씬 로드 콜백은 Awake에서 한 번만 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        Hook();
    }

    private void OnDisable()
    {
        Unhook();
    }

    private void OnDestroy()
    {
        // 씬 로드 콜백/게임매니저 이벤트 모두 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Unhook();
    }

    // ── 씬 로드 시 spawner 재바인딩 + 이벤트 재구독 ───
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 넘어가면 이전 spawner는 파괴될 수 있으니 다시 찾아보기
        if (spawner == null)
        {
            // 방법 1) 태그 사용 권장
            var tagged = GameObject.FindGameObjectWithTag("Spawner");
            if (tagged != null) spawner = tagged;

            // 방법 2) 씬에 하나만 있다면 컴포넌트로 찾아도 됨
            if (spawner == null)
            {
                var s = FindObjectOfType<Spawner>(true);
                if (s != null) spawner = s.gameObject;
            }
        }

        // 새 씬의 객체 기준으로 다시 구독
        Hook();
    }

    // ── 열림/닫힘 반영 ─────────────────────────────────
    private void OpenChanged(bool isOpen)
    {
        // Unity의 'fake null'을 고려해 == null 체크만으로 충분
        if (spawner == null) return;

        // 불필요한 호출 줄이기
        if (spawner.activeSelf != isOpen)
            spawner.SetActive(isOpen);
    }
}