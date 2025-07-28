using UnityEngine;
using UnityEngine.UI;

public class ProgressCircle : MonoBehaviour
{
    public Image fillImage;
    public Image Image;
    public Image completeImage;
    public float maxProgress = 100f;
    public float currentProgress = 100f;
    public float time = 5f;
    
    void Update()
    {
        // 예시로 1초에 time씩 감소
        currentProgress -= Time.deltaTime * time;
        currentProgress = Mathf.Clamp(currentProgress, 0, maxProgress);

        fillImage.fillAmount = currentProgress / maxProgress;


        if (fillImage.fillAmount==0)
        {
            fillImage.gameObject.SetActive(false);
            Image.gameObject.SetActive(false);
            completeImage.gameObject.SetActive(true);
            
        }
        // 항상 카메라를 향하도록 (옵션)
        transform.forward = Camera.main.transform.forward;
    }
}
