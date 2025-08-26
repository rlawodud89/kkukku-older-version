using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class QuestManager : MonoBehaviour
{
    [SerializeField] private bool useAddressables = true; // ← Addressables 쓰려면 true

    //보상 후 연계 퀘스트 애니메이션
    [SerializeField, Range(0f, 1.5f)] private float nextQuestDelay = 0.40f; // 보상 후 잠깐 쉬는 시간
    [SerializeField, Range(0f, 1.5f)] private float nextQuestInAnim = 0.60f; // 패널 팝인 시간


    [SerializeField] private Sprite normalButtonBg;     // 일반 버튼 배경
    [SerializeField] private Sprite specialButtonBg;    // 특퀘 버튼 배경
    [SerializeField] private Color specialTitleColor = new Color(1f, 0.94f, 0.6f); // 특퀘 텍스트 색(금빛 느낌)

    private IEnumerator LoadQuestsByLabel_Addressables(string label, int count, bool thenStart = false)
    {
        var handle = Addressables.LoadAssetsAsync<QuestSO>(label, null);
        yield return handle;

        var pool = handle.Result.Where(q => !q.isSpecial).ToList();
        if (pool.Count == 0)
        {
            Debug.LogError($"[QuestManager] Addressables 라벨 '{label}' 로 로드된 QuestSO가 없습니다.");
            yield break;
        }

        foreach (var q in pool.OrderBy(_ => UnityEngine.Random.value).Take(count))
        {
            ResetQuest(q);
            if (!activeQuests.Contains(q))
                activeQuests.Add(q);

            gameManager.Add_Quest(q.questTitle);
        }

        startAlertIcon.SetActive(activeQuests.Count > 0);

        if (thenStart) StartQuest(activeQuests);

        Addressables.Release(handle);
    }


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
    private IEnumerator Start()
    {
        while (gameManager == null)
        {
            gameManager = GameManager.getInstance();
            yield return null; // 한 프레임 대기
        }
        // 퀘스트 데이터 로드
        //QuestSO quest = Resources.Load<QuestSO>("Quest1"); 
        //StartQuest(quest);  // 퀘스트 시작

        //StartQuest(quests);   // 나중에 아침 시작할 때 퀘스트 주는걸로 바꾸기 

        //// 사용자가 껏다 켯을때, 저장되었을 때 상태 불러오기 ////

        //WipeAllQuestRows();

        if (LoadDBQuests())
        {
            StartQuest(activeQuests);
        }
        else
        {
            StartNewDay();
        }

        //gameManager.Add_InventoryItem("은하꿈실", 1);
        //gameManager.Add_InventoryItem("오로라빛이불", 1);
        //gameManager.Add_InventoryItem("햇빛운무솜", 1);
        //gameManager.Add_InventoryItem("몽환의꽃잎", 1);
        //gameManager.Add_InventoryItem("햇빛운무솜", 1);
        //gameManager.Add_InventoryItem("몽환의꽃잎", 1);
        //gameManager.Add_InventoryItem("청야달조각", 2);
        //Debug.Log($"[INV] 청야달조각: {gameManager.Count_InventoryItem("청야달조각")}");
        //Debug.Log($"[INV] 오로라빛이불: {gameManager.Count_InventoryItem("오로라빛이불")}");

    }

    private void WipeAllQuestRows()
    {
        var dbQuests = gameManager.Get_Current_Quest();
        if (dbQuests == null || dbQuests.Count == 0)
        {
            Debug.Log("[QuestManager] DB에 삭제할 퀘스트가 없습니다.");
        }
        else
        {

            foreach (var quest in dbQuests)
            {
                ResetQuest(quest);
                gameManager.Remove_Quest(quest.questTitle);
                Debug.Log($"[QuestManager] DB 삭제: {quest.questTitle}");
            }
        }
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
        }
        bool anyAlert = questButtons.Any(b => b.transform.Find("Alert").gameObject.activeSelf);
        completeAlertIcon.SetActive(anyAlert);
    }

    public void StartNewDay()
    {
        // 새로운 날 시작 시 퀘스트 초기화
        ////데베에서 이전 퀘스트 삭제(일반퀘스트만. 현재 퀘스트 중 특퀘 여부 확인 후 아닌 것만 삭제)
        foreach (var quest in activeQuests)
        {
            if (!quest.isSpecial)
            {
                gameManager.Remove_Quest(quest.questTitle); // 데베에서 퀘스트 삭제
                Debug.Log($"퀘스트 삭제: {quest.questTitle}");
            }
        }
        activeQuests.Clear(); // 기존 퀘스트 리스트 초기화
        DestroyAllQuestButtons(); // 기존 퀘스트 버튼 리스트 초기화
        LoadDBQuests(); ////특별퀘스트 로드
        StartCoroutine(LoadQuestsByLabel_Addressables("quest", 3, thenStart: true));
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


    //------------------------

    bool LoadDBQuests()
    {
        activeQuests = gameManager.Get_Current_Quest();
        if (activeQuests.Count == 0) { return false; }
        // 결과 확인
        foreach (var quest in activeQuests)
        {
            Debug.Log($"데베에서 불러온 퀘스트: {quest.questTitle}");
        }
        startAlertIcon.SetActive(activeQuests.Count > 0); // 퀘스트가 있으면 아이콘 표시
        return true;
    }

    // 퀘스트 시작
    public void StartQuest(List<QuestSO> quests)
    {
        foreach (QuestSO currentQuest in quests)
        {
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
            StyleQuestButton(questButton, currentQuest);
            // 버튼 클릭 이벤트
            questButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                OnQuestButtonClicked(questButton, currentQuest);
            });
        }
    }

    public void ResetQuest(QuestSO quest)
    {

        quest.getReward = false; // 보상 수령 여부 초기화
        quest.questProcess = 0; // 퀘스트 진행 상태 초기화
        quest.isCompleted = false; // 퀘스트 완료 여부 초기화
    }

    void OnQuestButtonClicked(GameObject questButton, QuestSO quest)
    {
        // 퀘스트 완료 시
        if (quest.isCompleted)
        {
            questRewardPanel.SetActive(true); // 퀘스트 보상 패널 열기
            ClearRewardItems(questRewardPanel.transform);

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

            // ✅ 이미 수령한 경우: 버튼 회색 처리 + 리스너 제거 후 바로 return
            Button getButton = questRewardPanel.transform.Find("GetButton").GetComponent<Button>();
            Image getBtnImg = getButton.GetComponent<Image>();

            getButton.onClick.RemoveAllListeners(); // 중복 방지

            bool already = quest.getReward;

            // ★ 인터랙션/색을 함께 제어해야 ‘진짜’ 활성화
            getButton.interactable = !already;
            getBtnImg.color = already
                ? new Color(181f / 255f, 174f / 255f, 174f / 255f)  // 비활성 색
                : Color.white;

            if (!already)
            {
                getButton.onClick.AddListener(() =>
                {
                    if (quest.getReward) return; // 이중가드
                    ProcessReward(quest, getButton);
                    HandleQuestAfterReward(quest, questButton);
                });
            }

        }
        // 퀘스트 진행 중
        else
        {
            questContentPanel.SetActive(true);
            ClearRewardItems(questRewardPanel.transform);
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

    public void SpecialQuestGetReward(QuestSO quest)
    {
        // 퀘스트 완료 시
        if (quest.isCompleted)
        {
            questRewardPanel.SetActive(true); // 퀘스트 보상 패널 열기
            ClearRewardItems(questRewardPanel.transform);

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

            // ✅ 이미 수령한 경우: 버튼 회색 처리 + 리스너 제거 후 바로 return
            Button getButton = questRewardPanel.transform.Find("GetButton").GetComponent<Button>();
            Image getBtnImg = getButton.GetComponent<Image>();

            getButton.onClick.RemoveAllListeners(); // 중복 방지

            bool already = quest.getReward;

            // ★ 인터랙션/색을 함께 제어해야 ‘진짜’ 활성화
            getButton.interactable = !already;
            getBtnImg.color = already
                ? new Color(181f / 255f, 174f / 255f, 174f / 255f)  // 비활성 색
                : Color.white;

            if (!already)
            {
                getButton.onClick.AddListener(() =>
                {
                    if (quest.getReward) return;
                    ProcessReward(quest, getButton);
                    HandleQuestAfterReward(quest, null); // ★ 버튼 없음
                });
            }

        }
        // 퀘스트 진행 중
        else
        {
            questContentPanel.SetActive(true);
            ClearRewardItems(questRewardPanel.transform);
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
    private void HandleQuestAfterReward(QuestSO quest, GameObject questButton)
    {
        // 1) 이전 퀘스트 정리
        gameManager.Remove_Quest(quest.questTitle);
        if (questButton == null)
            RemoveButtonByTitle(quest.questTitle);
        else
        {
            questButtons.Remove(questButton);
            Destroy(questButton);
        }

        activeQuests.Remove(quest);

        // 2) 보상 패널 닫기
        QuestRewardPanelClose();

        // 3) 연계 퀘스트가 있으면 추가하고 곧장 내용 패널 오픈
        if (quest.nextQuest != null)
        {
            StartCoroutine(TransitionToNextQuestSimple(quest.nextQuest));
        }

        // 4) 알림 재계산
        RecalculateCompleteAlertIcon();
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
        if (quest.getReward) return;
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
                    gameManager.Change_Energy(reward.amount);
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
    private void RecalculateCompleteAlertIcon()
    {
        bool anyAlert = questButtons.Any(b =>
        {
            var t = b.transform.Find("Alert");
            return t != null && t.gameObject.activeSelf;
        });
        completeAlertIcon.SetActive(anyAlert);
    }

    // QuestManager.cs 내부

    private GameObject FindButtonByTitleFromList(string title)
    {
        if (string.IsNullOrEmpty(title)) return null;

        // null 들어있을 수 있으니 먼저 정리
        questButtons.RemoveAll(b => b == null);

        foreach (var go in questButtons)
        {
            var t = go.transform.Find("QuestTitle");
            var tmp = t ? t.GetComponent<TMPro.TextMeshProUGUI>() : null;
            if (tmp != null && tmp.text == title)
                return go;
        }
        return null;
    }

    private void RemoveButtonByTitle(string title)
    {
        var btn = FindButtonByTitleFromList(title);
        if (btn != null)
        {
            questButtons.Remove(btn);
            Destroy(btn);
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
        StyleQuestButton(questButton, q);
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


    // 보상 수령 직후, 짧은 딜레이 + 페이드/스케일로 다음 퀘스트 내용 등장
    private IEnumerator TransitionToNextQuestSimple(QuestSO nextQuestAsset)
    {
        // 0) 아주 짧게 쉬는 맛
        yield return new WaitForSeconds(nextQuestDelay);

        // 1) 연계 퀘스트 생성 + 버튼
        var nextQuest = AddNextQuestAndReturn(nextQuestAsset);

        // 2) 내용 세팅 전 준비
        PreparePanelForAnim(questContentPanel);

        // 3) 내용 채우기
        ShowQuestDetail(nextQuest);

        // 4) 부드럽게 등장
        yield return AnimatePanelIn(questContentPanel, nextQuestInAnim);
    }


    // 연계 퀘스트 생성 + 버튼 바인딩 + 런타임 퀘스트 반환
    private QuestSO AddNextQuestAndReturn(QuestSO nextQuestAsset)
    {
        var quest = nextQuestAsset;
        ResetQuest(quest);

        activeQuests.Add(quest);
        gameManager.Add_Quest(quest.questTitle);
        if (startAlertIcon) startAlertIcon.SetActive(activeQuests.Count > 0);

        var btnGO = Instantiate(questButtonPrefab, scrollContent.transform);
        StyleQuestButton(btnGO, quest);
        questButtons.Add(btnGO);
        btnGO.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>().text = quest.questTitle;
        btnGO.transform.Find("ResultText").GetComponent<TextMeshProUGUI>().text = "진행 중";
        var btn = btnGO.GetComponent<Button>();
        btn.onClick.AddListener(() => OnQuestButtonClicked(btnGO, quest));

        return quest;
    }

    // 패널 준비/등장 애니메이션(아주 심플)
    private void PreparePanelForAnim(GameObject panel)
    {
        var cg = GetOrAdd<CanvasGroup>(panel);     // ← 안전
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        var rt = panel.transform as RectTransform;
        rt.localScale = Vector3.one * 0.95f;
        panel.SetActive(true);
    }

    private IEnumerator AnimatePanelIn(GameObject panel, float duration)
    {
        var cg = GetOrAdd<CanvasGroup>(panel);     // ← 안전
        var rt = panel.transform as RectTransform;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            cg.alpha = Mathf.Lerp(0f, 1f, k);
            rt.localScale = Vector3.one * Mathf.Lerp(0.95f, 1f, k);
            yield return null;
        }

        cg.alpha = 1f;
        rt.localScale = Vector3.one;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private static T GetOrAdd<T>(GameObject go) where T : Component
    {
        return go.TryGetComponent<T>(out var c) ? c : go.AddComponent<T>();
    }

    // --------------- 특퀘 생김새 -------------------


    private void StyleQuestButton(GameObject btnGO, QuestSO quest)
    {
        // 배경 이미지 스왑
        var bg = btnGO.GetComponent<Image>();
        if (bg)
        {
            if (quest.isSpecial && specialButtonBg) bg.sprite = specialButtonBg;
            else if (normalButtonBg) bg.sprite = normalButtonBg;

            // 9슬라이스 쓰면 깔끔: (선택)
            bg.type = Image.Type.Sliced;
        }

        // 텍스트 컬러
        var title = btnGO.transform.Find("QuestTitle")?.GetComponent<TextMeshProUGUI>();
        var result = btnGO.transform.Find("ResultText")?.GetComponent<TextMeshProUGUI>();
        if (quest.isSpecial)
        {
            if (title) title.color = specialTitleColor;
            if (result) result.color = specialTitleColor;
        }


    }

}
