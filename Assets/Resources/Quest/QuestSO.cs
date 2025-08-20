using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CreateAssetMenu를 사용하여 유니티 에디터에서 퀘스트 데이터를 쉽게 만들 수 있게 합니다.
[CreateAssetMenu(fileName = "Quest", menuName = "Quest/QuestData")]
public class QuestSO : ScriptableObject
{
    // 퀘스트에 대한 다양한 정보들
    public string questTitle;        // 퀘스트 제목
    public string questDescription;  // 퀘스트 설명
    public Reward[] rewards;          // 퀘스트 보상 종류와 양 (보상 클래스 배열)
    public int questProcess;          // 퀘스트 진행 상태
    public int questComplete;       // 퀘스트 완료 상태  
    public bool isCompleted;         // 퀘스트 완료 여부
    public bool getReward;         // 퀘스트 보상 수령 여부

    public bool isSpecial;           // 특별 퀘스트 여부
    public QuestSO nextQuest;        // 이어지는 다음 퀘스트 (있다면)
    [TextArea(5, 10)]
    public string content;       // 편지 본문
}