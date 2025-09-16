using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private const int SOUND_VOLUME_MAX = 10;
    private static int soundVolume = 6;

    public event EventHandler OnSoundVolumeChanged;

    [SerializeField] private AudioClip coinPickupAudioClip;
    [SerializeField] private AudioClip fuelPickupAudioClip;
    [SerializeField] private AudioClip crashAudioClip;
    [SerializeField] private AudioClip landingSuccessAudioClip;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Lander.Instance.onFuelPickup += Lander_onFuelPickup;
        Lander.Instance.onCoinPickup += Lander_onCoinPickup;
        Lander.Instance.onLanded += Lander_onLanded;
    }
    private void Lander_onLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.Success:
                AudioSource.PlayClipAtPoint(landingSuccessAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
                break;
            default:
                AudioSource.PlayClipAtPoint(crashAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
                break;

        }
    }
    private void Lander_onCoinPickup(object sender, System.EventArgs e)
    {
        AudioSource.PlayClipAtPoint(coinPickupAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
    }

    private void Lander_onFuelPickup(object sender, System.EventArgs e)
    {
        AudioSource.PlayClipAtPoint(fuelPickupAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
    }

    public void ChangeSoundVolume() {
        soundVolume = (soundVolume + 1) % SOUND_VOLUME_MAX;
        OnSoundVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetSoundVolume() {
        return soundVolume;
    }

    public float GetSoundVolumeNormalized() {
        return ((float)soundVolume) / SOUND_VOLUME_MAX;
    }
}
