using System;

namespace Assets.Scripts
{
	public class MessageBot:IMessageBot
	{
		private Display _Display;
		private ILooper _ILooper;

		public MessageBot (Display display, ILooper iLooper)
		{
			_Display = display;
			_ILooper = iLooper;
		}

		public void Crushed (ICrushable iCrushable, bool rightFoodType)
		{
			string str = null;
			if (typeof(Cow).IsAssignableFrom (iCrushable.GetType ())) {
				Cow cow = (Cow)iCrushable;
				switch (cow.CowType) {
				case Enums.CowType.Edible:
					switch (cow.Health) {
					case Enums.Health.Unwell:
						str = "Don't crush unwell cows! This contaminates the food!";
						break;
					default:
						str = "Wrong foot type. Switch trolly position before crushing!";
						break;
					}
					break;
				case Enums.CowType.Devil:
					str = "Crushing devil cows contaminates the food!";
					break;
				case Enums.CowType.Religious:
					str = "Try not to crush religious cows!";
					break;
				case Enums.CowType.SuperMoo:
					str = "Yum! Nothing like some super-moo to add flavour!";
					break;
				default:
					str = "Don't crush bombs ;).... BANG";
					break;
				}
			}
			if (str != null)
				_Display.DisplayItem (new MovingText (str, 1.1f, _ILooper, MovingText.Times.Once));
		}

		public void NotPhasable (Enums.CowType cowType)
		{
			string str = null;
			switch (cowType) {
			case Enums.CowType.Devil:
				str = "Devil cows are not phasible!";
				break;
			}
			if (str != null)
				_Display.DisplayItem (new MovingText (str, 1.1f, _ILooper, MovingText.Times.Once));
		}

		public void YouCrushedExplosive ()
		{
			_Display.DisplayItem (new MovingText ("You crushed TNT! Not a good idea...too late!", 1.3f, _ILooper, MovingText.Times.Once));
	
		}
	}
}

