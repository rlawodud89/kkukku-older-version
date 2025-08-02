using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[CreateAssetMenu(fileName = "NewLetter", menuName = "Letter")]
public class LetterSO : ScriptableObject
{
    public string title;         // 편지 제목
    [TextArea(5, 10)]
    public string content;       // 편지 본문
    public Sprite sleepingImage;  // 잠자는 이미지
}
