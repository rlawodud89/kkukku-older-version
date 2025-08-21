using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EmployeeInfo : MonoBehaviour
{

    private Employee Employee1;
    private Employee Employee2;
    private Employee Employee3;

    private Image fullcirclebar1;
    private Image fullcirclebar2;
    private Image fullcirclebar3;

    private Image fullbar1;
    private Image fullbar2;
    private Image fullbar3;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Employee1 == null)
        {
            GameObject empObj = GameObject.Find("Employee1(Clone)");
            if (empObj != null)
                Employee1 = empObj.GetComponent<Employee>();
        }

        if (Employee1 != null)
        {
            // 2. Employee1 오브젝트의 자식 중에서 "FullCircleBar"라는 이름의 오브젝트를 찾습니다.
            Transform fullCircleBarTransform = Employee1.transform.Find("fullcirclebar");
            Transform fullBarTransform = Employee1.transform.Find("fullbar");


            if (fullCircleBarTransform != null)
            {
                // 3. 찾은 오브젝트에서 Image 컴포넌트를 가져옵니다.
                fullcirclebar1 = fullCircleBarTransform.GetComponent<Image>();
                fullbar1 = fullBarTransform.GetComponent<Image>();
                if (fullcirclebar1 == null)
                {
                    Debug.LogError("FullCircleBar 오브젝트에 Image 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.LogError("Employee1의 자식 중에서 FullCircleBar 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
