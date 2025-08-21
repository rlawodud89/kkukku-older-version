using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BlanketManager : MonoBehaviour
{
    public static BlanketManager Instance;

    public List<ItemScript> blanketList = new List<ItemScript>();
    public Dictionary<string, ItemScript> blanketDict;

    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        gameManager = GameManager.getInstance();
        DontDestroyOnLoad(gameObject);
        LoadBlankets();
    }

    private void LoadBlankets()
    {
        //var loadedBlankets = Addressables.LoadAssetsAsync<ItemScript>("blanket", null)
        //    .WaitForCompletion();

        //blanketDict = loadedBlankets.ToDictionary(i => i.itemName);
        //blanketList = blanketDict.Values.ToList();

        //Debug.Log($"[BlanketManager] {blanketList.Count}개의 이불 로드 완료");

        blanketList = gameManager.Get_Current_BlanketDesign();
        blanketDict = blanketList.ToDictionary(i => i.itemName);
    }

}
