using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public float masterVolume;
    public float musicVolume;
    public float SFXVolume;

    public float test;
    // Start is called before the first frame update
    public void SetMasterVolume(float volume)
    {
        if (volume > 0)
        {
            if (masterVolume < 0)
            {
                masterVolume += volume;
                if (masterVolume > 0)
                {
                    masterVolume = 0;
                }
                masterMixer.SetFloat("Master", masterVolume);
            }
        }
        else
        {
            if (masterVolume > -80)
            {
                masterVolume += volume;
                if (masterVolume < -80)
                {
                    masterVolume = -80;
                }
                masterMixer.SetFloat("Master", masterVolume);
            }
        }
    }
    public void SetSFXVolume(float volume)
    {
        if (volume > 0)
        {
            if (SFXVolume < 0)
            {
                SFXVolume += volume;
                if (SFXVolume > 0)
                {
                    SFXVolume = 0;
                }
                masterMixer.SetFloat("SFX", SFXVolume);
            }
        }
        else
        {
            if (SFXVolume > -80)
            {
                SFXVolume += volume;
                if (SFXVolume < -80)
                {
                    SFXVolume = -80;
                }
                masterMixer.SetFloat("SFX", SFXVolume);
            }
        }
    }
    public void SetMusicVolume(float volume)
    {
        if (volume > 0)
        {
            if (musicVolume < 0)
            {
                musicVolume += volume;
                if (musicVolume > 0)
                {
                    musicVolume = 0;
                }
                masterMixer.SetFloat("Music", musicVolume);
            }
        }
        else
        {
            if (musicVolume > -80)
            {
                musicVolume += volume;
                if (musicVolume < -80)
                {
                    musicVolume = -80;
                }
                masterMixer.SetFloat("Music", musicVolume);
            }
        }
    }
    //not for useres
    public void MuteAllAudio()
    {
        masterMixer.SetFloat("Master", -80f);
    }
    public void UnMuteAllAudio()
    {
        masterMixer.SetFloat("Master", 0f);
    }
    public void AudioLoad(float master, float music, float sfx)
    {
        masterMixer.SetFloat("Master", master);
        masterMixer.SetFloat("Music", music);
        masterMixer.SetFloat("SFX", sfx);
    }
}