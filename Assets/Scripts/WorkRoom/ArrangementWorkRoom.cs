using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ArrangementWorkRoom : MonoBehaviour
{
    public Make_Fabric make_Fabric;
    public Make_Cotton make_Cotton;
    public Make_Sewing make_Sewing;
    public SnacksInventory snackInventory;
    public ColliderToggleOnEvent colliderToggleOnEvent;

    // 게임 메니저
    private GameManager gameManager;

    // 현재 작업실에 설치된 인테리어 정보
    private List<(InteriorScript item, float x, float y)> installedInteriors = new List<(InteriorScript, float, float)>();

    // 아이템 생성되는 곳
    private Transform itemParent;


    void Awake()
    {
        gameManager = GameManager.getInstance();

        if (gameManager == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("GameManager instance not found! Creating temporary one for test scene.");
            GameObject gm = new GameObject("GameManager");
            gameManager = gm.AddComponent<GameManager>();
#else
        Debug.LogError("GameManager instance not found!");
        return;
#endif
        }

        //itemParent = GameObject.Find("Pixels")?.transform;

        //installedInteriors = gameManager.Get_Current_RoomInterior();

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 씬이 언로드될 때 콜백 함수 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameManager == null) gameManager = GameManager.getInstance();

        itemParent = GameObject.Find("Pixels")?.transform;
        installedInteriors = gameManager.Get_Current_RoomInterior();

        // 씬이 로드될 때 실행할 코드 작성 (예: 특정 오브젝트 활성화, 데이터 로드 등)
        if (scene.name == "Work_Room") // 특정 씬에서만 동작하도록 조건 추가 가능
        {
            if (installedInteriors == null)
            {
                Debug.Log("No installed interiors found.");
                return;
            }

            foreach (var (item, x, y) in installedInteriors)
            {
                var go = Instantiate(item.prefab, new Vector3(x, y, 20), item.prefab.transform.rotation);

                if (item.interiorType == InteriorType.WORKER)
                {
                    Employee employee = go.GetComponent<Employee>();
                    (int workerID, int stamina, DateTime startTime, ItemScript workItem, float workingPercent) = gameManager.Get_Worker_Info(x, y);
                    employee.EmployeeID = workerID;
                    employee.staminar.currentStamina = stamina;
                    employee.workItem = workItem;
                    employee.workingPercent = workingPercent;
                    employee.snacksInventory = snackInventory;

                    if (item.workType == WorkType.FABRIC)
                    {
                        make_Fabric.Add_Employee(employee, employee.progressCircle);
                    }
                    else if (item.workType == WorkType.COTTON)
                    {
                        make_Cotton.Add_Employee(employee, employee.progressCircle);
                    }
                    else if (item.workType == WorkType.SEWING)
                    {
                        make_Sewing.Add_Employee(employee, employee.progressCircle);
                    }
                }
                else if (item.interiorName == "특별제작대")
                {
                    colliderToggleOnEvent.target = go;
                    Collider2D coll = go.GetComponent<Collider2D>();
                    colliderToggleOnEvent.col2D = coll;
                }



                //go.transform.SetParent(itemParent,true);
                //Debug.Log($"Installed Interior: {item.name} at ({x}, {y})");
            }
        }
    }

}
