using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class ControlButtonConfigurable : ControlButton
    {
        public List<ControlButtonConfiguration> Configurations;
        public ControlButtonConfiguration CurrentControlButtonConfiguration;
        private ControlButtonConfiguration GetConfiguration(String name)
        {
            foreach (ControlButtonConfiguration c in Configurations)
            {
                if (c.name.Equals(name))
                {
                    return c;
                }
            }
            return null;
        }
        public void SetConfiguration(string name)
        {
            ControlButtonConfiguration c = GetConfiguration(name);
            if (c == null)
                return;
            SetConfiguration(c);
        }
        public void SetConfiguration(ControlButtonConfiguration c)
        {
            CurrentControlButtonConfiguration = c;
            Type = c.Type;
            SpriteActiveDown = c.SpriteActiveDown;
            SpriteActiveUp = c.SpriteActiveUp;
            SpriteDown = c.SpriteDown;
            SpriteUp = c.SpriteUp;
            if(c._GoneDown!=null)
            _GoneDown = c._GoneDown;
            if (c._GoneUp != null)
            _GoneUp = c._GoneUp;
            Setup();
        }
        public void SetCallbacks(string name, Action goingDown, Action goingUp)
        {
            ControlButtonConfiguration c = GetConfiguration(name);
            if (c == null)
                return;
            c._GoingDown = goingDown;
            c._GoingUp = goingUp;
        }
    }
}
