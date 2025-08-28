using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnackSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;
    public Sprite defaultSprite;

    public Image Text_image;

    public void SetData(ItemScript data, int count)
    {
        icon.sprite = data.image;
        countText.text = count.ToString();
        Text_image.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        Debug.Log("clear");
        icon.sprite = defaultSprite;
        countText.text = "";
        Text_image.gameObject.SetActive(false);
    }
}
