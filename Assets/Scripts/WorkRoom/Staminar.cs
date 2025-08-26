using UnityEngine;
using UnityEngine.UI;

public class Staminar : MonoBehaviour
{
    public Image fillImage;
    public float maxStamina;
    public float currentStamina;

    private int lastLevel = -1;
    private GameManager gameManager;

    void Start()
    {
        // GameManager 인스턴스를 가져옵니다.
        gameManager = GameManager.getInstance();

        // 게임 시작 시 스태미나 UI를 초기화합니다.
        StaminarUI();
    }

    void Update()
    {
        // GameManager 인스턴스가 없으면 리턴합니다.
        if (gameManager == null)
            return;

        // 자신의 태그에 맞는 업그레이드 상태를 GameManager에서 확인합니다.
        bool isUpgraded = false;
        if (gameObject.CompareTag("Fox") && gameManager.isFoxUpgraded)
        {
            isUpgraded = true;
            Debug.Log("Fox is Upgraded");
            
        }
        else if (gameObject.CompareTag("Sheep") && gameManager.isSheepUpgraded)
        {
            isUpgraded = true;
            Debug.Log("Fox is Upgraded");
        }
        else if (gameObject.CompareTag("Cat") && gameManager.isCatUpgraded)
        {
            isUpgraded = true;
            Debug.Log("Fox is Upgraded");
        }

        // 업그레이드 상태가 감지되면 스태미나를 풀 충전합니다.
        if (isUpgraded)
        {
            UpdateMaxStamina(); // 먼저 최대 스태미나를 업데이트합니다.
            RestoreStamina();   // 스태미나를 풀로 채웁니다.

            // 작업이 완료되면 GameManager의 상태 플래그를 초기화합니다.
            if (gameObject.CompareTag("Fox")) gameManager.isFoxUpgraded = false;
            else if (gameObject.CompareTag("Sheep")) gameManager.isSheepUpgraded = false;
            else if (gameObject.CompareTag("Cat")) gameManager.isCatUpgraded = false;
        }
        else
        {
            // 업그레이드 상태가 아닐 때만 레벨에 따라 maxStamina를 업데이트합니다.
            UpdateMaxStamina();
        }
    }

    // 이 함수를 통해 외부에서 호출하지 않고 Staminar 내에서 직접 maxStamina를 업데이트합니다.
    public void UpdateMaxStamina()
    {
        int level = 1;
        switch (gameObject.tag)
        {
            case "Fox": level = gameManager.Get_LoomLevel(); break;
            case "Sheep": level = gameManager.Get_FillerLevel(); break;
            case "Cat": level = gameManager.Get_DecoLevel(); break;
        }

        if (level != lastLevel)
        {
            lastLevel = level;
            switch (level)
            {
                case 1: maxStamina = 100f; break;
                case 2: maxStamina = 130f; break;
                case 3: maxStamina = 150f; break;
                case 4: maxStamina = 170f; break;
                case 5: maxStamina = 200f; break;
                default: maxStamina = 100f; break;
            }
        }
    }

    // 스태미나를 풀로 채우고 UI를 갱신하는 함수
    public void RestoreStamina()
    {
        float delta = maxStamina - currentStamina;

        if (delta > 0)
        {
            currentStamina = maxStamina;
            StaminarUI();

            // DB에도 반영
            // Employee 객체에 접근해야 하므로 GameManager에서 처리하거나 다른 방법을 찾아야 합니다.
            // 여기서는 Employee 컴포넌트를 직접 가져와서 EmployeeID를 사용합니다.
            Employee emp = GetComponentInParent<Employee>();
            if (emp != null)
            {
                gameManager.Change_Worker_Stamina(emp.EmployeeID, (int)delta);
                Debug.Log($"[Staminar] {emp.EmployeeName} 스태미너 풀 충전 완료!");
            }
        }
        else
        {
            Debug.Log($"[Staminar] 이미 풀스태미나 상태.");
        }
    }

    public void StaminarUI()
    {
        fillImage.fillAmount = currentStamina / maxStamina;

        Debug.Log("fillAmount"+fillImage.fillAmount);
        Debug.Log("current" + currentStamina);
        Debug.Log("max" + maxStamina);

        if (currentStamina < 50)
        {
            fillImage.color = Color.yellow;
        }
        if (currentStamina < 30)
        {
            fillImage.color = Color.red;
        }

        // 항상 카메라를 향하도록 (옵션)
        transform.forward = Camera.main.transform.forward;
    }

    public void Addstamina(int extrastamina)
    {
        currentStamina += extrastamina;
        StaminarUI();
    }
}
