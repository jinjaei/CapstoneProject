using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * A simple Audio manager.
 * It will play a single music on loop and sounds on multiples audio sources generated automatically.
 * You can add sounds by editing the Sound enum (+ the soundsCount const).
 * 
 */
namespace IV_Demo
{
    [RequireComponent(typeof(AudioSource))]
    public class Audio : AManager<Audio>
    {
        // sounds enum
        public enum Sound
        {
            None = -1,
            MenuChange,
            BuyUpgrade,
            BuyIncome,
            CannotBuy,
            TypingEnd,
        }
        const int soundsCount = 5;

        // custom types
#pragma warning disable CS0649
        [Serializable] struct SoundParams
        {
            public string soundName;
            public SoundClip[] soundClipsArray;
            public float[] ponderationsArray;
        }

        [Serializable] struct SoundClip
        {
            public AudioClip clip;
            [Range(0f, 1f)] public float volumeRatio;
            public MinMax pitchBounds;
            [Range(-5, 5)] public int priority;
        }

        // editor
        [SerializeField] AudioClip music;
        [SerializeField, Range(0f, 1f)] float musicVolumeRatio = 1;
        [SerializeField] SoundParams[] sounds_ParamsArray;
        [SerializeField] int maxAudioSources = 64;
#pragma warning restore CS0649

        // public static access
        public static float musicVolume = 1f;

        public static float soundsVolume = 1f;

        public static bool muteMusic = false;

        public static bool muteSounds = false;

        public static void PlaySound(Sound sound)
        {
            if (sound < 0 || soundsVolume == 0 || muteSounds)
                return;

            PlaySound(instance.sounds_ParamsArray[(int)sound]);
        }

        public static void RefreshMusicVolume()
        {
            instance.musicSource.volume = (muteMusic ? 0 : musicVolume) * instance.musicVolumeRatio;
        }

        // internal logic
        GameObject sourceRefObj;
        Transform soundsParent;
        List<AudioSource> sourcesList = new List<AudioSource>();
        AudioSource musicSource;

        AudioSource testSource;

        static void PlaySound(SoundParams soundParam)
        {
            AudioSource source = null;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (instance.testSource == null)
                    instance.testSource = instance.GetComponent<AudioSource>();
                source = instance.testSource;
            }
            else
#endif
                source = instance.GetAudioSource();

            if (source == null)
                return;

            SoundClip soundClip = CustomRandom.Ponderated(soundParam.soundClipsArray, soundParam.ponderationsArray);

            source.clip = soundClip.clip;
            source.pitch = soundClip.pitchBounds.RandomRange();
            source.volume = soundsVolume * soundClip.volumeRatio;
            source.priority = 128 - soundClip.priority;
            source.Play();
        }

        AudioSource GetAudioSource()
        {
            foreach (AudioSource source in sourcesList)
            {
                if (!source.isPlaying)
                    return source;
            }

            return null;
        }

        // unity
        void Awake()
        {
#if !UNITY_EDITOR
            if (testSource == null)
                testSource = GetComponent<AudioSource>();

            testSource.enabled = false;
#endif

            Transform sourcesParent = GameObject.Find("Audio Sources").transform;

            musicSource = sourcesParent.Find("Music").GetComponent<AudioSource>();
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.Play();

            RefreshMusicVolume();

            sourceRefObj = sourcesParent.Find("Reference Objects/Source").gameObject;
            soundsParent = sourcesParent.Find("Sounds");

            soundsParent.hierarchyCapacity = maxAudioSources;
            for (int i = 0; i < maxAudioSources; i++)
            {
                GameObject inst = Instantiate(sourceRefObj, soundsParent);
                AudioSource sourceComp = inst.GetComponent<AudioSource>();

                sourcesList.Add(sourceComp);
            }
        }

        void OnValidate()
        {
            EnforceSoundsParamArray<Sound>(ref sounds_ParamsArray, soundsCount);
        }

        void EnforceSoundsParamArray<T>(ref SoundParams[] array, int enumCount) where T : Enum
        {
            array = OnValidateUtility.EnforceConstantEnumArray<T, SoundParams>(array, (SoundParams sp) => sp.soundName,
                (SoundParams sp, string str) => { sp.soundName = str; return sp; }, enumCount);

            for (int i = 0; i < enumCount; i++)
            {
                if (array[i].soundClipsArray != null && array[i].ponderationsArray.Length != array[i].soundClipsArray.Length)
                    Array.Resize(ref array[i].ponderationsArray, array[i].soundClipsArray.Length);

                if (array[i].soundClipsArray != null)
                    for (int j = 0; j < array[i].soundClipsArray.Length; j++)
                    {
                        array[i].soundClipsArray[j].pitchBounds = new MinMax(
                            Mathf.Max(-3, array[i].soundClipsArray[j].pitchBounds.min),
                            Mathf.Min(3, array[i].soundClipsArray[j].pitchBounds.max));
                    }
            }
        }
    }
}