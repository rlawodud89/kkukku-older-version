using UnityEngine;

public class CanvasOrderDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Dump();
        }
    }

    void Dump()
    {
        var canvases = GameObject.FindObjectsOfType<Canvas>(true);
        System.Array.Sort(canvases, (a, b) => a.sortingOrder.CompareTo(b.sortingOrder));
        Debug.Log("---- Canvas Dump (lowest -> highest) ----");
        foreach (var c in canvases)
        {
            string mode = c.renderMode.ToString();
            string name = c.gameObject.name;
            string info =
                $"{name}  | mode={mode}" +
                $" | override={c.overrideSorting}" +
                $" | layer={c.sortingLayerName}" +
                $" | order={c.sortingOrder}" +
                $" | enabled={c.enabled}";
            Debug.Log(info, c);
        }
        Debug.Log("---- end dump ----");
    }
}
