using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickIgnoreGround : MonoBehaviour
{
    public LayerMask clickLayerMask; 

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // UI 위 클릭이면 무시 (필요 없으면 이 줄 삭제)
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 p = cam.ScreenToWorldPoint(Input.mousePosition);

        // 그 지점에 겹치는 2D 콜라이더 중에서, 마스크에 해당하는 것만
        Collider2D hit = Physics2D.OverlapPoint(p, clickLayerMask);

        if (hit)
        {
            Debug.Log($"클릭: {hit.name}");
            hit.GetComponentInParent<ClickInteriorItem>()?.Select();
        }
        else
        {
            Debug.Log("맞은 Interactable 없음");
        }
    }
}
