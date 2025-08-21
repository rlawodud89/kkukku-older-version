using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class FoxClickBlocker : MonoBehaviour
{
    public Make_Fabric makeFabric; // Inspector에서 연결
    private PolygonCollider2D col;

    void Start()
    {

        col = GetComponent<PolygonCollider2D>();

        if (makeFabric == null)
        {
            makeFabric = FindObjectOfType<Make_Fabric>();
        }
    }
    void Update()
    {
        if (makeFabric != null)
        {
            col.enabled = !makeFabric.isMaking;
        }
    }
}
