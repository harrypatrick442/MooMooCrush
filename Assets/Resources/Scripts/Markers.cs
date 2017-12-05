using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Markers : ISuperMooMarkers, IDevilCowMarkers
    {
        private GameObject gameObject;
        private Vector2? _SuperMooFlyToStartPosition;
        public Vector2 SuperMooFlyToStartPosition
        {
            get
            {
                if (_SuperMooFlyToStartPosition == null)
                    _SuperMooFlyToStartPosition = MonoBehaviourHelper.GetVectorsFromMarkers(gameObject.transform, new string[] { "Markers", "SpawnSites", "FlyTo", "SuperMoo" })[0];
                return (Vector2)_SuperMooFlyToStartPosition;
            }
        }
        private Vector2[] _DevilCowSpawnSites;
        public Vector2[] DevilCowSpawnSites
        {
            get
            {
                if (_DevilCowSpawnSites == null)
                    _DevilCowSpawnSites = MonoBehaviourHelper.GetVectorsFromMarkers(gameObject.transform, new string[] { "Markers", "SpawnSites", "DevilCow" });
                return _DevilCowSpawnSites;
            }
        }
        private Vector2[] _SuperMooSpawnSites;
        public Vector2[] SuperMooSpawnSites
        {
            get
            {
                if (_SuperMooSpawnSites == null)
                    _SuperMooSpawnSites = MonoBehaviourHelper.GetVectorsFromMarkers(gameObject.transform, new string[] { "Markers", "SpawnSites", "SuperMoo" });
                return _SuperMooSpawnSites;
            }
        }
        private Rect[] _AllowedRegionsSuperMoo;
        public Rect[] AllowedRegionsSuperMoo
        {
            get
            {
                if (_AllowedRegionsSuperMoo == null)
                    _AllowedRegionsSuperMoo = MonoBehaviourHelper.GetRectsFromMarkers(gameObject.transform, new string[] { "Markers", "Constraints", "SuperMoo" });
                return _AllowedRegionsSuperMoo;
            }
        }
        private Rect[] _AllowedRegionsDevilCow;
        public Rect[] AllowedRegionsDevilCow
        {
            get
            {
                return AllowedRegionsSuperMoo;
            }
        }
        public Markers(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }
}
