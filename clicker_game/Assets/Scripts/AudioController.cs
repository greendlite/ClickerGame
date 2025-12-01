using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public AudioSource MusicSFX;
    public AudioSource SoundSFX;

    public Sprite MusicOn;
    public Sprite MusicOff;
    public Sprite SoundOn;
    public Sprite SoundOff;

    public Image MusicButton;
    public Image SoundButton;
    
    private void Awake()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicSFX.volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        
        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            SoundSFX.volume = PlayerPrefs.GetFloat("SoundVolume");
        }
    }
    
    private void Start()
    {
        if (MusicSFX.volume >= 0.1f)
        {
            MusicButton.GetComponent<Image>().sprite = MusicOn;
        }
        else
        {
            MusicButton.GetComponent<Image>().sprite = MusicOff;
        }

        if (SoundSFX.volume >= 0.1f)
        {
            SoundButton.GetComponent<Image>().sprite = SoundOn;
        }
        else
        {
            SoundButton.GetComponent<Image>().sprite = SoundOff;
        }
    }
    
    public void OnClickMusicButton()
    {
        if (MusicSFX.volume >= 0.1f)
        {
            MusicSFX.volume = 0;
            MusicButton.GetComponent<Image>().sprite = MusicOff;
        }
        else
        {
            MusicSFX.volume = 0.5f;
            MusicButton.GetComponent<Image>().sprite = MusicOn;
        }
        
        PlayerPrefs.SetFloat("MusicVolume", MusicSFX.volume);
    }
    
    public void OnClickSoundButton()
    {
        if (SoundSFX.volume >= 0.1f)
        {
            SoundSFX.volume = 0;
            SoundButton.GetComponent<Image>().sprite = SoundOff;
        }
        else
        {
            SoundSFX.volume = 0.5f;
            SoundButton.GetComponent<Image>().sprite = SoundOn;
        }
        
        PlayerPrefs.SetFloat("SoundVolume", SoundSFX.volume);
    }
}
