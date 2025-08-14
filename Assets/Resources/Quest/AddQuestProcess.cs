using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddQuestProcess : MonoBehaviour
{   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddProcessToQuest(QuestSO quest)
    {
        if (quest != null)  
        {
            QuestManager.Instance.AddProcessToQuest(quest, 1);
        }
        else
        {
            Debug.LogError("QuestSO is not assigned.");
        }
    }
}
