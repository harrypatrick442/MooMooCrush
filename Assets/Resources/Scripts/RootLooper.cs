using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class RootLooper : ILooper
    {
        private List<ILooperUpdate> updates = new List<ILooperUpdate>();
        private List<ILooperUpdate> updateRemoves = new List<ILooperUpdate>();
        private List<ILooper5Hz> updates5Hz = new List<ILooper5Hz>();
        private List<ILooper5Hz> update5HzRemoves = new List<ILooper5Hz>();
        private List<ILooper2Hz> updates2Hz = new List<ILooper2Hz>();
        private List<ILooper2Hz> update2HzRemoves = new List<ILooper2Hz>();
        private List<ILooper1Hz> updates1Hz = new List<ILooper1Hz>();
        private List<ILooper1Hz> update1HzRemoves = new List<ILooper1Hz>();
        private List<ILooperFixedUpdate> fixedUpdates = new List<ILooperFixedUpdate>();
        private List<ILooperFixedUpdate> fixedUpdateRemoves = new List<ILooperFixedUpdate>();
        public void AddFixedUpdate(ILooperFixedUpdate iLooperFixedUpdate)
        {
            if (!fixedUpdates.Contains(iLooperFixedUpdate))
            {
                fixedUpdates.Add(iLooperFixedUpdate);
            }
        }
        public void AddUpdate(ILooperUpdate iLooperUpdate)
        {
            if (!updates.Contains(iLooperUpdate))
            {
                updates.Add(iLooperUpdate);
            }
        }
        public void Add1Hz(ILooper1Hz iLooper1Hz)
        {
            if (!updates1Hz.Contains(iLooper1Hz))
            {
                updates1Hz.Add(iLooper1Hz);
            }
        }
        public void Add2Hz(ILooper2Hz iLooper2Hz)
        {
            if (!updates2Hz.Contains(iLooper2Hz))
            {
                updates2Hz.Add(iLooper2Hz);
            }
        }
        public void Add5Hz(ILooper5Hz iLooper5Hz)
        {
            if (!updates5Hz.Contains(iLooper5Hz))
            {
                updates5Hz.Add(iLooper5Hz);
            }
        }
        public void RemoveUpdate(ILooperUpdate iLooperUpdate)
        {
            updateRemoves.Add(iLooperUpdate);
        }
        public void Remove5Hz(ILooper5Hz iLooperUpdate)
        {
            update5HzRemoves.Add(iLooperUpdate);
        }
        public void Remove2Hz(ILooper2Hz iLooperUpdate)
        {
            update2HzRemoves.Add(iLooperUpdate);
        }
        public void Remove1Hz(ILooper1Hz iLooperUpdate)
        {
            update1HzRemoves.Add(iLooperUpdate);
        }
        public void RemoveFixedUpdate(ILooperFixedUpdate iLooperFixedUpdate)
        {
            fixedUpdateRemoves.Add(iLooperFixedUpdate);
        }
        public void Update()
        {
            int i = 0;
            foreach (ILooperUpdate iLooperUpdate in updateRemoves)
                updates.Remove(iLooperUpdate);
            int count = updates.Count;
            while (i < count)
            {
                ILooperUpdate iLooperUpdate = updates[i];
                try
                {
                    if (iLooperUpdate.LooperUpdate())
                    {
                        count--;
                        updates.Remove(iLooperUpdate);
                    }
                    else
                    {
                        i++;
                    }

                }
                catch (Exception ex)
                {
                    i++;
                    Debug.Log(ex.StackTrace);
                    updateRemoves.Add(iLooperUpdate);
                }

            }
        }
        private int i2Hz = 0;
        private int i5Hz = 0;
        private int i1Hz = 0;
        public void FixedUpdate()
        {
            int i = 0;
            foreach (ILooperFixedUpdate iLooperFixedUpdate in fixedUpdateRemoves)
                fixedUpdates.Remove(iLooperFixedUpdate);
            fixedUpdateRemoves.Clear();
            int count = fixedUpdates.Count;
            while (i < count)
            {
                ILooperFixedUpdate iLooperFixedUpdate = fixedUpdates[i];
                try
                {
                    if (iLooperFixedUpdate.LooperFixedUpdate())
                    {
                        count--;
                        fixedUpdates.Remove(iLooperFixedUpdate);
                    }
                    else
                    {
                        i++;
                    }

                }
                catch (Exception ex)
                {
                    i++;
                    Debug.LogError(ex)
                    ; fixedUpdateRemoves.Add(iLooperFixedUpdate);
                }
            }
            if (i5Hz >= 10)
            {
                i5Hz = 0;
                Update5Hz();
            }
            else
                i5Hz++;
            if (i2Hz>=25)
            {
                i2Hz = 0;
                if (i1Hz > 0)
                {
                    Update1Hz();
                    i1Hz = 0;
                }
                else
                    i1Hz++;
                Update2Hz();
            }
            else
                i2Hz++;
        }
        public void Update5Hz()
        {
            int i = 0;
            foreach (ILooper5Hz iLooper5Hz in update5HzRemoves)
                updates5Hz.Remove(iLooper5Hz);
            update5HzRemoves.Clear();
            int count = updates5Hz.Count;
            while (i < count)
            {
                ILooper5Hz iLooper5Hz = updates5Hz[i];
                try
                {
                    if (iLooper5Hz.Looper5Hz())
                    {
                        count--;
                        updates5Hz.Remove(iLooper5Hz);
                    }
                    else
                    {
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    i++;
                    Debug.LogError(ex);
                    update5HzRemoves.Add(iLooper5Hz);
                }
            }
        }
        public void Update2Hz()
        {
            int i = 0;
            foreach (ILooper2Hz iLooper2Hz in update2HzRemoves)
                updates2Hz.Remove(iLooper2Hz);
            update2HzRemoves.Clear();
            int count = updates2Hz.Count;
            while (i < count)
            {
                ILooper2Hz iLooper2Hz = updates2Hz[i];
                try
                {
                    if (iLooper2Hz.Looper2Hz())
                    {
                        count--;
                        updates2Hz.Remove(iLooper2Hz);
                    }
                    else
                    {
                        i++;
                    }

                }
                catch (Exception ex)
                {
                    i++;
                    Debug.LogError(ex);
                    update2HzRemoves.Add(iLooper2Hz);
                }
            }
        }
        public void Update1Hz()
        {
            int i = 0;
            foreach (ILooper1Hz iLooper1Hz in update1HzRemoves)
                updates1Hz.Remove(iLooper1Hz);
            update1HzRemoves.Clear();
            int count = updates1Hz.Count;
            while (i < count)
            {
                ILooper1Hz iLooper1Hz = updates1Hz[i];
                try
                {
                    if (iLooper1Hz.Looper1Hz())
                    {
                        count--;
                        updates1Hz.Remove(iLooper1Hz);
                    }
                    else
                    {
                        i++;
                    }

                }
                catch (Exception ex)
                {
                    i++;
                    Debug.LogError(ex);
                    update1HzRemoves.Add(iLooper1Hz);
                }
            }
        }
    }
}
