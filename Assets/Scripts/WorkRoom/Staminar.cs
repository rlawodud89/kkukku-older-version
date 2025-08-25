using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Staminar : MonoBehaviour
{
    public Image fillImage;
    public float maxStamina;
    public float currentStamina;

    private int lastLevel = -1;
    private GameManager gameManager;

    void Update()
    {
        if (gameManager == null)
            gameManager = GameManager.getInstance();

        int level = 1;
        switch (gameObject.tag)
        {
            case "Fox": level = gameManager.Get_LoomLevel(); break;
            case "Sheep": level = gameManager.Get_FillerLevel(); break;
            case "Cat": level = gameManager.Get_DecoLevel(); break;
        }

        // 레벨이 바뀐 경우에만 적용
        if (level != lastLevel)
        {
            lastLevel = level;

            switch (level)
            {
                case 1: maxStamina = 100f; break;
                case 2: maxStamina = 130f; break;
                case 3: maxStamina = 150f; break;
                case 4: maxStamina = 170f; break;
                case 5: maxStamina = 200f; break;
                default: maxStamina = 100f; break;
            }

        }
    }


    void Start()
    {
        StaminarUI();
    }

    public void StaminarUI()
    {
        fillImage.fillAmount = currentStamina / maxStamina;

        Debug.Log("fillAmount"+fillImage.fillAmount);
        Debug.Log("current" + currentStamina);
        Debug.Log("max" + maxStamina);
        if (currentStamina < 50)
        {
            fillImage.color = Color.yellow;
        }
        if (currentStamina < 30)
        {
            fillImage.color = Color.red;
        }

        // 항상 카메라를 향하도록 (옵션)
        transform.forward = Camera.main.transform.forward;
    }

    public void Addstamina(int extrastamina)
    {
        currentStamina += extrastamina;
        StaminarUI();
    }

    public void RechargeFullStamina()
    {
        currentStamina = maxStamina;
        StaminarUI();
        Debug.Log($"[{gameObject.tag}] 풀충전 완료: {currentStamina}/{maxStamina}");
    }
}
