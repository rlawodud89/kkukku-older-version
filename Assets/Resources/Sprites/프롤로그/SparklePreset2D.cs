// SparklePreset2D.cs
// 빈 GameObject에 붙이면 자동으로 2D 스파클 파티클을 구성합니다.
using UnityEngine;

[ExecuteAlways]
public class SparklePreset2D : MonoBehaviour
{
    [Header("스파클 기본")]
    public float rateOverTime = 20f;        // 중앙 밀도
    public float radius = 0.15f;            // 생성 반경(작게 = 중심 밀집)
    public float startSpeedMin = 0.5f;
    public float startSpeedMax = 1.5f;
    public float startSizeMin = 0.05f;
    public float startSizeMax = 0.15f;
    public float lifetimeMin = 0.5f;
    public float lifetimeMax = 1.0f;

    [Header("색/알파")]
    public Color startColor = Color.white;  // 살짝 노랑이면 더 반짝임
    public Gradient colorOverLife;

    [Header("정렬")]
    public string sortingLayerName = "Default";
    public int sortingOrder = 10;

    ParticleSystem ps;

    void OnEnable()
    {
        if (ps == null) Build();
        Apply();
    }

    void Reset()
    {
        // 기본 그라데이션(처음 불투명 → 끝 투명)
        var grad = new Gradient();
        grad.SetKeys(
            new[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLife = grad;
    }

    void Build()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null) ps = gameObject.AddComponent<ParticleSystem>();

        // 파티클 렌더러 세팅
        var pr = GetComponent<ParticleSystemRenderer>();
        pr.renderMode = ParticleSystemRenderMode.Billboard;
        pr.sortingLayerName = sortingLayerName;
        pr.sortingOrder = sortingOrder;

        // 머티리얼: Additive가 반짝임에 유리. 없으면 Sprites/Default로 대체.
        var shader = Shader.Find("Particles/Additive");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            var mat = new Material(shader);
            pr.material = mat;
        }
    }

    void Apply()
    {
        if (ps == null) return;

        // Main
        var main = ps.main;
        main.duration = 1.5f;
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World; // 오브젝트가 움직여도 흩어짐 유지
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetimeMin, lifetimeMax);
        main.startSpeed = new ParticleSystem.MinMaxCurve(startSpeedMin, startSpeedMax);
        main.startSize = new ParticleSystem.MinMaxCurve(startSizeMin, startSizeMax);
        main.startColor = startColor;

        // Emission: 밀도 조절
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = rateOverTime;

        // Shape: 거의 점에서만 생성되도록 작은 반경
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere; // 2D에선 원처럼 동작
        shape.radius = radius;
        shape.radiusThickness = 0f; // 중심에서만 생성

        // Velocity over Lifetime: 중심에서 바깥으로 퍼지게 + 살짝 회전감
        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.Local;
        vel.radial = new ParticleSystem.MinMaxCurve(0.6f, 1.0f);
        vel.orbitalX = 0f;
        vel.orbitalY = 0f;
        vel.orbitalZ = 0.15f; // 약간의 공전감(선택)

        // Size over Lifetime: 팍 반짝였다가 작아짐
        var size = ps.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve(
            new Keyframe(0f, 1.0f, 0f, -3f),
            new Keyframe(0.3f, 0.6f),
            new Keyframe(1f, 0f)
        );
        size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Color over Lifetime: 서서히 페이드아웃
        var col = ps.colorOverLifetime;
        col.enabled = true;
        col.color = new ParticleSystem.MinMaxGradient(colorOverLife);

        // Renderer 정렬 재적용
        var pr = GetComponent<ParticleSystemRenderer>();
        pr.sortingLayerName = sortingLayerName;
        pr.sortingOrder = sortingOrder;

        // 루프 시작
        if (!Application.isPlaying) ps.Simulate(0f, true, true);
        ps.Play();
    }

    // 런타임에 파라미터 바꾸면 즉시 반영하고 싶을 때 호출
    public void Refresh()
    {
        Apply();
    }
}
