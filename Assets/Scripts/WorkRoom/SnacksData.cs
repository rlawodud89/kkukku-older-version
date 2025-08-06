using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSnack", menuName = "Custom/Snack")]
public class SnacksData : ScriptableObject
{
    public string SnackName;
    public Sprite SnackSprite;
    public int extrastamina;
    public int extraseconds;
    public string reactionMessage;
    public int level;
}
