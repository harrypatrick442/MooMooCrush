using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace Assets.Scripts
{
    class UFOs : MonoBehaviour, IPausible
    {
        private INonVolatileData _INonVolatileData;
        private ICameraController _ICameraController;
        private CameraConfiguration _CameraConfiguration;// = new CameraConfiguration(0, 1.5f, 1.589392f);
        private IShow _IShowInstructions;
        public UFOSpawnSite[] SpawnSites
        {
            get
            {
                return GetComponentsInChildren<UFOSpawnSite>(true);
            }
        }
        private IResourceHelper _InterfacesHelper;
        private bool? _DoneInstruction;
        private bool DoneInstruction
        {
            get
            {
                if (_DoneInstruction == null)
                {
                    if (_INonVolatileData != null)
                    {
                        object o = _INonVolatileData["done_ufos_instruction"];
                        if (o != null)
                            try
                            {
                                _DoneInstruction = (bool)o;
                                return (bool)_DoneInstruction;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError(ex);
                            }
                    }
                    return false;
                }
                return (bool)_DoneInstruction;
            }
            set
            {
                _DoneInstruction = value;
                if (_INonVolatileData != null)
                {
                    _INonVolatileData["done_ufos_instruction"] = value;
                }
            }
        }
        public void Awake()
        {

        }
        public void Start()
        {
        }
        private List<UFO> list = new List<UFO>();
        public int Count
        {
            get
            {
                return list.Count;
            }
        }
        public void Spawn(UFOInfo ufoInfo)
        {
            UFO ufo = new UFO(gameObject.transform, ufoInfo, _InterfacesHelper);
            list.Add(ufo);
            if (list.Count < 2)
            {
                if (!DoneInstruction)
                {
                    Instruct();
                    DoneInstruction = true;
                }
            }
        }
        private void Instruct()
        {
            return;
            new Thread(() =>
            {
                ICameraConfiguration previousCameraConfiguration = null;
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    previousCameraConfiguration = _ICameraController.GetConfiguration();
                    _ICameraController.SetConfiguration(_CameraConfiguration);
                });
                Thread.Sleep(3000);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    _ICameraController.SetConfiguration(previousCameraConfiguration);
                });
                Thread.Sleep(2000);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    _IShowInstructions.Show();
                });
            }).Start();
        }
        public void Remove(UFO ufo)
        {
            list.Remove(ufo);
        }

        public void SetInterface<T>(T o)
        {
            Type type = o.GetType();
            if (typeof(IResourceHelper).IsAssignableFrom(type))
            {
                _InterfacesHelper = (IResourceHelper)o;
            }

            if (typeof(ICameraController).IsAssignableFrom(type))
                _ICameraController = (ICameraController)o;
            if (typeof(INonVolatileData).IsAssignableFrom(type))
                _INonVolatileData = (INonVolatileData)o;
            if (typeof(IShow).IsAssignableFrom(typeof(T)))
                _IShowInstructions = (IShow)o;
        }
        public void Pause()
        {
            foreach (UFO ufo in list)
            {
                ufo.Pause();
            }
        }
        public void Unpause()
        {
            foreach (UFO ufo in list)
            {
                ufo.Unpause();
            }
        }
    }


}