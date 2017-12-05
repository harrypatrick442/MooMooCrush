using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.SavedGame;
namespace Assets.Scripts
{
    public interface  IStalkable
    {
        bool StalkableIsTaken { get; }
        void TakeStalkable(WeakReference stalker);
        void StopStalking();
    }
}
