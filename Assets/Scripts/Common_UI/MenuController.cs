using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


public class MenuController : MonoBehaviour
{
    public GameObject menuButton; // MenuItems 오브젝트
    public List<GameObject> menuButtons; // 메뉴 버튼들
    public float spacing = 60f;          // 버튼 사이 간격
    public float delay = 0.05f;          // 애니메이션 간 딜레이
    public float fadeTime = 0.2f;        // 투명도 애니메이션 시간
    private bool isMenuOpen = false;

    public Sprite openSprite;     // 클릭 후 보여줄 이미지
    public Image targetImage;       // 현재 버튼의 이미지
    public Sprite closeSprite;   // 원래 이미지 저장
    private bool isOpen = true;
    private bool isRotating = false;


    public GameObject mapPanel; 
    
    public GameObject energy;
    public GameObject energyLevel;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void ToggleMenuItems()
    {
        StopAllCoroutines();

        if (isMenuOpen)
            StartCoroutine(HideMenu());
        else
            StartCoroutine(ShowMenu());

        isMenuOpen = !isMenuOpen;
    }

    IEnumerator ShowMenu()
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            RectTransform baseRT = menuButton.GetComponent<RectTransform>();
            Vector2 basePos = baseRT.anchoredPosition;

            GameObject btn = menuButtons[i];
            RectTransform rt = btn.GetComponent<RectTransform>();
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();

            btn.SetActive(true);
            rt.anchoredPosition = basePos;
            cg.alpha = 0;

            // 위치 이동
            Vector2 targetPos = basePos + new Vector2(0, -spacing * (i + 1));
            StartCoroutine(SlideTo(rt, targetPos, fadeTime));
            // 투명도 애니메이션
            StartCoroutine(FadeTo(cg, 1f, fadeTime));

            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator HideMenu()
    {
        for (int i = menuButtons.Count - 1; i >= 0; i--)
        {
            GameObject btn = menuButtons[i];
            RectTransform rt = btn.GetComponent<RectTransform>();
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();

            StartCoroutine(SlideTo(rt, menuButton.GetComponent<RectTransform>().anchoredPosition, fadeTime));
            StartCoroutine(FadeTo(cg, 0f, fadeTime));

            yield return new WaitForSeconds(delay);
            btn.SetActive(false);
        }
    }

    IEnumerator FadeTo(CanvasGroup cg, float targetAlpha, float duration)
    {
        float start = cg.alpha;
        float time = 0f;
        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(start, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = targetAlpha;
    }

    IEnumerator SlideTo(RectTransform rt, Vector2 targetPos, float duration)
    {
        Vector2 start = rt.anchoredPosition;
        float time = 0f;
        while (time < duration)
        {
            rt.anchoredPosition = Vector2.Lerp(start, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = targetPos;
    }

    public GameObject storeSignPopup;

    public void ShowStoreSignPopup()
    {
        if (storeSignPopup != null)
        {
            storeSignPopup.SetActive(true);
        }
        else
        {
            Debug.LogError("Store Sign Popup is not assigned in the inspector.");
        }

        TextMeshProUGUI confirmText = storeSignPopup.GetComponentInChildren<TextMeshProUGUI>();
        if (isOpen)
        {
            if (confirmText != null)
            {
                confirmText.text = $"게게를 여시겠습니까?";

                Button yesButton = storeSignPopup.transform.Find("YesButton").GetComponent<Button>();
                yesButton.onClick.AddListener(() => { ChangeImage(); storeSignPopup.SetActive(false); });

                // No 버튼 클릭 시 팝업 닫기
                Button noButton = storeSignPopup.transform.Find("NoButton").GetComponent<Button>();
                noButton.onClick.AddListener(() => { storeSignPopup.SetActive(false); });

            }
            else
            {
                Debug.LogError("Text 컴포넌트가 storeSignPopup 안에 없습니다.");
            }
        }
        else
        {
            if (confirmText != null)
            {
                confirmText.text = $"게게를 닫으시겠습니까?";
                
                Button yesButton = storeSignPopup.transform.Find("YesButton").GetComponent<Button>();
                yesButton.onClick.AddListener(() => { ChangeImage(); storeSignPopup.SetActive(false); });

                // No 버튼 클릭 시 팝업 닫기
                Button noButton = storeSignPopup.transform.Find("NoButton").GetComponent<Button>();
                noButton.onClick.AddListener(() => { storeSignPopup.SetActive(false); });
            }
            else
            {
                Debug.LogError("Text 컴포넌트가 storeSignPopup 안에 없습니다.");
            }
        }
    }

    public void ChangeImage()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateAndSwap());
        }
    }

    IEnumerator RotateAndSwap()
    {
        isRotating = true;

        float duration = 0.5f;
        float time = 0f;

        RectTransform rt = targetImage.GetComponent<RectTransform>();
        float startY = 0f;
        float endY = 90f;

        // 1단계: 0 → 90도 회전
        while (time < duration)
        {
            float angle = Mathf.Lerp(startY, endY, time / duration);
            rt.localEulerAngles = new Vector3(0, angle, 0);
            time += Time.deltaTime;
            yield return null;
        }

        rt.localEulerAngles = new Vector3(0, 90f, 0);

        // 이미지 교체
        isOpen = !isOpen;
        targetImage.sprite = isOpen ? openSprite : closeSprite;

        // 2단계: 90 → 180도 회전
        time = 0f;
        startY = 270f;
        endY = 360f;

        while (time < duration)
        {
            float angle = Mathf.Lerp(startY, endY, time / duration);
            rt.localEulerAngles = new Vector3(0, angle, 0);
            time += Time.deltaTime;
            yield return null;
        }

        rt.localEulerAngles = new Vector3(0, 0, 0); // 180도 대신 0으로 복원
        isRotating = false;
    }


    public void MapPanelClose()
    {
        if (mapPanel != null)
            mapPanel.SetActive(false);
    }

    public void MapPanelOpen()
    {
        if (mapPanel != null)
            mapPanel.SetActive(true);

    }

    

    public void SeeEnergyLevel()
    {
        if (energyLevel.activeSelf)
        {
            energyLevel.SetActive(false);
            energy.SetActive(true);
        }
        else
        {
            energyLevel.SetActive(true);
            energy.SetActive(false);
        }   
    }

    public GameObject confirmPopup;

    public void OnPlaceClicked(GameObject clickedObject){
        string placeName = clickedObject.tag;
        ShowConfirmPopup(placeName);
    }

    private void ShowConfirmPopup(string placeName)
    {
        confirmPopup.SetActive(true);

        TextMeshProUGUI confirmText = confirmPopup.GetComponentInChildren<TextMeshProUGUI>();
        if (confirmText != null)
        {
            confirmText.text = $"{placeName}에 가시겠습니까?";
        }else
        {
            Debug.LogError("Text 컴포넌트가 confirmPopup 안에 없습니다.");
        }
        

        // Yes 버튼 클릭 시 이동
        Button yesButton = confirmPopup.transform.Find("YesButton").GetComponent<Button>();
        yesButton.onClick.AddListener(() => { OnConfirm(placeName); });

         // No 버튼 클릭 시 팝업 닫기
         Button noButton = confirmPopup.transform.Find("NoButton").GetComponent<Button>();
        noButton.onClick.AddListener(() => { confirmPopup.SetActive(false); });


    }

    private void OnConfirm(string placeName)
    {
        // 태그에 맞는 씬으로 이동
        switch (placeName)
        {
            case "달빛언덕":
                SceneManager.LoadScene("Moonlight_Hill");
                break;
            case "수면정원":
                SceneManager.LoadScene("Sleeping_Garden");
                break;
            case "구름연못":
                SceneManager.LoadScene("Cloud_Pond");
                break;
            case "마을":
                SceneManager.LoadScene("Village");
                break;
            case "이불가게":
                SceneManager.LoadScene("Blanket_Shop");
                break;
            default:
                Debug.LogError("이동할 장소 씬이 없습니다.");
                Debug.LogError($"Unknown place name: {placeName}");
                break;
        }
    }
}
