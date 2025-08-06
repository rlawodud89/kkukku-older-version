using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewMaterial", menuName = "Custom/Material")]
public class MaterialData : ScriptableObject
{
    public string MaterialName;
    public Sprite MaterialSprite;
    public int level;
    public string tag;
}
