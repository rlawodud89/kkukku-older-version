using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    public AudioMixer audioMixer;
    [SerializeField] public AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    private GameManager gameManager;


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

        // 게임메니저
        gameManager= GameManager.getInstance();

        if (gameManager == null)
        {
            Debug.LogError("GameManager is not initialized.");
        }

         // AudioSource 준비
        if(bgmAudioSource == null)
        {
            bgmAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
            bgmAudioSource.loop = true;
            bgmAudioSource.playOnAwake = false;
        }

        
        // audioMixer 할당
        if(audioMixer == null)
        {
            var grp=bgmAudioSource.outputAudioMixerGroup;
            audioMixer = grp.audioMixer;
        }

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void Start()
    {
        // BGM 시작
        audioMixer.SetFloat("BGM", Mathf.Log10(gameManager.Get_BgSound()) * 20);
        bgmAudioSource.Play();
        //Debug.Log("BGM Volume: " + bgmAudioSource.volume);

        audioMixer.SetFloat("SFX", Mathf.Log10(gameManager.Get_EffectSound()) * 20);
    }

    private void OnActiveSceneChanged(Scene prev, Scene next)
    {
        if (next.name == "Prolog")
        {
            bgmAudioSource.Pause();
        }
        else
        {
            audioMixer.SetFloat("BGM", Mathf.Log10(gameManager.Get_BgSound()) * 20);
            bgmAudioSource.UnPause();
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
