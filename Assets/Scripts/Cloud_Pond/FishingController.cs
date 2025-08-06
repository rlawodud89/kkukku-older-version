using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishingController : MonoBehaviour
{
    private bool fishing_start = false;
    private Coroutine fishingRoutine;

    public Text fishing_txt;
    public Button fishing_closebtn;
    public Button fishing_btn;

    public float minDelay = 4f;
    public float maxDelay = 7f;

    
    private void Awake()
    {
        
    }
    


    public void click_fishingbtn()
    {
        if (!fishing_start)
        {
            fishing_start = true;
            fishingRoutine = StartCoroutine(SpawnItemLoop());
        }

        fishing_btn.gameObject.SetActive(false);
        fishing_closebtn.gameObject.SetActive(true);

       
    }

    public void click_fishingstopbtn()
    {
        if (fishing_start)
        {
            fishing_start = false;

            if (fishingRoutine != null)
            {
                StopCoroutine(fishingRoutine);
                fishingRoutine = null;
            }

            fishing_txt.text = ""; // �ؽ�Ʈ �ʱ�ȭ
        }

        fishing_closebtn.gameObject.SetActive(false);
        fishing_btn.gameObject.SetActive(true);
    }

    IEnumerator SpawnItemLoop()
    {
        while (fishing_start)
        {
            fishing_txt.text = "���ø� �����մϴ�.";
            yield return new WaitForSeconds(2f);
            fishing_txt.text = "";

            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            fishing_txt.text = "��� ȹ��!";
            yield return new WaitForSeconds(2f);
            fishing_txt.text = "";
        }
    }
}
