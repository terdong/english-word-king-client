using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance
    {
        private set;
        get;
    }

    private AudioSource audio_source_bgm_ = null;
    private AudioSource audio_source_effect_ = null;

    private AudioClip audio_clip_bgm_ = null;
    private AudioClip audio_clip_effect_ = null;

    private IDictionary<string, AudioClip> sound_pool_ = null;

    public static void PlayEffect( string effect_name )
    {
        if (Instance)
        {
            Instance.playEffect(effect_name);
        }
    }

    public static void PlayEffect_Touch()
    {
        SoundManager.PlayEffect( "gui_click" );
    }

    public static void PlayBGM( string bgm_name )
    {
        Instance.playBGM( bgm_name );
    }

    //public void TestPlay()
    //{
    //    audio_clip_bgm_ = Resources.Load<AudioClip>( "Audio/BGM/BGM_Kaze_ni_Naru_ar" );
    //    if ( audio_clip_bgm_ != null )
    //    {
    //        audio_source_effect_.PlayOneShot( audio_clip_bgm_ );
    //    }
    //}

    void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //Object.DontDestroyOnLoad(gameObject);

        Instance = this;

        audio_source_effect_ = transform.FindChild("Effect").GetComponent<AudioSource>();
        audio_source_bgm_ = transform.FindChild("BGM").GetComponent<AudioSource>();

        sound_pool_ = new Dictionary<string, AudioClip>();
    }

    protected void OnDestroy()
    {
        if (Instance != null)
        {
            Instance = null;
        }
    }

    void playEffect(string effect_name)
    {
        audio_clip_effect_ = null;
        if (sound_pool_.ContainsKey(effect_name))
        {
            audio_clip_effect_ = sound_pool_[effect_name];
        }else
        {
            audio_clip_effect_ = Resources.Load<AudioClip>("Audio/Effect/" + effect_name);
            sound_pool_.Add(effect_name, audio_clip_effect_);
            Debug.Log("sound_pool count = " + sound_pool_.Count);
        }

        if (audio_clip_effect_ != null)
        {
            audio_source_effect_.PlayOneShot(audio_clip_effect_);
        }
    }

    void playBGM(string bgm_name)
    {
        audio_clip_bgm_ = null;
        if (sound_pool_.ContainsKey(bgm_name))
        {
            audio_clip_bgm_ = sound_pool_[bgm_name];
        }
        else
        {
            audio_clip_bgm_ = Resources.Load<AudioClip>("Audio/BGM/" + bgm_name);
            sound_pool_.Add(bgm_name, audio_clip_bgm_);
            Debug.Log("sound_pool count = " + sound_pool_.Count);
        }

        if (audio_clip_bgm_ != null)
        {
            audio_source_bgm_.PlayOneShot(audio_clip_bgm_);
        }
    }
}
