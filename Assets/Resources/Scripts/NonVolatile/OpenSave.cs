using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class OpenSave
    {
        private IDoSave _IDoSave;
        private EventWaitHandle _EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private INonVolatileData _INonVolatileData;
        private ISavedGameMetadata _ISavedGameMetadata;
        private Exception _Exception;
        private Boolean _Exhausted = false;
        private enum Types { Open, Save }
        private Types _Type;
        private string _FileName;
        private static System.Object Lock = new System.Object();
        private void DoOpen(ISavedGameMetadata iSavedGameMetadata)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.ReadBinaryData(iSavedGameMetadata, OnSavedGameDataRead);
        }
        private void DoSave(ISavedGameMetadata iSavedGameMetadata)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
            builder = builder.WithUpdatedPlayedTime(_INonVolatileData.GetPlayedTimespan()).WithUpdatedDescription("Saved game at " + DateTime.Now);
            Texture2D screenshot = _INonVolatileData.GetSavedGameImage();
            if (screenshot != null)
            {
                try { builder = builder.WithUpdatedPngCoverImage(screenshot.EncodeToPNG()); } catch (Exception ex) { Debug.Log(ex.StackTrace); }
            }
            SavedGameMetadataUpdate updatedMetadata = builder.Build();
            savedGameClient.CommitUpdate(iSavedGameMetadata, updatedMetadata, _INonVolatileData.GetByteArray(), OnSavedGameWritten);

        }
        private Settings _Settings;
        private Settings Settings
        {
            get
            {
                if (_Settings == null)
                    _Settings = new Settings(_FileName);
                return _Settings;
            }
        }
        public void Save(INonVolatileData iNonVolatileData, string fileName, IDoneSave iDoneSave)
        {
            Debug.Log("save");
            new Thread(() =>
            {
                lock (Lock)
                {
                    if (_Exhausted)
                        iDoneSave.DoneSave(new SaveDoneInfo(new SaveException("OpenSave instance was already exhausted")));
                    _FileName = fileName;
                    _Type = Types.Save;
#if UNITY_EDITOR
                    foreach (String key in iNonVolatileData.Dictionary.Keys)
                        Debug.Log(key + " : " + iNonVolatileData.Dictionary[key]);
                    Settings.ReplaceOrAdd("NonVolatileData" + fileName, iNonVolatileData.Dictionary);
                    Settings.Save();
                    Debug.Log("saving");
                    iDoneSave.DoneSave(new SaveDoneInfo());
#else
                    _INonVolatileData = iNonVolatileData;
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        OpenSavedGame(fileName);
                    });
                    _EventWaitHandle.WaitOne();
                    _Exhausted = true;
                    if (_Exception != null)
                        iDoneSave.DoneSave(new SaveDoneInfo(_Exception));
                    else
                        iDoneSave.DoneSave(new SaveDoneInfo());
#endif

                }
            }).Start();
        }
        public void Open(string fileName, IDoSave iDoSave, IDoneOpen<INonVolatileData> iDoneOpen)
        {
            new Thread(() =>
            {
                lock (Lock)
                {
                    if (_Exhausted)
                        iDoneOpen.DoneOpen(new OpenDoneInfo<INonVolatileData>(new OpenException("OpenSave instance was already exhausted!!!")));
                    _IDoSave = iDoSave;
                    _Type = Types.Open;
                    _FileName = fileName;

#if UNITY_EDITOR
                    Dictionary<string, object> dictionary = null;
                    try
                    {
                        dictionary = (Dictionary<string, object>)Settings.getObject("NonVolatileData" + fileName);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("failed to get " + "NonVolatileData" + _FileName);
                    }
                    NonVolatileData nonVolatileData = new NonVolatileData(dictionary, fileName);
                    nonVolatileData.SetInterface(iDoSave);
                    iDoneOpen.DoneOpen(new OpenDoneInfo<INonVolatileData>(nonVolatileData));

#else
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            OpenSavedGame(fileName);
                        });
                        _EventWaitHandle.WaitOne();
                        _Exhausted = true;
                        if (_Exception != null)
                            iDoneOpen.DoneOpen(new OpenDoneInfo<INonVolatileData>(_Exception));
                        else
                            iDoneOpen.DoneOpen(new OpenDoneInfo<INonVolatileData>(_INonVolatileData));

#endif

                }
            }).Start();
        }
        private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata iSavedGameMetadata)
        {
            try
            {
                if (!_Exhausted)
                {
                    //_INonVolatileData.SetSavedGameMetadata(iSavedGameMetadata);
                    if (status == SavedGameRequestStatus.Success)
                    {
                        Debug.Log("Saved successfully");
                    }
                    else
                    {
                        _Exception = new SaveException();
                    }
                }
            }
            catch (Exception ex)
            {
                _Exception = new SaveException("", ex);
            }
            finally
            {
                _EventWaitHandle.Set();
            }
        }
        private void OpenSavedGame(string filename)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
        }
        private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata iSavedGameMetadata)
        {
            if (!_Exhausted)
            {
                _ISavedGameMetadata = iSavedGameMetadata;
                if (status == SavedGameRequestStatus.Success)
                {
                    if (_Type.Equals(Types.Open))
                    {
                        try
                        {
                            DoOpen(iSavedGameMetadata);
                            return;
                        }
                        catch (Exception ex)
                        {
                            _Exception = new OpenException("", ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            DoSave(iSavedGameMetadata);
                            return;
                        }
                        catch (Exception ex)
                        {
                            _Exception = new SaveException("", ex);
                        }
                    }
                }
                else
                {
                    _Exception = new OpenException();
                }
            }
            _EventWaitHandle.Set();
        }
        private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
        {

            if (!_Exhausted)
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    try
                    {
                        NonVolatileData nonVolatileData = new NonVolatileData(data, _FileName);
                        if (_IDoSave != null)
                            nonVolatileData.SetInterface(_IDoSave);
                        _INonVolatileData = nonVolatileData;
                    }
                    catch (Exception ex)
                    {
                        for (int I = 0; I < 100001; I++)
                            Debug.Log(ex);
                        _Exception = new OpenException("", ex);
                    }
                }
                else
                {
                    for (int I = 0; I < 100001; I++)
                        Debug.Log("OnSavedGameDataReadFail");
                    _Exception = new OpenException();
                }
            }
            _EventWaitHandle.Set();
        }
    }
}
