using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialUISlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;

    public void SetData(MaterialData data, int count)
    {
        icon.sprite = data.MaterialSprite;
        countText.text = count.ToString();
        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        Debug.Log("clear");
        icon.sprite = null;
        countText.text = "";
    }
}
