using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Staminar : MonoBehaviour
{
    public Image fillImage;
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float time = 5f;
    void Update()
    {
        // 예시로 1초에 10씩 감소
        currentStamina -= Time.deltaTime * time;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        fillImage.fillAmount = currentStamina / maxStamina;

        if (currentStamina<50)
        {
            fillImage.color = Color.yellow;
        }
       if(currentStamina < 30)
        {
            fillImage.color = Color.red;  
        }

        // 항상 카메라를 향하도록 (옵션)
        transform.forward = Camera.main.transform.forward;
    }

    public void Addstamina(int extrastamina)
    {
        Debug.Log(extrastamina + "만큼 줄어들었습니다.");
        currentStamina += extrastamina;
    }
}
