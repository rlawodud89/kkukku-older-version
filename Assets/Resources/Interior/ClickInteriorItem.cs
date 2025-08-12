using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class ClickInteriorItem : MonoBehaviour
{
    public Material normalMaterial;  // 기본
    public Material outlineMaterial;  // 선택했을 때 
    public Material errorMaterial;   // 잘못된 위치일 때
    [HideInInspector] public bool selected=false;

    public LayerMask groundLayer;    // 바닥 레이어
    public LayerMask obstacleLayer;    // 겹침 검사 대상(벽/가구 등)

    public Canvas targetCanvas;
    public GameObject checkButtonPrefab;
    private GameObject checkButton;
    private RectTransform checkButtonRectTransform;
    public Vector3 worldOffset = new Vector3(0, -1.5f, 0); // 오브젝트 기준 월드 오프셋
    public Vector2 screenOffset = Vector2.zero;            // 화면 픽셀 보정
   // public bool hideWhenOffscreen = true;       

    private Renderer rend;
    Collider2D col;
    private InteriorManager interiorManager;
    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = normalMaterial;

        interiorManager = FindObjectOfType<InteriorManager>(); 

        cam = Camera.main;

        col = GetComponent<Collider2D>();
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if (checkButton != null && targetCanvas != null)
            UpdateButtonPosition();

        // 다른 곳 클릭하면 선택 해제
        if (Input.GetMouseButtonDown(0) && selected)
        {
            Vector3 wp = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 p = new Vector2(wp.x, wp.y);

            // 화면 포인트에서 “0거리” 레이(=그 점에 있는 콜라이더 히트)
            RaycastHit2D hit2D = Physics2D.Raycast(p, Vector2.zero);

            if (hit2D.collider != null)
            {
                if (!hit2D.transform.IsChildOf(transform))
                {
                    selected = false;
                    Debug.Log($"{gameObject.name} 선택 해제 (다른 오브젝트 2D)");
                }
            }
        }
        
        if(!selected){
            rend.material = normalMaterial;
            if (checkButton != null) Destroy(checkButton.gameObject);
        }

        if(selected){
            bool isGrounded = CheckGround();
            bool isOverlap = CheckOverlap();

            if (!isGrounded || isOverlap)
            {
                rend.material = errorMaterial; // 문제 있음 → 빨간색
                checkButton.GetComponent<Button>().interactable = false;
            }else
            {
                rend.material = outlineMaterial; // 문제 없음 → 파란색
                checkButton.GetComponent<Button>().interactable = true;
            }

        }
    }
/*
    void OnMouseDown()
    {
        if (interiorManager != null && interiorManager.interiorMode)
        {
            rend.material = outlineMaterial;

            if (checkButtonPrefab != null && targetCanvas != null)
            {
                GameObject btn = Instantiate(checkButtonPrefab, targetCanvas.transform);

                // UI 위치를 오브젝트 아래쪽에 맞게 변환
                Vector3 worldPos = transform.position + Vector3.down * 1.5f;
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                btn.GetComponent<RectTransform>().position = screenPos;
            }     
        }
    }*/

    void OnMouseDown()
    {
        Select();
    }

    public void Select(){
         // 꾸미기 모드일 때
        if (interiorManager != null && interiorManager.interiorMode)
        {
            selected=true;

            if (outlineMaterial) rend.material = outlineMaterial;  // 외각선 강조

            // 체크 버튼 생성
            // 이미 만들어졌다면 새로 만들지 않고 그대로 사용
            if(targetCanvas == null)
            {
                Transform canvasTf = GameObject.Find("UICanvas")?.transform;
                targetCanvas = canvasTf?.GetComponent<Canvas>();
            }
            if (checkButton == null && checkButtonPrefab != null && targetCanvas != null)
            {
                // Canvas 자식으로 생성하고 RectTransform 캐싱
                checkButton = Instantiate(checkButtonPrefab, targetCanvas.transform);
                checkButtonRectTransform = checkButton.GetComponent<RectTransform>();

                checkButton.GetComponent<Button>().onClick.AddListener(ClickCheckButton);
            }

            // 생성 직후 한 번 위치 맞추기
            UpdateButtonPosition();
        }
    }

    void UpdateButtonPosition()
    {
        if (checkButton == null) return;

        var cam = Camera.main;
        if (cam == null) return;

        // 1) 월드 기준 위치 계산
        Vector3 worldPos = transform.position + worldOffset;

        // 2) 화면 좌표로 변환
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        checkButtonRectTransform.position = screenPos + (Vector3)screenOffset;
    }

    // 체크 버튼 클릭 시
    void ClickCheckButton(){
        selected=false;
        rend.material = normalMaterial;
        if (checkButton != null) Destroy(checkButton.gameObject);
    }

    // 나가기 버튼 클릭 시 
    public void ClickExitInteriorButton(){
        selected=false;
        rend.material = normalMaterial;
        if (checkButton != null) Destroy(checkButton.gameObject);
    }

    // 바닥에 닿았는지 검사
    bool CheckGround()
    {
        var b = col.bounds;
        Vector2 origin = b.center;
        // 반높이 + 여유(0.05~0.2 정도 씬에 맞추어 조정)
        float dist = b.extents.y;

        // groundLayer가 비었으면 기본 레이어 사용
        int mask = (groundLayer.value == 0) ? Physics2D.DefaultRaycastLayers : groundLayer.value;

        Debug.DrawLine(origin, origin + Vector2.down * dist, Color.cyan, 0f, false);
        var hit = Physics2D.Raycast(origin, Vector2.down, dist, mask);
        return hit.collider != null;
    }

    // 다른 물체와 겹쳤는지
    bool CheckOverlap(){
        var b = col.bounds;
        Vector2 center = b.center;
        Vector2 halfSize = new Vector2(b.extents.x * 0.8f, b.extents.y * 0.98f);
        float angle = 0f;

        int mask = (obstacleLayer.value == 0) ? Physics2D.DefaultRaycastLayers : obstacleLayer.value;

        // 본인 제외 필터링
        var hits = Physics2D.OverlapBoxAll(center, halfSize * 2f, angle, mask);
        foreach (var h in hits)
        {
            if (!h || h == col) continue;
            if (h.isTrigger) continue;

            var d = Physics2D.Distance(col, h);

            if (d.isOverlapped && d.distance < -0.0001f)
                return true;
        }
        return false;
    }

}
