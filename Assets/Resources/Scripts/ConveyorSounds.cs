using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class ConveyorSounds : IPausible
    {
		private PlayingSoundEffectHandle _PlayingSoundEffectHandle;
		public ConveyorSounds(IAudioPlayer iAudioPlayer){
			SoundEffectInfo soundEffectInfo =SoundEffects.Conveyor.Running;
			string name = soundEffectInfo.Name;
			_PlayingSoundEffectHandle = iAudioPlayer.Play( name, soundEffectInfo.Volume, true, true);
		}
        public void Pause()
        {
			//if(_PlayingSoundEffectHandle!=null)
			//_PlayingSoundEffectHandle.Pause();
        }
        public void Unpause()
        {
			if(_PlayingSoundEffectHandle!=null)
			_PlayingSoundEffectHandle.UnPause();
        }
    }
}
