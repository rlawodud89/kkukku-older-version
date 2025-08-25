using UnityEngine;

public class ColliderToggleOnEvent : MonoBehaviour
{
    [Header("채널 연결")]
    public VoidEventChannelSO onAppearChannel;
    public VoidEventChannelSO onCompleteChannel;

    [Header("토글 대상")]
    public GameObject target;   // 필요시
    public Collider col3D;      // 3D면 연결
    public Collider2D col2D;    // 2D면 연결

    private void OnEnable()
    {
        if (onAppearChannel != null) onAppearChannel.OnRaised += TurnOn;
        if (onCompleteChannel != null) onCompleteChannel.OnRaised += TurnOff;
        Debug.Log($"[Listener:{name}] Subscribed. appear={onAppearChannel?.name}, complete={onCompleteChannel?.name}, " +
                  $"has3D={(col3D != null)}, has2D={(col2D != null)}, target={(target ? target.name : "null")}");
    }

    private void OnDisable()
    {
        if (onAppearChannel != null) onAppearChannel.OnRaised -= TurnOn;
        if (onCompleteChannel != null) onCompleteChannel.OnRaised -= TurnOff;
    }

    private void TurnOn()
    {
        Debug.Log($"[Listener:{name}] TurnOn()");
        if (target) target.SetActive(true);
        if (col3D) col3D.enabled = true;
        if (col2D) col2D.enabled = true;
    }

    private void TurnOff()
    {
        Debug.Log($"[Listener:{name}] TurnOff()");
        if (target) target.SetActive(false);
        if (col3D) col3D.enabled = false;
        if (col2D) col2D.enabled = false;
    }
}
