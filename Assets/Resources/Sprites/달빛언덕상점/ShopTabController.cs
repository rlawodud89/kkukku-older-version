// ShopTabController.cs
using UnityEngine;
using UnityEngine.UI;

public class ShopTabController : MonoBehaviour
{
    [Header("메인 메뉴 패널")]
    [SerializeField] GameObject firstPanel;        // First_Panel

    [Header("Buttons")]
    [SerializeField] Button materialShopButton;     // 재료상점버튼
    [SerializeField] Button upgradeShopButton;      // 업그레이드상점버튼
    [SerializeField] Button recruitShopButton;      // 고용상점버튼

    [Header("Panels")]
    [SerializeField] GameObject materialShopPanel;  // 재료상점 패널
    [SerializeField] GameObject upgradeShopPanel;   // 업그레이드상점 패널
    [SerializeField] GameObject recruitShopPanel;   // 고용상점 패널

    [Header("공용 Back 버튼")]
    [SerializeField] Button backButton;

    void Awake()
    {
        // ▶ 메뉴 → 상점 패널
        materialShopButton.onClick.AddListener(() => OpenShop(materialShopPanel));
        upgradeShopButton.onClick.AddListener(() => OpenShop(upgradeShopPanel));
        recruitShopButton.onClick.AddListener(() => OpenShop(recruitShopPanel));

        // ▶ 상점 → 메뉴
        backButton.onClick.AddListener(BackToMenu);

        // 처음엔 Back 버튼을 숨겨둡니다.
        backButton.gameObject.SetActive(false);
    }

    /// <summary>메뉴 숨기고 target 상점 패널만 표시, Back 버튼 ON</summary>
    void OpenShop(GameObject target)
    {
        firstPanel.SetActive(false);
        backButton.gameObject.SetActive(true);

        materialShopPanel.SetActive(target == materialShopPanel);
        upgradeShopPanel.SetActive(target == upgradeShopPanel);
        recruitShopPanel.SetActive(target == recruitShopPanel);
    }

    /// <summary>모든 상점 패널 OFF, 메뉴 표시, Back 버튼 OFF</summary>
    void BackToMenu()
    {
        firstPanel.SetActive(true);
        backButton.gameObject.SetActive(false);

        materialShopPanel.SetActive(false);
        upgradeShopPanel.SetActive(false);
        recruitShopPanel.SetActive(false);
    }
}
