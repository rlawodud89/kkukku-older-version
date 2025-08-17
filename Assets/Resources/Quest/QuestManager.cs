using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestSO> activeQuests = new List<QuestSO>();  // 퀘스트 리스트

    public GameObject questPanel; // 퀘스트 패널
    public GameObject scrollContent; // 스크롤 콘텐츠
    public GameObject questButtonPrefab;
    public List<GameObject> questButtons = new List<GameObject>(); // 퀘스트 버튼 리스트

    public GameObject questContentPanel; // 퀘스트 내용 패널
    public GameObject questRewardPanel; // 퀘스트 보상 패널
    public GameObject questRewardPrefab; // 퀘스트 보상 프리팹
    public Sprite CoinImage; // 코인 이미지
    public Sprite MoonRockImage; // 월석 이미지
    public Sprite CozyEnergyImage; // 포근에너지 이미지

    public GameObject completeAlertIcon;
    public GameObject startAlertIcon;

    //public QuestSO[] quests;

    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.getInstance();
        // 퀘스트 데이터 로드
        //QuestSO quest = Resources.Load<QuestSO>("Quest1"); 
        //StartQuest(quest);  // 퀘스트 시작

        //StartQuest(quests);   // 나중에 아침 시작할 때 퀘스트 주는걸로 바꾸기 

        //// 사용자가 껏다 켯을때, 저장되었을 때 상태 불러오기 ////

        ///데베 연결 시 아래거 지우기
        StartNewDay();
    }

    // Update is called once per frame
    void Update()
    {
        // 퀘스트 완료 상태 업데이트
        foreach (GameObject qb in questButtons)
        {
            QuestSO quest = activeQuests.Find(q => q.questTitle == qb.transform.Find("QuestTitle").GetComponent<TMPro.TextMeshProUGUI>().text);
            if (quest != null)
            {
                if (quest.isCompleted)
                {
                    qb.transform.Find("ResultText").GetComponent<TMPro.TextMeshProUGUI>().text = "완료!";
                    //qb.transform.Find("Alert").gameObject.SetActive(true); 
                    
                    //// 데이터베이스에서 퀘스트 삭제 + 만약 연계 퀘스트가 있다면 해당 연계 퀘스트 로드 

                    if (!quest.getReward)
                    {
                        qb.transform.Find("Alert").gameObject.SetActive(true);
                    }
                    else
                    {
                        qb.transform.Find("Alert").gameObject.SetActive(false);
                    }
                }
                else
                {
                    qb.transform.Find("ResultText").GetComponent<TMPro.TextMeshProUGUI>().text = "진행 중";
                    qb.transform.Find("Alert").gameObject.SetActive(false);
                }
            }

            if (qb.transform.Find("Alert").gameObject.activeSelf)
            {
                completeAlertIcon.SetActive(true); // 퀘스트가 활성화되어 있으면 아이콘 표시
            }
            else
            {
                completeAlertIcon.SetActive(false); // 퀘스트가 비활성화되면 아이콘 숨김
            }
        }
    }

    public void StartNewDay()
    {
        // 새로운 날 시작 시 퀘스트 초기화
        DestroyAllQuestButtons(); // 기존 퀘스트 버튼 리스트 초기화
        activeQuests.Clear(); // 기존 퀘스트 리스트 초기화
        ////데베에서 이전 퀘스트 삭제(일반퀘스트만. 현재 퀘스트 중 특퀘 여부 확인 후 아닌 것만 삭제)
        LoadRandomQuests(3); // 새로운 퀘스트 로드
        ////특별퀘스트 로드
        StartQuest(activeQuests); // 새로 로드한 퀘스트 시작
    }
    //------------------------시계연결

    private void OnEnable()
    {
        StartCoroutine(EnsureSubscribed());
    }
    private IEnumerator EnsureSubscribed()
    {
        while (GameManager.getInstance() == null) yield return null;
        GameManager.getInstance().OnDayEnded += StartNewDay;
    }
    
    //---------------------------------

    // 퀘스트 랜덤으로 불러오기
    void LoadRandomQuests(int count)
    {
        // 모든 퀘스트 불러오기
        QuestSO[] allQuests = Resources.LoadAll<QuestSO>("Quest");

        // 중복 없이 랜덤으로 섞고 일부만 선택
        activeQuests = allQuests.OrderBy(q => Random.value).Take(count).ToList();

        ////allQuests 데베에 저장.

        // 결과 확인
        foreach (var quest in activeQuests)
        {
            Debug.Log($"선택된 퀘스트: {quest.questTitle}");
        }

        startAlertIcon.SetActive(activeQuests.Count > 0); // 퀘스트가 있으면 아이콘 표시
    }

    // 퀘스트 시작
    public void StartQuest(List<QuestSO> quests)
    {
        foreach (QuestSO currentQuest in quests)
        {
            // 퀘스트 초기화
            currentQuest.getReward = false; // 보상 수령 여부 초기화
            currentQuest.questProcess = 0; // 퀘스트 진행 상태 초기화
            currentQuest.isCompleted = false; // 퀘스트 완료 여부 초기화

            // 퀘스트 패널 설정
            GameObject questButton = Instantiate(questButtonPrefab, scrollContent.transform);
            questButtons.Add(questButton);
            questButton.transform.Find("QuestTitle").GetComponent<TMPro.TextMeshProUGUI>().text = currentQuest.questTitle;
            if (currentQuest.isCompleted)
                questButton.transform.Find("ResultText").GetComponent<TMPro.TextMeshProUGUI>().text = "완료!";
            else
            {
                questButton.transform.Find("ResultText").GetComponent<TMPro.TextMeshProUGUI>().text = "진행 중";
            }

            // 버튼 클릭 이벤트
            questButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                OnQuestButtonClicked(questButton, currentQuest);
            });
        }
    }

    void OnQuestButtonClicked(GameObject questButton, QuestSO quest)
    {
        // 퀘스트 완료 시
        if (quest.isCompleted)
        {
            questRewardPanel.SetActive(true); // 퀘스트 보상 패널 열기

            int rewardCount = quest.rewards.Length;

            // 퀘스트 보상 패널 설정
            for (int i = 0; i < rewardCount; i++)
            {
                GameObject questReward = Instantiate(questRewardPrefab, questRewardPanel.transform);
                questReward.transform.Find("AmountText").GetComponent<TMPro.TextMeshProUGUI>().text = quest.rewards[i].amount.ToString();

                // 보상 아이콘 업데이트 (보상 종류에 따라 이미지 변경)
                UnityEngine.UI.Image rewardImage = questReward.transform.Find("RewardImage").GetComponent<Image>();

                switch (quest.rewards[i].rewardType)
                {
                    case "재화":
                        rewardImage.sprite = CoinImage;
                        break;
                    case "월석":
                        rewardImage.sprite = MoonRockImage;
                        break;
                    case "포근에너지":
                        rewardImage.sprite = CozyEnergyImage;
                        break;
                    default:
                        rewardImage.sprite = null; // 알 수 없는 보상은 기본 이미지로 설정
                        break;
                }

                RectTransform rewardRect = questReward.GetComponent<RectTransform>();

                float spacing = 150f;  // 보상 간 간격을 고정
                float totalWidth = spacing * (rewardCount - 1); // 전체 너비 계산
                float startX = -totalWidth / 2f; // 첫 보상의 시작 위치 (가운데 정렬)

                float positionX = startX + i * spacing; // 보상별 X 위치 계산


                rewardRect.anchoredPosition = new Vector2(positionX, 5);

            }

            if (quest.getReward)
            {
                Debug.Log("이미 보상을 받았습니다.");
                questRewardPanel.transform.Find("GetButton").GetComponent<Image>().color = new Color(181f / 255f, 174f / 255f, 174f / 255f);
                return; // 이미 보상을 받은 경우
            }
            else
            {
                // 보상받기 버튼 클릭 이벤트
                questRewardPanel.transform.Find("GetButton").GetComponent<Image>().color = Color.white;
                Button getButton = questRewardPanel.transform.Find("GetButton").GetComponent<Button>();
                getButton.onClick.AddListener(() =>
                {
                    ProcessReward(quest, getButton);
                    questRewardPanel.transform.Find("GetButton").GetComponent<Image>().color = new Color(181f / 255f, 174f / 255f, 174f / 255f);
                });
            }



        }
        // 퀘스트 진행 중
        else
        {
            questContentPanel.SetActive(true);
            questContentPanel.transform.Find("QuestTitle").GetComponent<TMPro.TextMeshProUGUI>().text = quest.questTitle;
            questContentPanel.transform.Find("QuestDetail").GetComponent<TMPro.TextMeshProUGUI>().text = quest.questDescription;

            // 퀘스트 진행 상태 표시
            if (quest.questComplete > 1)
            {
                questContentPanel.transform.Find("QuestProgress").GetComponent<TMPro.TextMeshProUGUI>().text = $"{quest.questProcess} / {quest.questComplete}";
            }
            else
            {
                questContentPanel.transform.Find("QuestProgress").gameObject.SetActive(false); // 진행 상태가 없으면 숨김
            }

            int rewardCount = quest.rewards.Length;    // 보상의 개수

            // 퀘스트 보상 패널 설정
            for (int i = 0; i < rewardCount; i++)
            {
                GameObject questReward = Instantiate(questRewardPrefab, questContentPanel.transform);
                questReward.transform.Find("AmountText").GetComponent<TMPro.TextMeshProUGUI>().text = quest.rewards[i].amount.ToString();

                // 보상 아이콘 업데이트 (보상 종류에 따라 이미지 변경)
                UnityEngine.UI.Image rewardImage = questReward.transform.Find("RewardImage").GetComponent<Image>();

                switch (quest.rewards[i].rewardType)
                {
                    case "재화":
                        rewardImage.sprite = CoinImage;
                        break;
                    case "월석":
                        rewardImage.sprite = MoonRockImage;
                        break;
                    case "포근에너지":
                        rewardImage.sprite = CozyEnergyImage;
                        break;
                    default:
                        rewardImage.sprite = null; // 알 수 없는 보상은 기본 이미지로 설정
                        break;
                }

                RectTransform rewardRect = questReward.GetComponent<RectTransform>();

                float spacing = 150f;  // 보상 간 간격을 고정
                float totalWidth = spacing * (rewardCount - 1); // 전체 너비 계산
                float startX = -totalWidth / 2f; // 첫 보상의 시작 위치 (가운데 정렬)

                float positionX = startX + i * spacing; // 보상별 X 위치 계산


                rewardRect.anchoredPosition = new Vector2(positionX, -195);
            }
        }
    }


    // 퀘스트 진행 상태 업데이트
    public void AddProcessToQuest(QuestSO quest, int amount)
    {
        // 퀘스트 진행 상태 업데이트
        quest.questProcess += amount;

        // 퀘스트 완료 여부 확인
        if (quest.questProcess >= quest.questComplete)
        {
            quest.isCompleted = true; // 퀘스트 완료 상태로 변경
            Debug.Log($"퀘스트 '{quest.questTitle}' 완료!");
        }
        else
        {
            Debug.Log($"퀘스트 '{quest.questTitle}' 진행 중: {quest.questProcess} / {quest.questComplete}");
        }
    }


    // 보상받기 버튼 클릭 이벤트
    public void ProcessReward(QuestSO quest, Button getButton)
    {
        quest.getReward = true; // 보상 수령 상태 업데이트

        foreach (var reward in quest.rewards)
        {
            switch (reward.rewardType)
            {
                case "재화":
                    Debug.Log($"보상: {reward.amount} 재화");
                    // 실제로 재화를 지급하는 로직 추가 (예: 플레이어의 재화 양 증가)
                    gameManager.Change_Gold(reward.amount);
                    break;

                case "월석":
                    Debug.Log($"보상: {reward.amount} 월석");
                    // 실제로 월석을 지급하는 로직 추가 (예: 인벤토리에 월석 추가)
                    gameManager.Change_Moonrock(reward.amount);
                    break;

                case "포근에너지":
                    Debug.Log($"보상: {reward.amount} 포근에너지");
                    // 실제로 포근에너지를 지급하는 로직 추가 (예: 플레이어 포근에너지 증가)
                    break;

                default:
                    Debug.Log("알 수 없는 보상 종류입니다.");
                    break;
            }
        }

        // 클릭이벤트 해제
        getButton.onClick.RemoveAllListeners();
    }

    public bool IsQuestActive(QuestSO quest)
    {
        // 주어진 퀘스트가 활성화되어 있는지 확인
        return activeQuests.Contains(quest) && !quest.isCompleted;
    }


    public void PanelClose()
    {
        questPanel.SetActive(false);
        ////특퀘의 경우
        if (_currentQuestNpc != null)
        {
            _currentQuestNpc.OnQuestPanelClosed();
            _currentQuestNpc = null;
        }
    }

    public void PanelOpen()
    {
        questPanel.SetActive(true);

        if (startAlertIcon.activeSelf)
        {
            startAlertIcon.SetActive(false);
        }
    }

    // 퀘스트 내용 패널 닫기
    public void QuestContentPanelClose()
    {
        questContentPanel.SetActive(false);

        foreach (Transform child in questContentPanel.transform)
        {
            if (child.name.StartsWith("RewardPanel"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    // 퀘스트 보상 패널 닫기
    public void QuestRewardPanelClose()
    {
        questRewardPanel.SetActive(false);

        foreach (Transform child in questRewardPanel.transform)
        {
            if (child.name.StartsWith("RewardPanel"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    ////================= 특퀘 ========================////
    
    private AStarMover _currentQuestNpc;
    
    
    public void OpenQuestFromNpc(AStarMover npc, QuestSO quest)
    {
        _currentQuestNpc = npc;

        // 필요하면 활성 목록에 추가(중복 방지)
        if (quest && !activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            PlusQuest(quest);   // ★ 특퀘 버튼 생성
        }
            
        // 데베에 추가
        PanelOpen();

        // 버튼 경유 말고 바로 내용 패널 띄우기
        ShowQuestDetail(quest); // ★ 아래 헬퍼 메서드 추가
    }
    // QuestManager.cs (내용 패널 그려주는 헬퍼)
    private void ShowQuestDetail(QuestSO quest)
    {
        if (quest == null) return;

        // 이전 생성물 정리(중복 생성 방지)
        ClearRewardItems(questContentPanel.transform);

        questContentPanel.SetActive(true);
        questContentPanel.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>().text = quest.questTitle;
        questContentPanel.transform.Find("QuestDetail").GetComponent<TextMeshProUGUI>().text = quest.questDescription;

        var progress = questContentPanel.transform.Find("QuestProgress").GetComponent<TextMeshProUGUI>();
        if (quest.questComplete > 1)
        {
            progress.gameObject.SetActive(true);
            progress.text = $"{quest.questProcess} / {quest.questComplete}";
        }
        else progress.gameObject.SetActive(false);

        // 리워드 아이템 생성(컨테이너 밑으로)
        int rewardCount = quest.rewards.Length;
        for (int i = 0; i < rewardCount; i++)
        {
            GameObject questReward = Instantiate(questRewardPrefab, questContentPanel.transform);
            questReward.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = quest.rewards[i].amount.ToString();

            var rewardImage = questReward.transform.Find("RewardImage").GetComponent<Image>();
            switch (quest.rewards[i].rewardType)
            {
                case "재화": rewardImage.sprite = CoinImage; break;
                case "월석": rewardImage.sprite = MoonRockImage; break;
                case "포근에너지": rewardImage.sprite = CozyEnergyImage; break;
                default: rewardImage.sprite = null; break;
            }

            // 배치
            var rewardRect = questReward.GetComponent<RectTransform>();
            float spacing = 150f;
            float totalWidth = spacing * (rewardCount - 1);
            float startX = -totalWidth / 2f;
            float positionX = startX + i * spacing;
            rewardRect.anchoredPosition = new Vector2(positionX, -195);
        }
    }

    private void ClearRewardItems(Transform panel)
    {
        // 프리팹 이름 의존 없이 RewardImage/AmountText가 있는 애를 지우는 안전한 방식
        var buffer = new List<Transform>();
        foreach (Transform child in panel)
            if (child.Find("RewardImage") && child.Find("AmountText"))
                buffer.Add(child);
        foreach (var t in buffer) Destroy(t.gameObject);
    }
    public void PlusQuest(QuestSO q)
    {
        GameObject questButton = Instantiate(questButtonPrefab, scrollContent.transform);
        questButtons.Add(questButton);
        questButton.transform.Find("QuestTitle").GetComponent<TMPro.TextMeshProUGUI>().text = q.questTitle;
        if (q.isCompleted)
            questButton.transform.Find("ResultText").GetComponent<TMPro.TextMeshProUGUI>().text = "완료!";
        else
        {
            questButton.transform.Find("ResultText").GetComponent<TMPro.TextMeshProUGUI>().text = "진행 중";
        }

        // 버튼 클릭 이벤트
        questButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            OnQuestButtonClicked(questButton, q);
        });

    }
    private void DestroyAllQuestButtons()
    {
        foreach (var go in questButtons)
            if (go) Destroy(go);
        questButtons.Clear();

        // scrollContent 밑에 혹시 남아있는 자식도 깔끔히 제거
        foreach (Transform child in scrollContent.transform)
            Destroy(child.gameObject);
    }
}
