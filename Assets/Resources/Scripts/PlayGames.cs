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
    public class PlayGames : IDoSave
    {
        private ISavedGameMetadata _ISavedGameMetadata;
        private static volatile bool _IsInitialized = false;
        private bool Authenticated
        {
            get
            {
                return Social.localUser.authenticated;
            }
        }
        private static PlayGames _Instance;
        public static PlayGames Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new PlayGames();
                return _Instance;
            }
        }
        private Texture2D _Screenshot;
        public Texture2D Screenshot
        {
            get
            {
                return _Screenshot;
            }
            set
            {
                _Screenshot = value;
            }
        }
        public PlayGames()
        {
            if (_IsInitialized)
                throw new System.Exception("PlayGames is already initialized. Use Instance.");
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .RequireGooglePlus()
            .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            _IsInitialized = true;
            _Authenticate((bool success) => { });
        }
        private object _AuthenticateLock = new object();
        private void _Authenticate(Action<bool> callback)
        {
           /* if (Social.localUser.authenticated)
            {
                callback(true);
                return;
            }
            new Thread(() =>
        {
            lock (_AuthenticateLock)
            {*/
                if (!Social.localUser.authenticated)
                {
                    Social.localUser.Authenticate(callback);
                }
                else
                {
                    callback(true);
                }
           // }
        //}).Start();
        }
        private System.Object _LockOpenSave = new System.Object();
        public void Open(string fileName, IDoneOpen<INonVolatileData> iDoneOpen)
        {
            lock (_LockOpenSave)
            {
#if UNITY_EDITOR
                new OpenSave().Open(fileName, this, iDoneOpen);
#else
                _Authenticate((bool success) =>
                    {
                        if (success)
                        {
                            new OpenSave().Open(fileName, this, iDoneOpen);
                        }
                        else
                        {
                            OpenDoneInfo<INonVolatileData> openDoneInfo = new OpenDoneInfo<INonVolatileData>(new OpenException("Could not authenticate so failed to open game data!!!"));
                            openDoneInfo.Data = new NonVolatileData(fileName);
                            iDoneOpen.DoneOpen(openDoneInfo);
                        }
                    });
#endif
            }

        }
        public void Save(INonVolatileData iNonVolatileData, string fileName, IDoneSave iDoneSave)
        {
            lock (_LockOpenSave)
            {
#if UNITY_EDITOR
                Debug.Log("save");
                new OpenSave().Save(iNonVolatileData, fileName, iDoneSave);
#else
                    _Authenticate((bool success) =>
                    {
                        if (success)
                        {
                            new OpenSave().Save(iNonVolatileData, fileName, iDoneSave);
                        }
                        else
                        {
                            iDoneSave.DoneSave(new SaveDoneInfo(new SaveException("Could not authenticate so failed to save game data!!!")));
                        }
                    });
#endif
            }
        }
    }
}