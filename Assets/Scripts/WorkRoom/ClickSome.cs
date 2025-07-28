using UnityEngine;

public class ClickSome : MonoBehaviour
{
    public GameObject scrollView; // Scroll View 할당

    private Vector3 mouseDownPos;
    private float dragThreshold = 10f; // 최소 이동 거리 (픽셀 단위)

    void OnMouseDown()
    {
        mouseDownPos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        float movedDistance = Vector3.Distance(Input.mousePosition, mouseDownPos);

        if (movedDistance < dragThreshold)
        {
            scrollView.SetActive(true); // 클릭으로 판단될 때만 열기
        }
    }
}
