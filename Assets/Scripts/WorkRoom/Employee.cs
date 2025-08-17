using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Employee : MonoBehaviour
{
    public TextMeshProUGUI floatingText;
    public GameObject ballonPanel;
    public Image ballonimage;

    public Staminar staminar;
    private SnacksInventory snacksInventory;

    public string EmployeeName;

    void Start()
    {
        // ballonPanel 자동 연결 (EmployeeName과 같은 태그로 찾기)
        if (ballonPanel == null && !string.IsNullOrEmpty(EmployeeName))
        {
            GameObject panelObj = GameObject.FindWithTag(EmployeeName);
            if (panelObj != null)
            {
                ballonPanel = panelObj;
                ballonimage = panelObj.GetComponentInChildren<Image>();
                floatingText = panelObj.GetComponentInChildren<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning(EmployeeName + " 태그를 가진 BallonPanel을 찾지 못함");
            }
        }
    }

    public void GiveItem(ItemScript item)
    {
        if (snacksInventory == null)
            snacksInventory = FindObjectOfType<SnacksInventory>();

        Debug.Log(snacksInventory != null ? "찾음" : "못 찾음");

        snacksInventory.GiveSnackToEmployee(item);
        staminar.Addstamina(item.value);
        ShowFloatingText("+" + item.value);
    }

    public void Working()
    {
        staminar.Addstamina(-5);
        ShowFloatingText("열심히 만들어볼게!");
    }

    public void ShowFloatingText(string text)
    {
        if (floatingText != null)
            floatingText.text = text;

        if (floatingText != null)
            floatingText.gameObject.SetActive(true);

        if (ballonPanel != null)
            ballonPanel.SetActive(true);

        Invoke(nameof(HideFloatingText), 1.5f);
    }

    public void HideFloatingText()
    {
        if (floatingText != null)
            floatingText.gameObject.SetActive(false);

        if (ballonPanel != null)
            ballonPanel.SetActive(false);
    }
}
