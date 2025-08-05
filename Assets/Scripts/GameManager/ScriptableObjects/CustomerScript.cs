using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomer", menuName = "ScriptableObject/CustomerScript")]
public class CustomerScript : ScriptableObject
{
    public string customerName;
    public Sprite leftImage;
    public Sprite rightImage;
    public Sprite frontImage;
    public Sprite backImage;
}