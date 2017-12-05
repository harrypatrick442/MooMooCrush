using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class ControlButtonsConfigurableHandler : MonoBehaviour
    {
        private bool _Set = false;
        public List<ControlButtonConfigurable> _Buttons = new List<ControlButtonConfigurable>();
        public List<ControlButtonConfigurable> Buttons
        {
            get
            {
                return _Buttons;
            }
            set
            {
                    _Buttons = value;
            }
        }
        public List<ControlButtonConfiguration> Configurations = new List<ControlButtonConfiguration>();
        public ControlButtonConfiguration DefaultConfiguration;
        private ControlButtonConfiguration GetConfigurationFromName(string name)
        {
            foreach (ControlButtonConfiguration configuration in Configurations)
                if (configuration.name.Equals(name))
                    return configuration;
            if (DefaultConfiguration != null)
                if (DefaultConfiguration.name.Equals(name))
                    return DefaultConfiguration;
            return null;
        }
        public ControlButton GetButton(string name)
        {
            foreach (ControlButtonConfigurable controlButton in Buttons)
            {
                if (controlButton.CurrentControlButtonConfiguration != null)
                {
                    if (controlButton.CurrentControlButtonConfiguration.name.Equals(name))
                        return controlButton;
                }
            }
            return null;
        }
        public void Set(String[] names)
        {
            _Set = true;
            int buttonIndex = 0;
            List<int> toDefault = new List<int>();
            foreach (string name in names)
            {
                if (buttonIndex >= Buttons.Count)
                    return;
                if (name != null)
                {
                    ControlButtonConfiguration configuration = GetConfigurationFromName(name);
                    if (configuration != null)
                    {
                        Buttons[buttonIndex].SetConfiguration(configuration);
                    }
                    else
                    {
                        toDefault.Add(buttonIndex);
                    }
                }
                else
                {
                    toDefault.Add(buttonIndex);
                }
                buttonIndex++;
            }
            if (DefaultConfiguration != null)
            {
                while (buttonIndex < Buttons.Count)
                {
                    toDefault.Add(buttonIndex);
                    buttonIndex++;
                }
                foreach (int index in toDefault)
                {
                    Buttons[index].SetConfiguration(DefaultConfiguration);
                }
            }
        }
        public void SetCallbacks(string name, Action goneDown, Action goneUp)
        {
            ControlButtonConfiguration configuration = GetConfigurationFromName(name);
            if (configuration != null)
            {
                configuration._GoneDown = goneDown;
                configuration._GoneUp = goneUp;
            }
            foreach(ControlButtonConfigurable controlButtonConfigurable in   Buttons)
            {
                if (controlButtonConfigurable.CurrentControlButtonConfiguration != null)
                {
                    if (controlButtonConfigurable.CurrentControlButtonConfiguration.name.Equals(name))
                    { 
                        controlButtonConfigurable.SetConfiguration(configuration);
                    }
                }
                else
                {
                    if (DefaultConfiguration.Equals(configuration))
                    {
                        controlButtonConfigurable.SetConfiguration(configuration);
                    }
                }
            }
        }
        private void Start()
        {
            if(!_Set)
            Set(new string[] { });
        }
    }
}
