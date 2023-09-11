using System;
using UnityEngine;
using UnityEngine.UI;
using InspectorAttribute;

/*
 * Options menu.
 * Includes audio volume change, a link to the Juste Tools website and buttons for opening the save menus.
 * 
 */ 
namespace IV_Demo
{
    public class Options : MonoBehaviour
    {
        [Header("Audio")]
        [PreviewSprite(40)] public Sprite musicOnSprite;
        [PreviewSprite(40)] public Sprite musicOffSprite;
        [PreviewSprite(40)] public Sprite soundsOnSprite;
        [PreviewSprite(40)] public Sprite soundsOffSprite;
        [Space]
        public Button muteMusicButton;
        public Button muteSoundsButton;
        public Slider musicVolumeSlider;
        public Slider soundsVolumeSlider;
        [Header("Juste Tools logo")]
        public Button justeToolsButton;
        public string url = "https://justetools.com";
        [Header("Save")]
        public Button exportButton;
        public Button importButton;
        public SaveWindows saveWindows;
        
        const string muteMusicPrefKey = "Mute Music";
        const string muteSoundsPrefKey = "Mute Sounds";
        const string musicVolumePrefKey = "Music Volume";
        const string soundsVolumePrefKey = "Sounds Volume";

        const float editAfterDelay = 5f; // changes will be saved to disk after not changing the options for this much time

        float lastEditTime = -1;

        void Start()
        {
            // load player prefs
            if (PlayerPrefs.HasKey(muteMusicPrefKey))
                Audio.muteMusic = Convert.ToBoolean(PlayerPrefs.GetInt(muteMusicPrefKey));
            if (PlayerPrefs.HasKey(muteSoundsPrefKey))
                Audio.muteSounds = Convert.ToBoolean(PlayerPrefs.GetInt(muteSoundsPrefKey));

            if (PlayerPrefs.HasKey(musicVolumePrefKey))
                Audio.musicVolume = PlayerPrefs.GetFloat(musicVolumePrefKey);
            if (PlayerPrefs.HasKey(soundsVolumePrefKey))
                Audio.soundsVolume = PlayerPrefs.GetFloat(soundsVolumePrefKey);

            Audio.RefreshMusicVolume();

            ((Image)muteMusicButton.targetGraphic).sprite = (Audio.muteMusic ? musicOffSprite : musicOnSprite);
            ((Image)muteSoundsButton.targetGraphic).sprite = (Audio.muteSounds ? soundsOffSprite : soundsOnSprite);
            musicVolumeSlider.value = Audio.musicVolume;
            soundsVolumeSlider.value = Audio.soundsVolume;

            // Audio UI listeners
            muteMusicButton.onClick.AddListener(() =>
            {
                Audio.muteMusic = !Audio.muteMusic;
                ((Image)muteMusicButton.targetGraphic).sprite = (Audio.muteMusic ? musicOffSprite : musicOnSprite);
                Audio.RefreshMusicVolume();

                PlayerPrefs.SetInt(muteMusicPrefKey, Convert.ToInt32(Audio.muteMusic));
                lastEditTime = Time.unscaledTime;
            });

            muteSoundsButton.onClick.AddListener(() =>
            {
                Audio.muteSounds = !Audio.muteSounds;
                ((Image)muteSoundsButton.targetGraphic).sprite = (Audio.muteSounds ? soundsOffSprite : soundsOnSprite);

                PlayerPrefs.SetInt(muteSoundsPrefKey, Convert.ToInt32(Audio.muteSounds));
                lastEditTime = Time.unscaledTime;
            });

            musicVolumeSlider.onValueChanged.AddListener((f) =>
            {
                Audio.musicVolume = f;
                Audio.RefreshMusicVolume();

                PlayerPrefs.SetFloat(musicVolumePrefKey, Audio.musicVolume);
                lastEditTime = Time.unscaledTime;
            });

            soundsVolumeSlider.onValueChanged.AddListener((f) =>
            {
                Audio.soundsVolume = f;

                PlayerPrefs.SetFloat(soundsVolumePrefKey, Audio.soundsVolume);
                lastEditTime = Time.unscaledTime;
            });

            // just tools logo
            justeToolsButton.onClick.AddListener(() => Application.OpenURL(url));

            // export / import
            exportButton.onClick.AddListener(() => saveWindows.OpenExport());
            importButton.onClick.AddListener(() => saveWindows.OpenImport());
        }

        void Update()
        {
            if (lastEditTime >= 0 && Time.unscaledTime - lastEditTime > editAfterDelay)
            {
                PlayerPrefs.Save();
                lastEditTime = -1;
            }
        }
    }
}