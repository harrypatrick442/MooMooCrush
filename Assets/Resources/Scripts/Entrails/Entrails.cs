using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public abstract class Entrails
    {
        private const float FORCE = 200f;
        public virtual int MinNComponents { get; }
        public virtual int MaxNComponents { get; }
        public virtual int Duration { get { return 2; } }
        private List<EntrailsItem> _EntrailsItems = new List<EntrailsItem>();
        public virtual List<EntrailsComponent> EntrailsComponents { get; }
        public class EntrailsComponent
        {
            public int MaxInstances;
            public Type Type;
            public EntrailsComponent(Type type, int maxInstances)
            {
                Type = type;
                MaxInstances = maxInstances;
            }
        }
        public Entrails()
        {
        }
        public void Initialize(Transform transform, Vector2 position) {
            int nEntrails = Random.Range(MinNComponents, MaxNComponents  +1);
            int i = 0;
            List<Tuple<EntrailsComponent, Int32>> entrailsComponentAndCount = new List<Tuple<EntrailsComponent, Int32>>();
            foreach(EntrailsComponent entrailsComponent in EntrailsComponents)
            {
                entrailsComponentAndCount.Add(new Tuple<EntrailsComponent,Int32>(entrailsComponent, 0));
            }
            int entrailsComponentCount = entrailsComponentAndCount.Count;
            Vector2 forceVector = new Vector2(FORCE, FORCE);
            while (i < nEntrails && entrailsComponentCount > 0)
            {
                int entrailsComponentIndex = Random.Range(0, entrailsComponentCount);
                Tuple<EntrailsComponent, int> tuple = entrailsComponentAndCount[entrailsComponentIndex];
                tuple.B++;
                if(tuple.B>=tuple.A.MaxInstances)
                {
                    entrailsComponentAndCount.Remove(tuple);
                    entrailsComponentCount--;
                }
                EntrailsItem entrailsItem = (EntrailsItem)Activator.CreateInstance(tuple.A.Type);
                float angle = Random.Range(-Mathf.PI, Mathf.PI);
                Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                force.Normalize();
                force.Scale(forceVector);
                entrailsItem.Initialize(transform, position, force);
                _EntrailsItems.Add(entrailsItem);
                i++;
            }
        }
        public void Destroy()
        {
            foreach (EntrailsItem entrailsItem in _EntrailsItems)
                entrailsItem.Destroy();
        }
    }
}
