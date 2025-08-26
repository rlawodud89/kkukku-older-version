using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class SheepClickBlocker : MonoBehaviour
{
    public Make_Cotton makeCotton; // Inspector에서 연결
    private PolygonCollider2D col;

    void Start()
    {
        col = GetComponent<PolygonCollider2D>();
        if (makeCotton == null)
        {
            makeCotton = FindObjectOfType<Make_Cotton>();
        }
    }

    void Update()
    {
        if (makeCotton != null)
        {
           // col.enabled = !makeCotton.isMaking;
        }
    }
}
