using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class SoundEffectInfo
    {
		private float _Volume;
		public float Volume{
			get{
				return _Volume;
			}
		}
		private string _Name;
		public string Name{
			
			get{
				return _Name;
				}
		}
		public SoundEffectInfo(string name, float volume=1)
		{
			_Name=name;
			_Volume=volume;
		}
	}
}
