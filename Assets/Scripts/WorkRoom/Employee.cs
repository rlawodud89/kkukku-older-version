using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Employee : MonoBehaviour
{   
    public TextMeshProUGUI floatingText;
    public TextMeshProUGUI reactionText;
    public GameObject ballonPanel;
    public Image ballonimage;

    public Staminar staminar;
    public SnacksInventory snacksInventory;

    public string EmployeeName;

    public void GiveItem(ItemScript item)
    {
        snacksInventory.GiveSnackToEmployee(item);
        Debug.Log("GiveSnackToEmployee 호출됨");


        Debug.Log("먹이줌");
        
        staminar.Addstamina(item.value);
        ShowFloatingText("+" + item.value);
        
        //ShowReaction(item.reactionMessage);
    }

    public void Working()
    {
        staminar.Addstamina(-5);

        ShowFloatingText("열심히 만들어볼게!");
    }

    public void ShowFloatingText(string text)
    {

        floatingText.text = text;
        floatingText.gameObject.SetActive(true);
        // 간단한 fade out 애니메이션 추가 가능
        ballonPanel.SetActive(true);
        Invoke(nameof(HideFloatingText), 1.5f);
    }

    public void ShowFloatingFabric(ItemScript blanket)
    {

    }

    public void HideFloatingText()
    {
        floatingText.gameObject.SetActive(false);
        ballonPanel.SetActive(false);
    }

    void ShowReaction(string message)
    {
        reactionText.text = message;
        reactionText.gameObject.SetActive(true);
        Invoke(nameof(HideReaction), 2.0f);
    }

    void HideReaction()
    {
        reactionText.gameObject.SetActive(false);
    }
}
