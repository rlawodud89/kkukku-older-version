using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class AddQuestProcess : MonoBehaviour
{   
    public static AddQuestProcess Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // 필요 시
        }
    }

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AddProcessToQuest(string questname)
    {

        if (!string.IsNullOrEmpty(questname))
        {
            QuestSO quest=gameManager.Get_Quest(questname);
            QuestManager.Instance.AddProcessToQuest(quest, 1);
        }
        else
        {
            Debug.LogError("QuestSO is not assigned.");
        }

    }

}
