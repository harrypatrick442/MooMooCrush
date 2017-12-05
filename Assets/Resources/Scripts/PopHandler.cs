using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class PopHandler : MonoBehaviour, IPopHandler
    {
        private ILooper _ILooper;
        private IExploder _IExploder;
        private IVibrator _IVibrator;
        public PopHandler(IVibrator iVibrator, ILooper iLooper, IExploder iExploder)
        {
            _IVibrator = iVibrator;
            _ILooper = iLooper;
            _IExploder = iExploder;
        }private void Explode(PopInfo popInfo)
        {
            Vector2? position = popInfo.IPopable.GetPosition2();
            if (position != null)
            {
                popInfo.IPopable.Explode((Vector2)position, 200);
            }
            popInfo.IsDone = true;
        }
        public void Pop(PopInfo popInfo)
        {
            if (popInfo.Vibrate)
            {
                _IVibrator.Vibrate(new Vibratable(popInfo.IPopable, 10f, 0.8f)
                {
                    Callback = new Action(() =>
                    {
                        Explode(popInfo);
                    }),
                    StartAmplitude = 0.00f,
                    EndAmplitude = 0.04f
                });
            }
            else
                Explode(popInfo);
        }
    }
}
