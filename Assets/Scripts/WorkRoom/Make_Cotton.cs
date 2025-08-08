using UnityEngine;
using UnityEngine.UI;
public class Make_Cotton : MonoBehaviour
{
    public static Make_Cotton Instance { get; private set; }

    public GameObject cottonPanel;
    public SewingPanel sewingPanel;

    public Employee Employee2;
    public ProgressCircle progresscircle;

    public GameObject BallonPanel;
    public Button CottonButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 중복 방지
            return;
        }

        Instance = this;
    }

    public void HandleMakeClicked(ItemScript currentBlanket)
    {
        Debug.Log("Make_Cotton에서 Make 버튼 클릭됨 감지!");

        cottonPanel.SetActive(false);   
        Employee2.Working();

        progresscircle.OnComplete = () =>
        {
            Debug.Log("complete");
            showcotton(currentBlanket);
        };

        progresscircle.CompleteCircle();


    }

    void showcotton(ItemScript currentBlanket)
    {
        if (currentBlanket != null)
        {
            Debug.Log(currentBlanket.itemName + "솜 넣은 모습");

            BallonPanel.SetActive(true);
            CottonButton.gameObject.SetActive(true);
            //CottonButton.image.sprite = currentBlanket.Cotton;

            CottonButton.onClick.RemoveAllListeners();
            CottonButton.onClick.AddListener(() =>
            {
                //currentBlanket.CottonCount += 1;
                //sewingPanel?.SetSelectedBlanket(currentBlanket);

                BallonPanel.SetActive(false);
                CottonButton.gameObject.SetActive(false);
                progresscircle.ProgressInit();

            });

        }
        else
        {
            Debug.Log("null");
        }
    }
}
