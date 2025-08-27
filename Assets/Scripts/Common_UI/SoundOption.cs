using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOption : MonoBehaviour
{
    // 오디오 믹서
    public AudioMixer audioMixer;

    // 슬라이더
    public Slider BGMSlider;
    public Slider SFXSlider;

    // 사운드 버튼 
    public GameObject BGMSoundButton;
    public GameObject SFXSoundButton;

    // 사운드 버튼 이미지
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private GameManager gameManager;

    void Awake()
    {
        // 게임메니저
        gameManager= GameManager.getInstance();

        // db에서 값 받아와서 설정하기
        BGMSlider.value=gameManager.Get_BgSound();
        audioMixer.SetFloat("BGM", Mathf.Log10(BGMSlider.value) * 20);
        //BGMSoundButton.GetComponent<Button>().onClick.AddListener(() => ClickSoundButton(BGMSoundButton));

        SFXSlider.value = gameManager.Get_EffectSound();
        audioMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
        //SFXSoundButton.GetComponent<Button>().onClick.AddListener(() => ClickSoundButton(SFXSoundButton));
    }

    void Update() {
        if(BGMSlider.value == 0.0001f)
        {
            BGMSoundButton.GetComponent<Image>().sprite=soundOffSprite;
        }
        else
        {
            BGMSoundButton.GetComponent<Image>().sprite=soundOnSprite;
        }

        if(SFXSlider.value == 0.0001f)
        {
            SFXSoundButton.GetComponent<Image>().sprite=soundOffSprite;
        }
        else
        {
            SFXSoundButton.GetComponent<Image>().sprite=soundOnSprite;
        }
    }

    // 볼륨 조절
    public void SetBgmVolume(){
        audioMixer.SetFloat("BGM", Mathf.Log10(BGMSlider.value) * 20);
        gameManager.Set_BgSound(BGMSlider.value);

    }

    public void SetSfxVolume(){
        audioMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
        gameManager.Set_EffectSound(SFXSlider.value);
    }

    // 사운드 버튼 클릭시 
    public void ClickSoundButton(GameObject soundButton)
    {
        if(soundButton.GetComponent<Image>().sprite == soundOnSprite)
        {
            soundButton.GetComponent<Image>().sprite = soundOffSprite;
            if(soundButton.name == "BGMSoundButton")
            {
                audioMixer.SetFloat("BGM", -80f); // BGM 음소거
                BGMSlider.value = 0.0001f;
                gameManager.Set_BgSound(0.0001f); // DB에 저장
            }
            else if(soundButton.name == "SFXSoundButton")
            {
                audioMixer.SetFloat("SFX", -80f); // SFX 음소거
                SFXSlider.value = 0.0001f;
                gameManager.Set_EffectSound(0.0001f); // DB에 저장
            }
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundOnSprite;
            if(soundButton.name == "BGMSoundButton")
            {
                audioMixer.SetFloat("BGM", 0f); // BGM 재생
                BGMSlider.value = 1f;
                gameManager.Set_BgSound(1f); // DB에 저장
            }
            else if(soundButton.name == "SFXSoundButton")
            {
                audioMixer.SetFloat("SFX", 0f); // SFX 재생
                SFXSlider.value = 1f;
                gameManager.Set_EffectSound(1f); // DB에 저장
            }
        }

    }

}
