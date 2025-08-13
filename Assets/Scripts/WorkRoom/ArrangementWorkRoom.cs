using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.SceneManagement;

public class ArrangementWorkRoom : MonoBehaviour
{
    // 게임 메니저
    private GameManager gameManager;

    // 현재 작업실에 설치된 인테리어 정보
    private List<(InteriorScript item, int x, int y)> installedInteriors=new List<(InteriorScript, int, int)>();

    // 아이템 생성되는 곳
    private Transform itemParent;

    // 테스트용
    public InteriorScript furnitureItem;
    //public InteriorScript workerItem;
    //public InteriorScript tileItem;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameManager.getInstance();

        if(gameManager == null)
        {
            Debug.LogError("GameManager instance not found!");
            return;
        }

        itemParent = GameObject.Find("Pixels")?.transform;

        //installedInteriors=gameManager.Get_Current_RoomInterior();
        installedInteriors = gameManager?.Get_Current_RoomInterior() 
                         ?? new List<(InteriorScript item, int x, int y)>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        // 씬이 로드될 때 호출될 콜백 함수 등록
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 씬이 언로드될 때 콜백 함수 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때 실행할 코드 작성 (예: 특정 오브젝트 활성화, 데이터 로드 등)
        if (scene.name == "Work_Room") // 특정 씬에서만 동작하도록 조건 추가 가능
        {
            Debug.Log("Work_Room loaded!");
            // 특정 오브젝트 활성화 등 원하는 작업 수행

            // 데베에서 가져오기
            

            // 테스트용
            //installedInteriors.Add((furnitureItem, -4, 2));

            
            if(installedInteriors==null)
            {
                Debug.Log("No installed interiors found.");
                return;
            }

            foreach (var (item, x, y) in installedInteriors)
            {
                Instantiate(item.prefab, new Vector3(x, y, 20), item.prefab.transform.rotation, itemParent);
                Debug.Log($"Installed Interior: {item.name} at ({x}, {y})");
            }
        }
    }

}
