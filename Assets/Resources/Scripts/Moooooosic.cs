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
    public class Moooooosic : ILooperFixedUpdate, IDisposable
    {
        private const float NOTE_SPACING = 0.2F;
        private bool _Done = false;
        private ILooper _ILooper;
        private const float Z = -10;
        private float _Speed;
        private class NoteInfo
        {
            public float Distance = 0;
            private Note _Note;
            public Note Note
            {
                get
                {
                    return _Note;
                }
            }
            public NoteInfo(Note note)
            {
                _Note = note;
            }
        }
        private GameObject _Parent;
        private Vector2 _From;
        private Vector2 _To;
        private float _AngleMultiplier = Mathf.PI / NOTE_SPACING;
        private Matrix2x2 _Transform;
        private float _Distance;
        private List<NoteInfo> _Notes = new List<NoteInfo>();
        public Moooooosic(ILooper iLooper, GameObject parent, Vector2 from, Vector2 to, float speed)
        {
            _Speed = speed;
               _ILooper = iLooper;
            _Parent = parent;
            _ILooper.AddFixedUpdate(this);
            SetPositions(from, to);
        }
        public void Stop()
        {
            _Done = true;
        }
        public void SetPositions(Vector2 from, Vector2 to)
        {
            _From = from;
            _To = to;
            Vector2 distance = to - from;
            _Distance = distance.magnitude;
            float angle = Mathf.Atan2(distance.y, distance.x);
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);
            _Transform = new Matrix2x2(cos, sin, -sin, cos);
        }
        private Vector2 GetPositionFromDistance(float distance)
        {
            float angle = _AngleMultiplier * distance;
            Vector2 untransformed = new Vector2(distance, Mathf.Sin(angle) * NOTE_SPACING/2);
            return _From + (_Transform * untransformed);
        }
        public bool LooperFixedUpdate()
        {
            if (_Done)
            {
                foreach (NoteInfo noteInfo in _Notes)
                {
                    noteInfo.Note.Dispose();
                }
            }
            else
            {
                int count = _Notes.Count;
                if (count > 0)
                {
                    int i = 0;
                    float distanceStep = Time.deltaTime * _Speed;
                    float distancePr = _Distance - NOTE_SPACING;
                    while (i < count)
                    {
                        NoteInfo noteInfo = _Notes[i];
                        if (noteInfo.Distance >= distancePr)
                        {
                            noteInfo.Note.Dispose();
                            _Notes.Remove(noteInfo);
                            count--;
                        }
                        else
                        {
                            noteInfo.Distance += distanceStep;
                            noteInfo.Note.SetPosition(GetPositionFromDistance(noteInfo.Distance));
                            i++;
                        }
                    }
                    if (_Notes.Count > 0)
                    {
                        NoteInfo noteInfo0 = _Notes[0];
                        float spacing = NOTE_SPACING;
                        while (noteInfo0.Distance >= spacing)
                        {
                            Vector2 position2D = GetPositionFromDistance(noteInfo0.Distance - spacing);
                            _Notes.Insert(0, new NoteInfo(new Note(Note.NoteType.OtherRan, _Parent, new Vector3(position2D.x, position2D.y, Z))));
                            spacing += NOTE_SPACING;
                        }
                    }
                }
                else
                {
                    _Notes.Add(new NoteInfo(new Note(Note.NoteType.Clef, _Parent, new Vector3(_From.x, _From.y, Z))));
                }
            }
            return _Done;
        }
        public void Dispose()
        {
            _Done = true;
        }
    }
}