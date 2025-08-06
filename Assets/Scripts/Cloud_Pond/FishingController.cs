
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingController : MonoBehaviour
{
    private bool fishing_start = false;
    private Coroutine fishingRoutine;

    public TextMeshProUGUI fishing_txt;
    public Button fishing_closebtn;
    public Button fishing_btn;
    public MaterialManager materialManager;
    public MaterialsInventory materialsInventory;

    private MaterialData currentdata;

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
            fishing_txt.text = "낚시 중...";

            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            getMaterial();

            fishing_txt.text = currentdata.MaterialName + " 획득!";
            yield return new WaitForSeconds(2f);
            fishing_txt.text = "";
        }
    }

    void getMaterial()
    {
        int material_count = materialManager.MaterialsList.Count;
        int itemIndex = Random.Range(0, material_count - 1);

        currentdata = materialManager.MaterialsList[itemIndex];
        materialsInventory.AddMaterial(currentdata);


    }
}
