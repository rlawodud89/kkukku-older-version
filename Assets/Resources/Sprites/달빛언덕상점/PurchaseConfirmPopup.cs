using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PurchaseConfirmPopup : MonoBehaviour
{
    [SerializeField] TMP_Text messageText;
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;

    Action onYes;

    void Awake()
    {
        yesBtn.onClick.AddListener(() => { onYes?.Invoke(); Hide(); });
        noBtn.onClick.AddListener(Hide);
    }

    // 기존: ItemCard 용
    public void Show(ItemCard card, Action yesCallback)
    {
        onYes = yesCallback;
        string msg = card.IsRecruit
            ? $"{card.Data.displayName}\n구매하시겠습니까?"
            : $"{card.Data.displayName} x{card.Quantity}\n구매하시겠습니까?";
        messageText.text = msg;
        ShowInternal();
    }

    // 새로 추가: 자유 메시지
    public void ShowMessage(string message, Action yesCallback)
    {
        onYes = yesCallback;
        messageText.text = message;
        ShowInternal();
    }

    void ShowInternal()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        transform.SetAsLastSibling();             // 항상 맨 위
    }

    void Hide() => gameObject.SetActive(false);
}
