using UnityEngine;
using UnityEngine.Audio;

public class AudioContoller : MonoBehaviour
{
    public static AudioContoller main;

    public AudioMixer audioMixer;

    private void Awake()
    {
        //Singleton pattern
        if (main != null && main != this)
        {
            Destroy(gameObject);
            return;
        }
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set saved volume
        audioMixer.SetFloat("musicVol", PlayerPrefs.GetFloat("musicVol"));
        audioMixer.SetFloat("SFXVol", PlayerPrefs.GetFloat("SFXVol"));
    }

    public void SetMusicVolume(float soundLevel)
    {
        audioMixer.SetFloat("musicVol", soundLevel);
        PlayerPrefs.SetFloat("musicVol", soundLevel);
    }

    public void SetSFXVolume(float soundLevel)
    {
        audioMixer.SetFloat("SFXVol", soundLevel);
        PlayerPrefs.SetFloat("SFXVol", soundLevel);
    }
}
