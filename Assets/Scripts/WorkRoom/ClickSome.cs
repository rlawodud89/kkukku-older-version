using UnityEngine;

public class ClickSome : MonoBehaviour
{
    private Transform canvasTransform;
    public GameObject scrollView; // Scroll View �Ҵ�
    public GameObject Panel;
    private Vector3 mouseDownPos;
    private float dragThreshold = 2f; // �ּ� �̵� �Ÿ� (�ȼ� ����)

    private InteriorManager interiorManager;

    void Start()
    {

        interiorManager = FindObjectOfType<InteriorManager>();  

        canvasTransform = GameObject.Find("UICanvas")?.transform;
    }

    // 패널 세팅
    void SetPanel(GameObject gameObject){
        Debug.Log($"Setting panel for {gameObject.name}");
        if(gameObject.name=="blanket_storage(Clone)"){
            Panel=canvasTransform.Find("BlanketStorage_Panel").gameObject;
            Debug.Log($"Found Panel: {Panel.name}");
            scrollView=Panel.transform.Find("BlanketStorage_ScrollView").gameObject;
            Debug.Log($"Found ScrollView: {scrollView.name}");
        }else if(gameObject.name=="material_storage(Clone)"){
            Panel=canvasTransform.Find("MaterialStorage_Panel").gameObject;
            scrollView=Panel.transform.Find("MaterialStorage_Scroll View").gameObject;
        }else if(gameObject.name=="snack_box(Clone)"){
            Panel=canvasTransform.Find("Snacks_Panel").gameObject;
            scrollView=Panel.transform.Find("SnackStorage_Scroll View").gameObject;
        }else if(gameObject.name=="Employee1(Clone)"){
            Panel=canvasTransform.Find("Fabric_Panel").gameObject;
            scrollView=Panel.transform.Find("Fabric_Scroll View").gameObject;
        }else if(gameObject.name=="Employee2(Clone)"){
            Panel=canvasTransform.Find("Cotton_Panel").gameObject;
            scrollView=Panel.transform.Find("Cotton_Scroll View").gameObject;
        }else if(gameObject.name=="Employee3(Clone)"){
            Panel=canvasTransform.Find("Sewing_Panel").gameObject;
            scrollView=Panel.transform.Find("Sewing_Scroll View").gameObject;
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
