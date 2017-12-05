using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class PlayingSoundEffectHandle:IDisposable
    {
		private AudioSourceWrapper _AudioSourceWrapper;
private int _AudioSourceWrapperUserIteration;
		private bool _Disposed=false;
		private bool Disposed{
			get{
	return _Disposed||_AudioSourceWrapper.IsFree||_AudioSourceWrapper.UseIteration!=_AudioSourceWrapperUserIteration;
			}
		}
		public PlayingSoundEffectHandle(AudioSourceWrapper audioSourceWrapper)
		{
            _AudioSourceWrapper = audioSourceWrapper;
			_AudioSourceWrapperUserIteration=audioSourceWrapper.UseIteration;
		}
		public void Start(){
			if(Disposed)
				throw new ObjectDisposedException("The object has already been disposed of");
            _AudioSourceWrapper.AudioSource.Play();
		}
		public void Stop(){
			if(Disposed)
				throw new ObjectDisposedException("The object has already been disposed of");
            _AudioSourceWrapper.AudioSource.Stop();
		}
		public void Pause(){
			if(Disposed)
				throw new ObjectDisposedException("The object has already been disposed of");
            _AudioSourceWrapper.AudioSource.Pause();
		}
		public void UnPause(){
			if(Disposed)
				throw new ObjectDisposedException("The object has already been disposed of");
            _AudioSourceWrapper.AudioSource.UnPause();
		}
		public bool IsAlive{
			get{
				return !Disposed;
			}
		}
		~PlayingSoundEffectHandle
		()
		{
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
			Debug.Log("disposing2");
			if(!_Disposed&&_AudioSourceWrapper.UseIteration==_AudioSourceWrapperUserIteration&&_AudioSourceWrapper.KeepAlive)
			{
                _AudioSourceWrapper.Free();
			}
            });
		}
		public void Dispose()
		{
			Debug.Log("disposing");
			if(!Disposed&&_AudioSourceWrapper.UseIteration==_AudioSourceWrapperUserIteration)
			{
			_Disposed=true;
                _AudioSourceWrapper.Free();
			}
		}
	}
}
