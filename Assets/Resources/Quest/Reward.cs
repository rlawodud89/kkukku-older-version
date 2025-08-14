using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reward
{
    public string rewardType;  // 보상 종류 (예: '골드', '아이템', '경험치')
    public int amount;         // 보상 양 (예: 100골드, 1개 아이템 등)
}