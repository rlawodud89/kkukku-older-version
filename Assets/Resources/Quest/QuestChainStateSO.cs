// QuestChainStateSO.cs
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestChainState", menuName = "Quest/QuestChainState")]
public class QuestChainStateSO : ScriptableObject
{
    [SerializeField] private int _currentIndex = 0;     // 현재 단계(0..N-1). 끝난 뒤엔 러너가 steps.Count로 셋할 수도.
    [SerializeField] private bool _chainCompleted = false;

    public int CurrentIndex => _currentIndex;
    public bool ChainCompleted => _chainCompleted;

    public event Action<int> OnIndexChanged;     // newIndex
    public event Action<bool> OnChainCompletedChanged;

    public void SetIndex(int newIndex)
    {
        if (_currentIndex == newIndex) return;
        _currentIndex = newIndex;
        if (Application.isPlaying) OnIndexChanged?.Invoke(_currentIndex);
    }

    public void SetChainCompleted(bool v)
    {
        if (_chainCompleted == v) return;
        _chainCompleted = v;
        if (Application.isPlaying) OnChainCompletedChanged?.Invoke(_chainCompleted);
    }

#if UNITY_EDITOR
    // Play 중 인스펙터에서 값을 바꿔도 이벤트가 터지도록
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            OnIndexChanged?.Invoke(_currentIndex);
            OnChainCompletedChanged?.Invoke(_chainCompleted);
        }
    }
#endif
}
