using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBtn : MonoBehaviour
{
    public RectTransform targetRoot; // 흔들고 싶은 곳
    public GameObject hideObject;   // 흔드는 동안 숨길 오브젝트

    public float duration;        // 흔들리는 시간
    public float magnitudeX;     // 흔들림 강도 X축
    public float magnitudeY;     // 흔들림 강도 Y축
    public float changeInterval; // 랜덤 목표 위치 변경 주기 (커질수록 느려짐)
    public float lerpSpeed;     // Lerp 보간 속도 (작을수록 느려짐)

    public Action OnReset;

    private Vector3 originalPos;

    public void ClickResetBtn()
    {
        GameObject item1 = GameObject.Find("Item1");
        GameObject item2 = GameObject.Find("Item2");

        if (item1 == null && item2 == null) // 간식을 다 채집했을 때만 리셋 가능
        {
            originalPos = targetRoot.localPosition;
            StartCoroutine(Shake());
            OnReset?.Invoke();
        }
    }

    private IEnumerator Shake()
    {
        AudioManager.Instance.PlaySFX("running");  // 효과음

        float elapsed = 0f;

        if (hideObject != null)
            hideObject.SetActive(false);

        Vector3 targetPos = originalPos;
        float targetTimer = 0f;

        while (elapsed < duration)
        {
            targetTimer += Time.deltaTime;
            // 일정 주기마다 새로운 목표 랜덤 위치 지정
            if (targetTimer >= changeInterval)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * magnitudeX;
                float y = UnityEngine.Random.Range(-1f, 1f) * magnitudeY;
                targetPos = originalPos + new Vector3(x, y, 0);
                targetTimer = 0f;
            }

            // 스무스하게 이동
            targetRoot.localPosition = Vector3.Lerp(targetRoot.localPosition, targetPos, lerpSpeed);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래 위치로 부드럽게 복귀
        float returnElapsed = 0f;
        float returnDuration = 0.2f;
        Vector3 startPos = targetRoot.localPosition;

        while (returnElapsed < returnDuration)
        {
            targetRoot.localPosition = Vector3.Lerp(startPos, originalPos, returnElapsed / returnDuration);
            returnElapsed += Time.deltaTime;
            yield return null;
        }

        targetRoot.localPosition = originalPos;

        if (hideObject != null)
            hideObject.SetActive(true);
    }

}