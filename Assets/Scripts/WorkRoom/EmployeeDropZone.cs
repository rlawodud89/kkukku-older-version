using UnityEngine.EventSystems;
using UnityEngine;

public class EmployeeDropZone : MonoBehaviour
{
    public Employee employee;

    public void OnDropFromDrag(ItemDrag draggedItem)
    {
        Debug.Log("직원에게 직접 드롭 처리됨");
        if (draggedItem != null)
        {
            employee.GiveItem(draggedItem.itemData);
            draggedItem.MarkAsDropped();

            // 드래그 성공한 경우에만 제거
            Destroy(draggedItem.gameObject);
        }
    }


}
