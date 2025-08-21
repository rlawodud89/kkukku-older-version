using UnityEngine;

[CreateAssetMenu(fileName = "NewLetter", menuName = "ScriptableObject/LetterScript")]
public class LetterScript : ScriptableObject
{
    public string letterName;
    public Sprite image;
    public string message;
}