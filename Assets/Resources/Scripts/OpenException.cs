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
    public class OpenException : Exception
    {
        public  OpenException():base()
        {
        }
        public OpenException(string message, Exception ex): base(message, ex)
        {
        }
        public OpenException( string message): base(message)
        {
        }
    }

}