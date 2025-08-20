using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    void Awake()
    {
        // 싱글턴 + 씬 전환 유지
        if(Instance!=null&&Instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // AudioSource 준비
        if(bgmAudioSource == null)
        {
            bgmAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
            bgmAudioSource.loop = true;
        }

        // BGM 시작
        if(!bgmAudioSource.isPlaying&&bgmAudioSource.clip != null)
        {
            bgmAudioSource.Play();
        }
    }

    // SFX 재생 (사운드 클립 인자로 받아 설정)
    public void PlaySFX(string clipName)
    {
        if(sfxAudioSource == null)
        {
            sfxAudioSource = transform.GetChild(1).GetComponent<AudioSource>();
        }

        // Resouces 폴더 안에 있는 에셋을 런타임에 로드
        AudioClip clip = Resources.Load<AudioClip>("Sound/" + clipName);

        if (clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }
}
