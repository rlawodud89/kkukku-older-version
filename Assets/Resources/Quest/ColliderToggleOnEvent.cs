using UnityEngine;

public class ColliderToggleOnEvent : MonoBehaviour
{
    [Header("채널 연결(같은 에셋을 양쪽에서 참조해야 함)")]
    public VoidEventChannelSO onAppearChannel;
    public VoidEventChannelSO onCompleteChannel;

    [Header("토글 대상(비워두면 자기 자신/자식에서 자동 탐색)")]
    public GameObject target;
    public Collider col3D;
    public Collider2D col2D;

    [Header("옵션")]
    [Tooltip("OnEnable 때 자동으로 콜라이더를 찾아 바인딩합니다.")]
    public bool autoBindOnEnable = true;

    void Awake()
    {
        // target 비어있으면 기본은 자기 자신
        if (!target) target = gameObject;
    }

    void OnEnable()
    {
        if (autoBindOnEnable) AutoBind();

        if (onAppearChannel != null) onAppearChannel.OnRaised += TurnOn;
        if (onCompleteChannel != null) onCompleteChannel.OnRaised += TurnOff;

        Debug.Log($"[Listener:{name}] Subscribed."
            + $" appearID={(onAppearChannel ? onAppearChannel.GetInstanceID() : 0)}"
            + $", completeID={(onCompleteChannel ? onCompleteChannel.GetInstanceID() : 0)}"
            + $", has3D={(col3D != null)}, has2D={(col2D != null)}"
            + $", target={(target ? target.name : "null")}");
    }

    void OnDisable()
    {
        if (onAppearChannel != null) onAppearChannel.OnRaised -= TurnOn;
        if (onCompleteChannel != null) onCompleteChannel.OnRaised -= TurnOff;
    }

    void AutoBind()
    {
        // 1) 대상 오브젝트 밑에서 우선 탐색
        if (target)
        {
            if (!col2D) col2D = target.GetComponent<Collider2D>() ?? target.GetComponentInChildren<Collider2D>(true);
            if (!col3D) col3D = target.GetComponent<Collider>() ?? target.GetComponentInChildren<Collider>(true);
        }
        // 2) 그래도 없으면 자기 자신/자식에서 탐색
        if (!col2D) col2D = GetComponent<Collider2D>() ?? GetComponentInChildren<Collider2D>(true);
        if (!col3D) col3D = GetComponent<Collider>() ?? GetComponentInChildren<Collider>(true);

        if (!col2D && !col3D)
            Debug.LogWarning($"[Listener:{name}] Collider를 찾지 못했습니다. target='{(target ? target.name : "null")}'", this);
    }

    void TurnOn()
    {
        Debug.Log($"[Listener:{name}] TurnOn()");
        if (col2D) col2D.enabled = true;
        if (col3D) col3D.enabled = true;
    }

    void TurnOff()
    {
        Debug.Log($"[Listener:{name}] TurnOff()");
        if (col2D) col2D.enabled = false;
        if (col3D) col3D.enabled = false;
    }
}
