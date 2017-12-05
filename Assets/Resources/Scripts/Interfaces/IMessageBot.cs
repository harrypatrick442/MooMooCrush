using System;

namespace Assets.Scripts
{
	public interface IMessageBot
	{
		void Crushed (ICrushable iCrushable, bool rightFoodType);
		void NotPhasable(Enums.CowType cowType);
		void YouCrushedTN();
	}
}

