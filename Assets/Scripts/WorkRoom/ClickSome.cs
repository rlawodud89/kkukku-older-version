using UnityEngine;

public class ClickSome : MonoBehaviour
{
    public GameObject scrollView; // Scroll View �Ҵ�
    public GameObject Panel;
    private Vector3 mouseDownPos;
    private float dragThreshold = 2f; // �ּ� �̵� �Ÿ� (�ȼ� ����)

    private InteriorManager interiorManager;

    void Start()
    {
        interiorManager = FindObjectOfType<InteriorManager>();       
    }

    void Update()
    {

    }

    void OnMouseDown()
    {
        mouseDownPos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (interiorManager != null && interiorManager.interiorMode)
            return;
        
        float movedDistance = Vector3.Distance(Input.mousePosition, mouseDownPos);

        if (movedDistance < dragThreshold)
        {
            scrollView.SetActive(true); // Ŭ������ �Ǵܵ� ���� ����
            Panel.SetActive(true);
        }
    }
}
