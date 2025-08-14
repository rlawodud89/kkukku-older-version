using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopController1 : MonoBehaviour
{
    [Header("업그레이드 버튼들")]
    [SerializeField] Button loomButton;        // 직조기 업그레이드
    [SerializeField] Button fillerButton;      // 충진기 업그레이드
    [SerializeField] Button decoTableButton;   // 장식 테이블 업그레이드

    [Header("레벨 텍스트들")]
    [SerializeField] TMP_Text loomLevelText;        
    [SerializeField] TMP_Text fillerLevelText;     
    [SerializeField] TMP_Text decoLevelText;   

    [Header("팝업")]
    [SerializeField] PurchaseConfirmPopup popup;

    [Header("가격 설정")]
    [SerializeField] int loomPrice = 3000;
    [SerializeField] int fillerPrice = 5000;
    [SerializeField] int decoTablePrice = 8000;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.getInstance();
        loomButton.onClick.AddListener(OnLoomClick);
        fillerButton.onClick.AddListener(OnFillerClick);
        decoTableButton.onClick.AddListener(OnDecoTableClick);

        loomLevelText.text = "Lv. " + gameManager.Get_LoomLevel();
        fillerLevelText.text = "Lv. " + gameManager.Get_FillerLevel();
        decoLevelText.text = "Lv. " + gameManager.Get_DecoLevel();
    }

    void OnLoomClick()
    {
        if (!CanBuy(loomPrice))
        {
            Debug.Log("재화 부족");
            return;
        }

        popup.ShowMessage(
            $"직조기 업그레이드 (가격 {loomPrice} G)\n진행하시겠습니까?",
            DoLoomUpgrade);
    }

    void OnFillerClick()
    {
        if (!CanBuy(fillerPrice))
        {
            Debug.Log("재화 부족");
            return;
        }

        popup.ShowMessage(
            $"충진기 업그레이드 (가격 {fillerPrice} G)\n진행하시겠습니까?",
            DoFillerUpgrade);
    }

    void OnDecoTableClick()
    {
        if (!CanBuy(decoTablePrice))
        {
            Debug.Log("재화 부족");
            return;
        }

        popup.ShowMessage(
            $"장식 테이블 업그레이드 (가격 {decoTablePrice} G)\n진행하시겠습니까?",
            DoDecoTableUpgrade);
    }

    void DoLoomUpgrade()
    {
        gameManager.Change_Gold(-loomPrice);
        gameManager.Change_LoomLevel(1);
        loomLevelText.text = "Lv. " + gameManager.Get_LoomLevel().ToString();
        Debug.Log("직조기 업그레이드 완료!");
    }

    void DoFillerUpgrade()
    {
        gameManager.Change_Gold(-fillerPrice);
        gameManager.Change_FillerLevel(1);
        fillerLevelText.text = "Lv. " + gameManager.Get_FillerLevel().ToString();
        Debug.Log("충진기 업그레이드 완료!");
    }

    void DoDecoTableUpgrade()
    {
        gameManager.Change_Gold(-decoTablePrice);
        gameManager.Change_DecoLevel(1);
        decoLevelText.text = "Lv. " + gameManager.Get_DecoLevel().ToString();
        Debug.Log("장식 테이블 업그레이드 완료!");
    }

    private bool CanBuy(int value)
    {
        if (value <= gameManager.Get_Gold()) return true;
        else return false;
    }
}
