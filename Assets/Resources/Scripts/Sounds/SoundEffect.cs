using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class SoundEffect
    {
		
    private AudioClip _AudioClip;
	private IAudioPlayer _IAudioPlayer;
	public SoundEffect(AudioClip audioClip, IAudioPlayer iAudioPlayer)
	{
        _AudioClip = audioClip;
		_IAudioPlayer=iAudioPlayer;
	}
	public PlayingSoundEffectHandle Play(float vol=1.0f, bool keepAlive=false, bool looping=false)
	{
		return _IAudioPlayer.Play(_AudioClip, vol, keepAlive, looping);
	}
	}
}
