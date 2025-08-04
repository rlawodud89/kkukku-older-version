using UnityEngine;
using UnityEngine.UI;

public class MaterialItemUI : MonoBehaviour
{
    public Image icon;
    public Text label;
    public Text quantity;

    public void Set(MaterialData data)
    {
        icon.sprite = data.MaterialSprite;
        label.text = data.MaterialName;
        //quantity.text = data.Materialquantity;
    }
}
