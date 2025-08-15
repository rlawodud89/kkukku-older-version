using UnityEngine;

public class FishingRodSwing : MonoBehaviour
{
    public float normalBobHeight = 1f;
    public float normalBobSpeed = 3f;
    public float actionBobHeight = 2f;
    public float actionBobSpeed = 5f;

    private float currentBobHeight;
    private float currentBobSpeed;
    private Vector3 startPos;

    public FishingController fishingcontroller;
    public FishingMiniGame fishingminigame;

    void Start()
    {
        startPos = transform.localPosition;

        if (fishingcontroller == null)
            fishingcontroller = FindObjectOfType<FishingController>();

        if (fishingminigame == null)
            fishingminigame = FindObjectOfType<FishingMiniGame>();

        currentBobHeight = normalBobHeight;
        currentBobSpeed = normalBobSpeed;
    }

    void Update()
    {
        if (!fishingcontroller.fishing_start) return;

        // 상태에 따라 세팅 변경
        if (fishingminigame != null && fishingminigame.miniGameRunning)
        {
            currentBobHeight = actionBobHeight;
            currentBobSpeed = actionBobSpeed;
        }
        else
        {
            currentBobHeight = normalBobHeight;
            currentBobSpeed = normalBobSpeed;
        }

        // 위아래로 움직이기
        float offset = Mathf.Sin(Time.time * currentBobSpeed) * currentBobHeight;
        transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}
