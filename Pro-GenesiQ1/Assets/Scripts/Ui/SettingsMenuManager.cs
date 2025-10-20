using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderGeneral;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSfx;

    [Header("Resolution")]
    [SerializeField] private TMP_Dropdown dropdownResolution;

    [Header("Toggle")]
    [SerializeField] private Toggle toggleFullScreen;

    private Resolution[] resolutions;

    private void Start()
    {
        if (dropdownResolution is not null)
        {
            dropdownResolution.ClearOptions();
            AddResolutionOnDropdown(dropdownResolution);
            dropdownResolution.value = SettingsKeys.HasKey(SettingsKeys.ResolutionIndex) 
                ? SettingsKeys.GetInt(SettingsKeys.ResolutionIndex) : dropdownResolution.options.Count - 1;
        }

        if (SettingsKeys.HasKey(SettingsKeys.FullScreen))
        {
            Screen.fullScreen = SettingsKeys.GetInt(SettingsKeys.FullScreen) == 1;
        }
    
        if (toggleFullScreen is not null)
        {
            toggleFullScreen.isOn = Screen.fullScreen;
        }

        sliderGeneral?.onValueChanged.AddListener(SetVolumeGeneral);
        sliderMusic?.onValueChanged.AddListener(SetVolumeMusic);
        sliderSfx?.onValueChanged.AddListener(SetVolumeSfx);
    }
    private void OnEnable()
    {
        InitAudioSettings(sliderGeneral, SettingsKeys.General);
        InitAudioSettings(sliderMusic, SettingsKeys.Music);
        InitAudioSettings(sliderSfx, SettingsKeys.Sfx);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        SettingsKeys.SetInt(SettingsKeys.FullScreen, isFullScreen ? 1 : 0);
    }

    #region Audio

    private void SetVolumeGeneral(float volume)
    {
        SetVolume(SettingsKeys.General, volume);
    }

    private void SetVolumeSfx(float volume)
    {
        SetVolume(SettingsKeys.Sfx, volume);
    }

    private void SetVolumeMusic(float volume)
    {
        SetVolume(SettingsKeys.Music, volume);
    }

    private void SetVolume(string key, float volume)
    {
        if (audioMixer is null)
        {
            Debug.LogWarning("AudioMixer is not assigned in the inspector.");
            return;
        }
    
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("Audio key is null or empty.");
            return;
        }
    
        if (volume is < -80.0f or > 20.0f)
        {
            Debug.LogWarning($"Volume value {volume} is out of range. It must be between -80 and 20.");
            return;
        }

        audioMixer.SetFloat(key, volume);
        SettingsKeys.SetFloat(key, volume);
    }

    private void InitAudioSettings(Slider slider, string key)
    {
        if (slider is null || !SettingsKeys.HasKey(key) || audioMixer is null)
        {
            return;
        }
        
        slider.value = SettingsKeys.GetFloat(key);
        audioMixer.SetFloat(key, slider.value);
    }

    #endregion


    #region Resolution

    private void AddResolutionOnDropdown(TMP_Dropdown dropdown)
    {
        if (resolutions is null || resolutions.Length == 0)
        {
            resolutions = Screen.resolutions.GroupBy(r => new { r.width, r.height }).Select(group => group.First()).ToArray();
        }
    
        List<string> options = resolutions.Select(resolution => resolution.width + "x" + resolution.height).ToList();
    
        dropdown.AddOptions(options);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            return;
        }
    
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    
        SettingsKeys.SetInt(SettingsKeys.ResolutionIndex, resolutionIndex);
    }

    #endregion
}