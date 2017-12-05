using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public class Cows : MonoBehaviour, IPausible
    {
        private ResourcesHelper _InterfacesHelper;
        private ISuperMooMarkers _ISuperMooMarkers;
        private IDevilCowMarkers _IDevilCowMarkers;
        private IShowAlertFeedSuperMooCow _IShowAlertFeedSuperMooCow;
        public void Awake()
        {

        }
        public void Start()
        {
        }
        private List<Cow> list = new List<Cow>();
        public int Count
        {
            get
            {
                return list.Count;
            }
        }
        public void Spawn(Vector3 spawnSite, CowInfo cowInfo)
        {
            Cow cow;
            switch (cowInfo.CowType)
            {
                case Enums.CowType.Devil:
                    cow = new DevilCow(_IDevilCowMarkers.AllowedRegionsDevilCow, spawnSite, this, _InterfacesHelper, Remove, cowInfo);
                    break;
                case Enums.CowType.SuperMoo:
                    cow = new SuperMooCow(_ISuperMooMarkers.AllowedRegionsSuperMoo, spawnSite, this, _InterfacesHelper, Remove, _IShowAlertFeedSuperMooCow, cowInfo);
                    break;
                default:
                    cow = new Cow(spawnSite, this, _InterfacesHelper, Remove, cowInfo);
                    break;
            }
            list.Add(cow);
        }
        public void Remove(Cow cow)
        {
            list.Remove(cow);
        }
        public void SetInterface(object o)
        {
            if (typeof(ResourcesHelper).IsAssignableFrom(o.GetType()))
            {
                _InterfacesHelper = (ResourcesHelper)o;
                _ISuperMooMarkers = _InterfacesHelper.Get<ISuperMooMarkers>();
                _IDevilCowMarkers = _InterfacesHelper.Get<IDevilCowMarkers>();
                _IShowAlertFeedSuperMooCow = _InterfacesHelper.Get<IShowAlertFeedSuperMooCow>();
            }
        }
        public void Pause()
        {
            foreach (Cow cow in list)
            {
                cow.Pause();
            }
        }
        public void Unpause()
        {
            foreach (Cow cow in list)
            {
                cow.Unpause();
            }
        }
    }


}