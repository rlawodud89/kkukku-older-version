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

    void SetPanel(GameObject gameObject){
        Debug.Log($"Setting panel for {gameObject.name}");
        if(gameObject.name=="blanket_storage(Clone)"){
            Panel=GameObject.Find("BlanketStorage_Panel");
            Debug.Log($"Found Panel: {Panel.name}");
            scrollView=GameObject.Find("BlanketStorage_ScrollView");
            Debug.Log($"Found ScrollView: {scrollView.name}");
        }else if(gameObject.name=="material_storage(Clone)"){
            Panel=GameObject.Find("MaterialStorage_Panel");
            scrollView=GameObject.Find("MaterialStorage_ScrollView");
        }
    }

    void Update()
    {
        if (Panel == null || scrollView == null)
        {
            SetPanel(this.gameObject);
        }else{
            Debug.Log($"Panel and ScrollView are already set for {this.gameObject.name}");
        }
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
