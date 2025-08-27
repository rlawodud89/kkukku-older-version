using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class fishingUISlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;

    public void SetData(ItemScript data, int count)
    {
        if (data.image == null)
        {
            gameObject.SetActive(false);
            return;
        }

        icon.sprite = data.image;
        countText.text = count.ToString();
        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        Debug.Log("clear");
        icon.sprite = null;
        countText.text = "";
        gameObject.SetActive(false); // Added this line
    }
}