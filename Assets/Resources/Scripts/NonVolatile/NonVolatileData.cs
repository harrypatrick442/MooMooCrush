using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.SavedGame;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace Assets.Scripts
{
    class NonVolatileData : INonVolatileData, IDoneSave
    {
        private DateTime? _StartDateTime;
        private DateTime StartDateTime
        {
            get
            {
                if (_StartDateTime == null)
                    return DateTime.Now;
                return (DateTime)_StartDateTime;
            }
        }
        private IDoSave _IDoSave;
        private MySavedGameMetadata _MySavedGameMetadata = new MySavedGameMetadata();
        private Dictionary<string, object> _Dictionary = new Dictionary<string, object>();
        public Dictionary<string, object> Dictionary { get { return _Dictionary; } }
        private WhenFinishedModifying _WhenFinishedModifying;
        private WhenFinishedModifying WhenFinishedModifying
        {
            get
            {
                if (_WhenFinishedModifying == null) _WhenFinishedModifying = new WhenFinishedModifying(_FinishedModifying, 400);
                return _WhenFinishedModifying;
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
        private Thread _ThreadScheduleSave;
        private Thread ThreadScheduleSave
        {
            get
            {
                if (_ThreadScheduleSave == null)
                    _ThreadScheduleSave = new Thread(() => { });
                return _ThreadScheduleSave;
            }
        }
        private string _FileName;
        public NonVolatileData(string fileName)
        {
            _FileName = fileName;
        }
        public NonVolatileData(Dictionary<string, object> dictionary, string fileName)
        {
            _FileName = fileName;
            if (dictionary != null)
                _Dictionary = dictionary;
        }
        public NonVolatileData(byte[] byteArray, string fileName)
        {
            _FileName = fileName;
            if (byteArray.Length > 0)
            {
                try
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    MemoryStream memoryStream = new MemoryStream(byteArray);
                    _Dictionary = (Dictionary<string, object>)binaryFormatter.Deserialize(memoryStream);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }
        public void AddOrReplace(string key, object value)
        {
            this[key] = value;
        }
        private void _ScheduleSave()
        {
            WhenFinishedModifying.Reset();
        }
        private void _FinishedModifying()
        {
            Debug.Log("finished modifying");
            Debug.Log(_IDoSave);
            if (_IDoSave != null)
                _IDoSave.Save(this, _FileName, this);
        }
        public object this[string key]
        {
            get
            {
                if (_Dictionary.ContainsKey(key))
                    return _Dictionary[key];
                return null;
            }
            set
            {
                _Dictionary[key] = value;
                if (_StartDateTime == null) _StartDateTime = DateTime.Now;
                _ScheduleSave();
            }
        }
        public void DoneSave(SaveDoneInfo saveDoneInfo)
        {
            if (!saveDoneInfo.Successful)
            {
                Debug.Log(saveDoneInfo.Exception);
            }
        }
        public void SetInterface(object o)
        {
            if (typeof(IDoSave).IsAssignableFrom(o.GetType()))
            {
                _IDoSave = (IDoSave)o;
            }
        }
        public byte[] GetByteArray()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, _Dictionary);
            return memoryStream.ToArray();
        }
        public ISavedGameMetadata GetSavedGameMetadata()
        {
            return _MySavedGameMetadata;
        }
        public Texture2D GetSavedGameImage()
        {
            return _Screenshot;
        }
        public TimeSpan GetPlayedTimespan()
        {
            return DateTime.Now - StartDateTime;
        }
    }
}
