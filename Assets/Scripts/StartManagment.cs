using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManagment : MonoBehaviour
{
    public AudioSource audioSource;

    DBManager dbManager;

    void Start()
    {
        dbManager = DBManager.getInstance();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        audioSource.Play();  // 효과음

        // PlayerPrefs에 FirstPlay 값이 있는지 확인
        if (!PlayerPrefs.HasKey("FirstPlayer"))
        {
            // 처음 실행이므로 프롤로그로 이동
            PlayerPrefs.SetInt("FirstPlayer", 1);
            PlayerPrefs.Save();

            if (!dbManager.HaveDB()) dbManager.InitDB();
            SceneManager.LoadScene("Prolog");
        }
        else
        {
            // 이미 실행된 적이 있으므로 메인 게임으로 이동
            User user = dbManager.Get_User();
            SceneManager.LoadScene(user.endScene);
        }
    }
}
