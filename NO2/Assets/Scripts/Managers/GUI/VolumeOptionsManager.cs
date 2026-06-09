using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VolumeOptionsManager : MonoBehaviour
{
    public GameObject optionsPanel;
    public Slider masterSlider;
    public AudioMixer masterMixer;
    public AudioMixer audioMixer;
    public AudioMixer sfxMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public GameObject soundOn;
    public GameObject soundOff;


    //private InputSystem m_Actions;
    //private InputSystem.UIActions m_UI;
    //private bool _wantsToExit;
    //private bool _optionsOpened;
    void Start()
    {
        // Cargar valores guardados (o usar los de por defecto)
        float savedMasterMusic = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.7f);

        masterSlider.value = savedMasterMusic;
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        //SetBrightness(savedMasterMusic);
        SetMusicVolume(savedMusic);

        // Escuchar cambios en los sliders
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    //private void Update()
    //{
    //    if (_wantsToExit && _optionsOpened)
    //    {
    //        _wantsToExit= false;
    //        CloseOptions();
    //    }
    //}

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        //_optionsOpened= true;
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        //_optionsOpened= false;
        PlayerPrefs.Save(); // guarda en disco
    }



    void SetMusicVolume(float value)
    {
        // Convierte lineal → logarítmico (así suena natural)
        float db = value > 0.001f
            ? Mathf.Log10(value) * 20f
            : -80f;
        masterMixer.SetFloat("MusicVolume", db);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    void SetMasterVolume(float value)
    {
        // Convierte lineal → logarítmico (así suena natural)
        float db = value > 0.001f
            ? Mathf.Log10(value) * 20f
            : -80f;
        masterMixer.SetFloat("MasterVolume", db);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
    void SetSFXVolume(float value)
    {
        // Convierte lineal → logarítmico (así suena natural)
        float db = value > 0.001f
            ? Mathf.Log10(value) * 20f
            : -80f;
        masterMixer.SetFloat("SFXVolume", db);
        PlayerPrefs.SetFloat("SFXVolume", value);
        if (value < 0.01)
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
        }
        else
        {
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
    }


    //public void OnEscape(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        _wantsToExit = true;
    //    }
    //}
    //void OnDestroy()
    //{
    //    m_Actions.Dispose();
    //}
    //void OnEnable()
    //{
    //    m_UI.Enable();
    //}

}