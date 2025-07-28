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

            fishing_txt.text = ""; // 텍스트 초기화
        }

        fishing_closebtn.gameObject.SetActive(false);
        fishing_btn.gameObject.SetActive(true);
    }

    IEnumerator SpawnItemLoop()
    {
        while (fishing_start)
        {
            fishing_txt.text = "낚시를 시작합니다.";
            yield return new WaitForSeconds(2f);
            fishing_txt.text = "";

            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            fishing_txt.text = "재료 획득!";
            yield return new WaitForSeconds(2f);
            fishing_txt.text = "";
        }
    }
}
