using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.UIElements;
using static UnityEditor.Progress;

public class Employee : MonoBehaviour
{
    public GameObject ballonPanel;
    public TextMeshProUGUI floatingText;
    public Button ItemButton;

    public Staminar staminar;
    public SnacksInventory snacksInventory;
    public ProgressCircle progressCircle;

    public string EmployeeName;
    public int EmployeeID;
    public ItemScript workItem;
    public float workingPercent;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();
    }


    public void GiveItem(ItemScript item)
    {
        snacksInventory.GiveSnackToEmployee(item);
        Debug.Log("GiveSnackToEmployee 호출됨"+item.value);

        staminar.Addstamina(item.value);
        ShowFloatingText("+" + item.value);

        gameManager.Change_Worker_Stamina(EmployeeID, item.value);
    }

    public void Working()
    {
        staminar.Addstamina(-5);

        gameManager.Change_Worker_Stamina(EmployeeID, -5);
    }


    public void ShowFloatingText(string text)
    {
        floatingText.text = text;
        floatingText.gameObject.SetActive(true);
        // 간단한 fade out 애니메이션 추가 가능
        ballonPanel.SetActive(true);
        Invoke(nameof(HideFloatingText), 1.5f);
    }

    public void HideFloatingText()
    {
        floatingText.gameObject.SetActive(false);
        ballonPanel.SetActive(false);
    }

}
