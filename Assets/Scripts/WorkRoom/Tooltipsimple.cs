using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class PanelHoverToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("마우스를 올렸을 때 켜질 Panel")]
    public GameObject targetPanel;

    public TextMeshProUGUI staminarText;

    public Employee employee;


    public void OnPointerEnter(PointerEventData eventData)
    {

        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            staminarText.text = (employee.staminar.fillImage.fillAmount*100).ToString() +" / "+ employee.staminar.maxStamina.ToString();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetPanel != null)
            targetPanel.SetActive(false);
    }
}
