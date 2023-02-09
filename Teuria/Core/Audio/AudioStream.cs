using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoSound;
using MonoSound.Streaming;

namespace Teuria;

public class AudioStream 
{
    public StreamPackage StreamSound => streamSound;
    private StreamPackage streamSound;
    public void Load(string path, AudioType audioType, bool loop = false)
    {
        var titleContainer = TitleContainer.OpenStream(path);
        streamSound = StreamLoader.GetStreamedSound(titleContainer, audioType, loop);
    }
    
    public void LoadXact(string soundBankPath, string waveBankPath, string cueName, bool loop = false)
    {
        streamSound = StreamLoader.GetStreamedXACTSound(soundBankPath, waveBankPath, cueName, loop);
    }

    public void ApplyFilters(params int[] filters) 
    {
        streamSound.ApplyFilters(filters);
    }

    public void Play()
    {
        streamSound.PlayingSound.Play();
    }

    public void Pause() 
    {
        streamSound.PlayingSound.Pause();
    }

    public void Stop(bool immediate = false) 
    {
        streamSound.PlayingSound.Stop(immediate);
    }

    public void Free() 
    {
        StreamLoader.FreeStreamedSound(ref streamSound);
    }
}

public static class SoundController
{
    public static SoundEffect LoadXNB(string path) => Load(path + ".xnb");
    public static SoundEffect LoadMP3(string path) => Load(path + ".mp3");
    public static SoundEffect LoadOgg(string path) => Load(path + ".ogg");
    public static SoundEffect LoadWav(string path) => Load(path + ".wav");

    public static SoundEffect LoadXNB(string path, int filterID) => Load(path, AudioType.XNB, filterID);
    public static SoundEffect LoadMP3(string path, int filterID) => Load(path, AudioType.MP3, filterID);
    public static SoundEffect LoadOgg(string path, int filterID) => Load(path, AudioType.OGG, filterID);
    public static SoundEffect LoadWav(string path, int filterID) => Load(path, AudioType.WAV, filterID);
    public static SoundEffect Load(string path) 
    {
        return EffectLoader.GetEffect(path);
    }

    public static SoundEffect Load(string path, AudioType audioType, int filterID) 
    {
        using var titleContainer = TitleContainer.OpenStream(path);
        return EffectLoader.GetFilteredEffect(titleContainer, audioType, path, filterID);
    }

    public static void Play(SoundEffectInstance soundEffect, float volume, float pitch, float pan)
    {
        soundEffect.Volume = volume;
        soundEffect.Pitch = pitch;
        soundEffect.Pan = pan;
        soundEffect.Play();
    }

    public static void Play(SoundEffectInstance soundEffect)
    {
        soundEffect.Play();
    }
}