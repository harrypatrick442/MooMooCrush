using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Assets.Scripts
{
    public interface IFeedable
    {
		bool FeedHover<T>(T iFeed) where T:IFeed;
		void FeedUnHover();
		bool Feed<T>(T iFeed) where T:IFeed;
    }
}