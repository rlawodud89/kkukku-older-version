using UnityEngine;
using UnityEngine.EventSystems;

public class EmployeeDropZone : MonoBehaviour, IDropHandler
{
    public Employee employee;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("직원에게 직접 드롭 처리됨");

        ItemDrag draggedItem = eventData.pointerDrag?.GetComponent<ItemDrag>();

        if (draggedItem != null && draggedItem.itemData != null)
        {
            // 아이템 지급
            employee.GiveItem(draggedItem.itemData);

            // 드롭 성공 표시 (dragClone 삭제는 ItemDrag가 알아서 함)
            draggedItem.MarkAsDropped();

            // 퀘스트
            AddQuestProcess.Instance.AddProcessToQuest("직원에게 간식주기");

        }
    }
}
