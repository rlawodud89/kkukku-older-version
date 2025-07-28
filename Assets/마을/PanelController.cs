using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    // Start is called before the first frame update
    public Button showButton;
    public Button hideButton;
    public GameObject panel;

    void Start()
    {
        showButton.onClick.AddListener(() => panel.SetActive(true));
        hideButton.onClick.AddListener(() => panel.SetActive(false));
    }
}
