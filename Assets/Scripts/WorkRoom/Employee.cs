using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Employee : MonoBehaviour
{
    public TextMeshProUGUI floatingText;
    public GameObject ballonPanel;

    public Staminar staminar;
    private SnacksInventory snacksInventory;

    public string EmployeeName;

    public void SetBallonPanel(GameObject panel)
    {
        ballonPanel = panel;
        floatingText = panel.GetComponentInChildren<TextMeshProUGUI>(true);
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
