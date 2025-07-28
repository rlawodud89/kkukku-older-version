using UnityEngine;
using UnityEngine.UI;

public class MaterialItemUI : MonoBehaviour
{
    public Image icon;
    public Text label;
    public Text quantity;

    public void Set(MaterialData data)
    {
        icon.sprite = data.materialIcon;
        label.text = data.materialName;
        quantity.text = data.materialquantity;
    }
}
