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
    public class Note:IDisposable
    {
        public enum NoteType { Clef, OtherRan }
        private GameObject gameObject;
        private bool _Destroyed = false;
        public Note(NoteType noteType, GameObject parent, Vector3 position)
        {
            string prefabName;
            switch (noteType)
            {
                case NoteType.Clef:
                    prefabName = "clef";
                    break;
                default:
                    switch(Random.Range(0, 3))
                    {
                        case 0:
                            prefabName = "note0";
                            break;
                        case 1:
                            prefabName = "note1";
                            break;
                        default:
                            prefabName = "note2";
                            break;
                    }
                    break;
            }
            gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Moosic/"+prefabName));
            gameObject.transform.parent = parent.transform;
            gameObject.transform.position = position;
        }
        public void SetPosition(Vector2 position)
        {
            gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
        }
        public void Dispose()
        {
            if(!_Destroyed)
                UnityEngine.Object.Destroy(gameObject);
            _Destroyed = true;
        }
    }
}