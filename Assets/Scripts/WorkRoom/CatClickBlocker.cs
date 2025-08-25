using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class CatClickBlocker : MonoBehaviour
{
    public Make_Sewing makeSewing; // Inspector에서 연결
    private PolygonCollider2D col;

    void Start()
    {
        col = GetComponent<PolygonCollider2D>();
        if (makeSewing == null)
        {
            makeSewing = FindObjectOfType<Make_Sewing>();
        }
    }

    void Update()
    {
        if (makeSewing != null)
        {
            col.enabled = !makeSewing.isMaking;
        }
    }
}
