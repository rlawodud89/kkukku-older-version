// FoxClickBlocker.cs

using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class FoxClickBlocker : MonoBehaviour
{
    public Make_Fabric makeFabric;
    private PolygonCollider2D col;

    void Start()
    {
        col = GetComponent<PolygonCollider2D>();

        if (makeFabric == null)
        {
            // FindObjectOfType으로 Make_Fabric 인스턴스를 찾음
            makeFabric = FindObjectOfType<Make_Fabric>();
        }
    }

    void Update()
    {
        if (makeFabric != null)
        {
            // Make_Fabric의 isMaking 상태에 따라 이 객체의 콜라이더를 제어
            //col.enabled = !makeFabric.isMaking;
        }
    }
}