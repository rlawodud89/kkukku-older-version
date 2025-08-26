using UnityEngine;
using UnityEngine.UI;

public class Staminar : MonoBehaviour
{
    public Image fillImage;
    public float maxStamina;
    public float currentStamina;

    private int lastLevel = -1;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();
        UpdateMaxStamina();
        StaminarUI();
    }

    public void StaminarUI()
    {
        if (maxStamina <= 0)
        {
            fillImage.fillAmount = 0;
        }
        else
        {
            fillImage.fillAmount = currentStamina / maxStamina;
        }

        if (currentStamina < 50)
        {
            fillImage.color = Color.yellow;
        }
        if (currentStamina < 30)
        {
            fillImage.color = Color.red;
        }

        transform.forward = Camera.main.transform.forward;
    }

    public void Addstamina(int extrastamina)
    {
        currentStamina += extrastamina;
        StaminarUI();
    }

    public void UpdateMaxStamina()
    {

        if (gameManager==null)
        {
            gameManager=GameManager.getInstance();
        }
        int level = 1;
        switch (gameObject.tag)
        {
            case "Fox": level = gameManager.Get_LoomLevel(); break;
            case "Sheep": level = gameManager.Get_FillerLevel(); break;
            case "Cat": level = gameManager.Get_DecoLevel(); break;
        }

        if (level != lastLevel)
        {
            lastLevel = level;
            switch (level)
            {
                case 1: maxStamina = 100; break;
                case 2: maxStamina = 130; break;
                case 3: maxStamina = 150; break;
                case 4: maxStamina = 170; break;
                case 5: maxStamina = 200; break;
                default: maxStamina = 100; break;
            }
        }
    }
}