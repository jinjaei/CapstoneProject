using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] AudioClips;

    [SerializeField] AudioSource BGMPlayer; // 배경 음악
    [SerializeField] AudioSource SFXPlayer; // 효과음
    [SerializeField] Slider SoundSlider;

    private static object _lock = new object();
    private static SoundManager _instance = null;
    public static SoundManager instance
    {
        get
        {
            if (applicationQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("SoundManager ");
                    obj.AddComponent<SoundManager>();
                    _instance = obj.GetComponent<SoundManager>();
                }
                return _instance;
            }
        }
        set
        {
            _instance = value;
        }
    }
    private static bool applicationQuitting = false;


    void Awake()
    {
        _instance = this;
        // 싱글톤 인스턴스
        SoundSlider.onValueChanged.AddListener(ChangeSoundVolume);
    }
    public void PlaySound(string type)
    {
        int index = 0;

        switch (type)
        {
            case "ButtonClick": index = 0; break; // 버튼 클릭 효과음
            case "Enhance": index = 1; break; // 강화 효과음
        }

        SFXPlayer.clip = AudioClips[index];
        SFXPlayer.Play();
    }

    void ChangeSoundVolume(float value)
    {
        BGMPlayer.volume = value;
        SFXPlayer.volume = value;
    }
}
