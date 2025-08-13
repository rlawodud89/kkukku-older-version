using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "ScriptableObject/QuestScript")]
public class QuestScript : ScriptableObject
{
    public string questName;
    public bool special;

    [Header("퀘스트 완료 보상, 없는 것은 안 써도 됨")]
    public int rewardGold;
    public int rewardMoonrock;
    [Header("완료 보상 아이템 이름, 수량 추가")]
    public List<rewardItemEntry> rewardItem;

    [Header("다음 퀘스트가 있는 경우, 다음 퀘스트의 이름 저장")]
    public string nextQuestName;

    [Header("특별 퀘스트인 경우, 완료 후 얻는 편지 이름 저장")]
    public string letterName;
}

[System.Serializable]
public class rewardItemEntry
{
    public string itemName;
    public int count;
}