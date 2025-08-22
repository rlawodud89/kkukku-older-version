
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class FishingController : MonoBehaviour
{
    public bool fishing_start = false;
    private Coroutine fishingRoutine;

    public TextMeshProUGUI fishing_txt;
    public Button fishing_closebtn;
    public Button fishing_btn;
    public GameObject checkpanel;

    public MaterialsInventory materialsInventory;
    public FishingMiniGame fishingminigame;

    private ItemScript currentdata;
    private GameManager gameManager;



    public float minDelay = 4f;
    public float maxDelay = 7f;

    private void Start()
    {

        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

    }
    public void click_fishingbtn()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        if (gameManager.Get_EnergyLevel() >= 2)
        {
            if (!fishing_start)
            {
                fishing_start = true;

                fishingRoutine = StartCoroutine(SpawnItemLoop());
            }

            fishing_btn.gameObject.SetActive(false);
            fishing_closebtn.gameObject.SetActive(true);
        }
        else
        {
            checkpanel.SetActive(true);
        }
    }

    public void click_fishingstopbtn()
    {
        if (fishingminigame == null)
        {
            fishingminigame = FindObjectOfType<FishingMiniGame>();
        }

        if (fishingminigame.miniGameRunning)
        {
            fishingminigame.miniGameRunning = false;
        }
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

    public void Click_yesbtn()
    {
        checkpanel.SetActive(false);
    }


    IEnumerator SpawnItemLoop()
    {
        while (fishing_start)
        {
            fishing_txt.text = "낚시 중...";

            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);


            if (fishingminigame == null)
            {
                fishingminigame = FindObjectOfType<FishingMiniGame>();
            }

            fishingminigame.GetMaterial();
            //getMaterial();


            yield return new WaitForSeconds(2f);
        }
    }


    void getMaterial()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        currentdata = gameManager.Get_Random_Material();
        materialsInventory.AddMaterial(currentdata);
        gameManager.Add_InventoryItem(currentdata.itemName, 1);
        Debug.Log(currentdata.itemName + "획득");

    }
}
