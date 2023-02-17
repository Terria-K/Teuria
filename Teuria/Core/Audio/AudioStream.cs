// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Audio;
// using MonoSound;

// namespace Teuria;

// public static class SoundController
// {
//     public static SoundEffect LoadXNB(string path) => Load(path + ".xnb");

//     public static SoundEffect Load(string path) 
//     {
//         return EffectLoader.GetEffect(path);
//     }

//     public static void Play(SoundEffectInstance soundEffect, float volume, float pitch, float pan)
//     {
//         soundEffect.Volume = volume;
//         soundEffect.Pitch = pitch;
//         soundEffect.Pan = pan;
//         soundEffect.Play();
//     }

//     public static void Play(SoundEffectInstance soundEffect)
//     {
//         soundEffect.Play();
//     }
// }