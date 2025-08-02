using UnityEngine;

[CreateAssetMenu(fileName = "NewWorker", menuName = "ScriptableObject/WorkerScript")]
public class WorkerScript : ScriptableObject
{
    public string workerName;
    public Sprite image;
    public WorkType workType;
}