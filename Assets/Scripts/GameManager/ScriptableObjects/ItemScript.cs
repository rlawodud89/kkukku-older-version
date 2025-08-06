using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObject/ItemScript")]
public class ItemScript : ScriptableObject
{
    public string itemName;
    public Sprite image;
    public ItemType itemType;

    [Header("간식, 재료 레벨")]
    public int level;

    [Header("간식은 회복량, 나머지는 판매가격")]
    public int value;

    [Header("이불 제작 때 필요한 재료 이름, 수량 리스트")]
    public List<RecipeEntry> recipe; // <(아이템 이름, 필요 수량)>

    [Header("이불 디자인 가격")]
    public int designValue;

    [Header("이불창 찼을 때 이미지")]
    public Sprite fullImage;
}

[System.Serializable]
public class RecipeEntry
{
    public string itemName;
    public int count;
}

