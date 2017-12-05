using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IAudioPlayer
    {
		PlayingSoundEffectHandle Play(AudioClip audioClip, float volume = 1.0f, bool loop = false, bool keepAlive=false);
		PlayingSoundEffectHandle Play(string name, float volume = 1.0f, bool loop = false, bool keepAlive=false);
    }
}
