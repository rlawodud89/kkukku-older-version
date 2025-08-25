using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelHoverToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("마우스를 올렸을 때 켜질 Panel")]
    public GameObject targetPanel;

    public TextMeshProUGUI staminarText;

    public Employee employee;

    private GameManager gameManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameManager == null)
        {
            gameManager=GameManager.getInstance();
        }
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            staminarText.text = gameManager.Get_Worker_Stamina(employee.EmployeeID).ToString() +" / "+ employee.staminar.maxStamina.ToString();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetPanel != null)
            targetPanel.SetActive(false);
    }
}
