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

    private ItemScript currentYarn;
    private ItemScript currentCotton;

    public GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.getInstance();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 중복 방지
            return;
        }

        Instance = this;
    }

    public void HandleMakeClicked(ItemScript currentYarn)
    {
        Debug.Log("Make_Cotton에서 Make 버튼 클릭됨 감지!");
        gameManager.Add_InventoryItem(currentYarn.itemName, -1);

        currentCotton = gameManager.Yarn_to_Cotton(currentYarn.itemName);

        cottonPanel.SetActive(false);   
        Employee2.Working();

        progresscircle.OnComplete = () =>
        {
            gameManager.Add_InventoryItem(currentCotton.itemName, 1);
            Debug.Log("complete");
            showcotton();
        };

        progresscircle.CompleteCircle();


    }

    void showcotton()
    {
        if (currentCotton != null)
        {

            BallonPanel.SetActive(true);
            CottonButton.gameObject.SetActive(true);
            CottonButton.image.sprite =currentCotton.image;

            CottonButton.onClick.RemoveAllListeners();
            CottonButton.onClick.AddListener(() =>
            {
                //currentBlanket.CottonCount += 1;
                

                sewingPanel.currentSewing = currentCotton;
                sewingPanel?.SetSelectedBlanket();

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
