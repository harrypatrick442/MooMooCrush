using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.SavedGame;
using System.Threading;
namespace Assets.Scripts
{
    public class SaveException : Exception
    {
        public SaveException(): base()
        {
        }
        public SaveException(string message, Exception ex):base(message, ex)
        {
            
        }
        public SaveException(string message) : base(message)
        {
           
        }
    }

}