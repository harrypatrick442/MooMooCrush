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
    public interface  INonVolatileData   : IGetByteArray
    {
        ISavedGameMetadata GetSavedGameMetadata();
        Texture2D GetSavedGameImage();
        TimeSpan GetPlayedTimespan();
        object this[string key] { get; set; }
       Dictionary<string, object> Dictionary { get; }
    }
}
