using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    public AudioClip glitchSound;
    public AudioClip gameplayMusic;

    private AudioSource sfxAudioSource;
    private AudioSource musicAudioSource;

    private void Awake() {
        if (instance) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource = gameObject.AddComponent<AudioSource>();

        musicAudioSource.clip = gameplayMusic;
        musicAudioSource.volume = 0.1f;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPitchedGlitch(float intensity) {

        sfxAudioSource.volume = Mathf.Min(intensity / 5f, 1f);
        // consumeAudioSource.clip = consumeSound;
        sfxAudioSource.pitch = (Random.Range(0.6f, 1.1f));
        sfxAudioSource.PlayOneShot(glitchSound);
    }

    public void PlayGlitchSound() {
        sfxAudioSource.PlayOneShot(glitchSound);
    }
}
