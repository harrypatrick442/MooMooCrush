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
    public class Moosics : MonoBehaviour, IMoosics
    {
        private ILooper _ILooper;
        public Moooooosic Spawn(Vector2 from, Vector2 to, float speed)
        {
            return new Moooooosic(_ILooper, gameObject, from, to, speed);
        }
        public void SetInterface<T>(T o)
        {
            Type type = o.GetType();
            if (typeof(ILooper).IsAssignableFrom(type))
            {
                _ILooper = (ILooper)o;
            }
        }
    }
}