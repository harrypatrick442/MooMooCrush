using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class AudioPlayer : IAudioPlayer
    {
        private AudioSources _AudioSources;
        public AudioPlayer(ILooper iLooper, GameObject audioSources, int nAudioSourcesToCreate, params string[] audioClipPaths)
        {
            foreach (string audioClipPath in audioClipPaths)
            {
                foreach (AudioClip audioClip in Resources.LoadAll<AudioClip>(audioClipPath))
                {
                    _MapNameToSoundEffect[audioClip.name] = new SoundEffect(audioClip, this);
                    Debug.Log(audioClip.name);
                }
            }
            List<AudioSourceWrapper> audioSourcesWrappers = new List<AudioSourceWrapper>();
            while (audioSources.GetComponentsInChildren<AudioSource>().Count() < nAudioSourcesToCreate)
            {
                AudioSource audioSource = audioSources.AddComponent<AudioSource>();
                audioSource.transform.parent = audioSources.transform;
                AudioSourceWrapper audioSoruceWrapper = new AudioSourceWrapper(audioSource);
                audioSourcesWrappers.Add(audioSoruceWrapper);
            }
            _AudioSources = new AudioSources(audioSourcesWrappers, iLooper);
        }
        private Dictionary<string, SoundEffect> _MapNameToSoundEffect = new Dictionary<string, SoundEffect>();

        public PlayingSoundEffectHandle Play(string name, float volume = 1.0f, bool keepAlive=false, bool looping=false)
        {
            Debug.Log("playing: " + name);
            if (_MapNameToSoundEffect.ContainsKey(name))
            {
                Debug.Log("playing: " + name);
                return _MapNameToSoundEffect[name].Play(volume,keepAlive, looping);
            }
            return null;
        }
        PlayingSoundEffectHandle IAudioPlayer.Play(AudioClip audioClip, float volume=1.0f, bool keepAlive=false, bool loop=false)
        {
            AudioSourceWrapper audioSourceWrapper = _AudioSources.Free;
            if (audioSourceWrapper != null)
            {keepAlive
				=loop||keepAlive;
                audioSourceWrapper.KeepAlive = keepAlive;
                audioSourceWrapper.Play(audioClip, volume, keepAlive, loop);
                return new PlayingSoundEffectHandle(audioSourceWrapper);
            }
            return null;
        }

        private class AudioSources : ILooper5Hz
        {

            public AudioSources(List<AudioSourceWrapper> audioSourceWrappers, ILooper iLooper)
            {
                _AudioSources = audioSourceWrappers;
                _ILooper = iLooper;
            }
            private ILooper _ILooper;
            private List<AudioSourceWrapper> _AudioSources = new List<AudioSourceWrapper>();
            private int Count
            {
                get
                {
                    int count = 0;
                    foreach (AudioSourceWrapper audioSourceWrapper in _AudioSources)
                    {
                        if (audioSourceWrapper.IsFree)
                            count++;
                    }
                    return count;
                }
            }
            public AudioSourceWrapper Free
            {
                get
                {
                    foreach (AudioSourceWrapper audioSourceWrapper in _AudioSources)
                    {
                        if (audioSourceWrapper.IsFree)
                            return audioSourceWrapper;
                    }
                    return null;
                }
            }
            public bool Looper5Hz()
            {
                return true;
            }
        }

    }
}
