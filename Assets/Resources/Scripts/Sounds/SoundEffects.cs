using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class SoundEffects
    {
		public static class Moo{
			public class Hurting
			{
				private static List<SoundEffectInfo> _List = new List<SoundEffectInfo>{
					new SoundEffectInfo("moo4"),
					new SoundEffectInfo("moo5"),
					new SoundEffectInfo("moo6"),
					new SoundEffectInfo("moo7"),
					new SoundEffectInfo("moo8"),
					new SoundEffectInfo("moo11"),
					new SoundEffectInfo("moo13"),
					new SoundEffectInfo("moo14"),
					//new SoundEffectInfo("moo15"),
					
				};
                private static RouletteWheel<SoundEffectInfo> _RouletteWheel;
                private static RouletteWheel<SoundEffectInfo> RouletteWheel{
					get{
						if(_RouletteWheel==null)
						{
							float probability = 100f/_List.Count;
							_RouletteWheel   =  new RouletteWheel<SoundEffectInfo>((from a in _List select new RouletteSlot<SoundEffectInfo>(probability, a)).ToArray());
						}
						return _RouletteWheel;
					}
				}
				public static SoundEffectInfo Random{
					
					get{
							return RouletteWheel.Spin();
					}
					
				}
			}
		}
			public static class Phaser{
				private static SoundEffectInfo _Running = new SoundEffectInfo("phaser0");
				public static SoundEffectInfo Running{
					get{
						return _Running;
					}
				}
			}
			public static class Crusher{
				private static SoundEffectInfo _Stomp = new SoundEffectInfo("stomp0");
				public static SoundEffectInfo Stomp{
					get{
						return _Stomp;
					}
				}
			}
			public static class MachineGun{
				private static SoundEffectInfo _Running = new SoundEffectInfo("gun0");
				public static SoundEffectInfo Running{
					get{
						return _Running;
					}
				}
			}
			public static class Conveyor{
				private static SoundEffectInfo _Running = new SoundEffectInfo("conveyor0", 0.2f);
				public static SoundEffectInfo Running{
					get{
						return _Running;
					}
				}
			}
			public static class Blood{
				private static SoundEffectInfo _Splat = new SoundEffectInfo("bloodsplat0");
				public static SoundEffectInfo Splat{
					get{
						return _Splat;
					}
				}
			}
	}
}
