using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TeleportationSimulationHandler:MonoBehaviour, ITeleportationSimulationHandler, ILooperFixedUpdate
    {
        private class Simulation
        {
            private enum State { LeavingOnly, Both, ArrivingOnly }
            private State _State = State.LeavingOnly;
            private GameObject gameObjectLeaving;
            private GameObject gameObjectArriving;
            private const float SCALE_CONSTANT = 2.600f;
            private const float Z = -6;
            private float _ScaleTo;
            private float _StartTime;
            private Action _CallbackBoth;
            public Simulation(GameObject parent, Vector2 from, Vector2 to, float radius, Action callbackBoth)
            {
                _ScaleTo = radius*SCALE_CONSTANT;
                _StartTime = Time.time;
                gameObjectLeaving = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/teleportation_sphere"));
                gameObjectLeaving.transform.parent = parent.transform;
                gameObjectLeaving.transform.position = new Vector3(from.x, from.y, Z);
                gameObjectArriving = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/teleportation_sphere"));
                gameObjectArriving.transform.parent = parent.transform;
                gameObjectArriving.transform.localPosition = new Vector3(to.x, to.y, Z);
                gameObjectArriving.SetActive(false);
                _CallbackBoth = callbackBoth;
            }
            public bool Update()
            {
                switch (_State)
                {
                    case State.LeavingOnly:
                        float a= (Time.time - _StartTime) / 0.25f;
                        if (a >= 1)
                        {
                            _State = State.Both;
                            _StartTime = Time.time;
                            gameObjectLeaving.transform.localScale = new Vector3(_ScaleTo, _ScaleTo, 1);
                            gameObjectArriving.transform.localScale = new Vector3(_ScaleTo, _ScaleTo, 1);
                            gameObjectArriving.SetActive(true);
                            if(_CallbackBoth!=null)
                                _CallbackBoth();
                        }
                        else
                        {
                            float scale = _ScaleTo * a;
                            gameObjectLeaving.transform.localScale = new Vector3(scale, scale, 1);
                        }
                        break;
                    case State.Both:
                        if(Time.time- _StartTime>0.1)
                        {
                            _State = State.ArrivingOnly;

                            _StartTime = Time.time;
                        }
                        break;
                    default :

                        float b = (Time.time - _StartTime) / 0.25f;
                        if (b >= 1)
                        {
                            _State = State.Both;
                            _StartTime = Time.time;
                            gameObjectArriving.SetActive(false);
                            UnityEngine.Object.Destroy(gameObjectArriving);
                            UnityEngine.Object.Destroy(gameObjectLeaving);
                            return true;
                        }
                        else
                        {
                            float scale = _ScaleTo * (1-b);
                            gameObjectLeaving.transform.localScale = new Vector3(scale, scale, 1);
                        }
                        break;
                }
                return false;
            }
        }
        private ILooper _ILooper;
        private List<Simulation> _Simulations = new List<Simulation>();
        public void SimulateTeleportation(Vector2 from, Vector2 to, float radius, Action callbackSwap)
        {
            _Simulations.Add(new Simulation(gameObject, from, to, radius, callbackSwap));
            _ILooper.AddFixedUpdate(this);
        }
        public bool LooperFixedUpdate()
        {
            int count = _Simulations.Count;
            int i = 0;
            while(i<count)
            {
                Simulation simulation = _Simulations[i];
                if (simulation.Update())
                {
                    _Simulations.Remove(simulation);
                    count--;
                }
                else i++;
            }
            return count < 1;
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