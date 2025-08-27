using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class DraggableObject : MonoBehaviour
{
    public Tilemap tilemap;          
    private Vector3 offset;
    private bool dragging = false;
    private Camera cam;

    private InteriorManager interiorManager;

    void Start()
    {
        //cam = Camera.main;
        //interiorManager = FindObjectOfType<InteriorManager>();
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
        cam = Camera.main;
        interiorManager = FindObjectOfType<InteriorManager>();
    }

    void OnMouseDown()
    {
        if (!interiorManager.interiorMode || interiorManager.tileMode)
            return;

        dragging = true;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        offset = transform.position - mouseWorld;

    }

    void OnMouseUp()
    {
        if (!interiorManager.interiorMode)
            return;

        dragging = false;
    }

    void Update()
    {
        if (tilemap == null)
        {
            tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        }

        if (dragging)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            Vector3 dragPos = mouseWorld + offset;


            Vector3Int cellPos = tilemap.WorldToCell(dragPos);
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);

            transform.position = cellCenter;
        }
    }
}
