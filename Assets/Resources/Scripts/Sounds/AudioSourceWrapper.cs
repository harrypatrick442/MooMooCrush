using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
        public class AudioSourceWrapper
        {
			
			
			private int _UseIteration=0;
			public int UseIteration{
				get{return _UseIteration;}
			}
            private AudioSource _AudioSource;
			public AudioSource AudioSource{
				get{
					return _AudioSource;
				}
			}
            private bool _KeepAlive;
            public bool KeepAlive
            {
                get
                {
                    return _KeepAlive;
                }
                set
                {
                    _KeepAlive = value;
                }
            }
            public AudioSourceWrapper(AudioSource audioSource, bool keepAlive = false)
            {
                _AudioSource = audioSource;
                _KeepAlive = keepAlive;
            }
            public bool IsFree
            {
                get
                {
                    return !(_AudioSource.isPlaying || _KeepAlive|| _AudioSource.loop);
                }
            }
public void Free()
{
	_AudioSource.Stop();
	_AudioSource.loop=false;
	_KeepAlive=false;
}
            public void Play(AudioClip audioClip, float volume,bool keepAlive, bool loop)
            {
				_AudioSource.clip=audioClip;
                _AudioSource.loop = loop;
				 _AudioSource.volume=volume;
                _AudioSource.Play();
				_UseIteration++;
            }
        }
}
