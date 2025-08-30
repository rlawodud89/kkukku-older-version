using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ClickSome : MonoBehaviour
{
    [Header("채널 연결(같은 에셋을 양쪽에서 참조해야 함)")]
    public VoidEventChannelSO onAppearChannel;
    public VoidEventChannelSO onCompleteChannel;

    private Transform canvasTransform;
    public GameObject scrollView;
    public GameObject Panel;
    private Vector3 mouseDownPos;
    private float dragThreshold = 2f;

    private InteriorManager interiorManager;
    private GameManager gameManager;
    private bool specialOn = false;

    void Start()
    {
        interiorManager = FindObjectOfType<InteriorManager>();
        gameManager = GameManager.getInstance();

        canvasTransform = GameObject.Find("UICanvas")?.transform;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        //if (onAppearChannel != null) onAppearChannel.OnRaised += TurnOn;
        //if (onCompleteChannel != null) onCompleteChannel.OnRaised += TurnOff;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        //if (onAppearChannel != null) onAppearChannel.OnRaised -= TurnOn;
        //if (onCompleteChannel != null) onCompleteChannel.OnRaised -= TurnOff;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        interiorManager = FindObjectOfType<InteriorManager>();
        gameManager = GameManager.getInstance();

        canvasTransform = GameObject.Find("UICanvas")?.transform;
    }

    // 패널 세팅
    void SetPanel(GameObject gameObject)
    {
        Debug.Log($"Setting panel for {gameObject.name}");
        if (gameObject.name == "blanket_storage(Clone)")
        {
            Panel = canvasTransform.Find("BlanketStorage_Panel").gameObject;
            Debug.Log($"Found Panel: {Panel.name}");
            scrollView = Panel.transform.Find("BlanketStorage_ScrollView").gameObject;
            Debug.Log($"Found ScrollView: {scrollView.name}");
        }
        else if (gameObject.name == "material_storage(Clone)")
        {
            Panel = canvasTransform.Find("MaterialStorage_Panel").gameObject;
            scrollView = Panel.transform.Find("MaterialStorage_Scroll View").gameObject;
        }
        else if (gameObject.name == "snack_box(Clone)")
        {
            Panel = canvasTransform.Find("Snacks_Panel").gameObject;
            scrollView = Panel.transform.Find("SnackStorage_Scroll View").gameObject;
        }
        else if (gameObject.name == "Employee1(Clone)") // 원단 직원
        {
            Panel = canvasTransform.Find("Fabric_Panel").gameObject;
            scrollView = Panel.transform.Find("Fabric_Scroll View").gameObject;
        }
        else if (gameObject.name == "Employee2(Clone)") // 솜 직원
        {
            Panel = canvasTransform.Find("Cotton_Panel").gameObject;
            scrollView = Panel.transform.Find("Cotton_Scroll View").gameObject;
        }
        else if (gameObject.name == "Employee3(Clone)") // 데코 직원
        {
            Panel = canvasTransform.Find("Sewing_Panel").gameObject;
            scrollView = Panel.transform.Find("Sewing_Scroll View").gameObject;
        }
        else if (gameObject.name == "Special_Crafting(Clone)")
        {
            Panel = canvasTransform.Find("Special_Panel").gameObject;
        }

    }

    void Update()
    {
        if (Panel == null || scrollView == null)
        {
            SetPanel(this.gameObject);
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

        if (gameManager.isDayEndPanel) return;

        // 마우스가 UI 위에 있을 경우 → 클릭 무시
        if (IsPointerOverUI())
        {
            Debug.Log("UI 위 클릭 → Table 클릭 무시");
            return;
        }

        if (gameObject.name == "Special_Crafting(Clone)")
        {
            var quests = gameManager.Get_Current_Quest();
            specialOn = false;
            foreach (var q in quests)
            {
                if (q.questTitle == "제대로 된 이불 (2)") specialOn = true;
            }

            if (!specialOn) return;
        }

        float movedDistance = Vector3.Distance(Input.mousePosition, mouseDownPos);

        if (movedDistance < dragThreshold)
        {
            if (scrollView != null)
            {
                scrollView.SetActive(true);
            }
            Panel.SetActive(true);
            //Debug.LogError("Panel");

            if (gameObject.name == "Employee1(Clone)") // 원단 직원
            {
                Employee employee = gameObject.GetComponent<Employee>();
                Make_Fabric.Instance.Set_CurrentEmployee(employee.EmployeeID);
            }
            else if (gameObject.name == "Employee2(Clone)") // 솜 직원
            {
                Employee employee = gameObject.GetComponent<Employee>();
                Make_Cotton.Instance.Set_CurrentEmployee(employee.EmployeeID);
            }
            else if (gameObject.name == "Employee3(Clone)") // 데코 직원
            {
                Employee employee = gameObject.GetComponent<Employee>();
                Make_Sewing.Instance.Set_CurrentEmployee(employee.EmployeeID);
            }
        }
    }

    // 실제 UI 위에 있는지 확인하는 정밀 메서드
    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        // 모든 GraphicRaycaster를 검사
        foreach (var gr in FindObjectsOfType<GraphicRaycaster>())
        {
            gr.Raycast(eventData, results);
            if (results.Count > 0) // 하나라도 걸리면
                return true;
        }

        return false;
    }
}
