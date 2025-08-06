using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemTree : MonoBehaviour
{
    public List<Button> items;

    public SnacksManager snacksManager;
    public SnacksInventory snacksInventory;
    private int count1;
    private static int MAXCOUNT = 5;
    private List<SnacksData> Snackslist;
    private Dictionary<Button, SnacksData> buttonToSnackMap = new Dictionary<Button, SnacksData>();

    // Start is called before the first frame update
    void Start()
    {

        if (snacksManager == null)
        {
            snacksManager = FindObjectOfType<SnacksManager>();
        }

        Snackslist = snacksManager.GetSnacksList();

        if (snacksInventory == null)
        {
            snacksInventory = FindObjectOfType<SnacksInventory>();
            Debug.Log("����");
        }

        //��ư � Ȱ��ȭ �� ����
        int randomCount = Random.Range(1, items.Count + 1);

        // �̹� Ȱ��ȭ�� ��ư �ε��� �����ϴ� ����Ʈ
        List<int> usedIndices = new List<int>();

        // ���� ����Ʈ�� �������� �з�
        List<SnacksData> level1Snacks = Snackslist.FindAll(s => s.level == 1);
        List<SnacksData> level2Snacks = Snackslist.FindAll(s => s.level == 2);
        List<SnacksData> level3Snacks = Snackslist.FindAll(s => s.level == 3);

        // Ȯ�� ����ġ ����
        int weight1 = 60;
        int weight2 = 30;
        int weight3 = 10;

        // ����
        int totalWeight = weight1 + weight2 + weight3;

        // �� ��ư�� ����
        for (int i = 0; i < randomCount; i++)
        {
            int buttonIndex;
            do
            {
                buttonIndex = Random.Range(0, items.Count);

            } while (usedIndices.Contains(buttonIndex));

            usedIndices.Add(buttonIndex);

            Button btn = items[buttonIndex];
            btn.gameObject.SetActive(true);

            // ���� ����ġ ����
            int rand = Random.Range(1, totalWeight + 1); // 1~100
            SnacksData selectedSnack = null;

            if (rand <= weight1 && level1Snacks.Count > 0)
            {
                selectedSnack = level1Snacks[Random.Range(0, level1Snacks.Count)];
            }
            else if (rand <= weight1 + weight2 && level2Snacks.Count > 0)
            {
                selectedSnack = level2Snacks[Random.Range(0, level2Snacks.Count)];
            }
            else if (level3Snacks.Count > 0)
            {
                selectedSnack = level3Snacks[Random.Range(0, level3Snacks.Count)];
            }
            else
            {
                // ���� ó��: ��� ������ ����ִ� ���
                selectedSnack = Snackslist[Random.Range(0, Snackslist.Count)];
            }

            // �̹��� ���� �� ����
            Sprite selectedSprite = selectedSnack.SnackSprite;
            items[buttonIndex].GetComponent<Image>().sprite = selectedSprite;

            Button btnCopy = btn;
            buttonToSnackMap[btnCopy] = selectedSnack;
            btn.onClick.AddListener(() => ClickItem(btnCopy));



        }
        count1 = 0;
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void ClickItem(Button clickedButton)
    {
        count1++;
        Debug.Log(count1);
        if (count1 == MAXCOUNT)
        {
            count1 = 0;

            if (buttonToSnackMap.TryGetValue(clickedButton, out SnacksData snack))
            {
                snacksInventory.AddSnack(snack, 1);
                Debug.Log("���� ȹ��: " + snack.name);
                clickedButton.gameObject.SetActive(false);
            }

        }
    }

}

