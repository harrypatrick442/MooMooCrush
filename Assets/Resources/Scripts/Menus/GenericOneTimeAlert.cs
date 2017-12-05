using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Assets.Scripts
{
    public class GenericOneTimeAlert : GenericAlert
    {
        private String _DoneBooleanSaveName;
        private INonVolatileData _INonVolatileData;
        private bool? _DoneInstruction;
        private bool DoneInstruction
        {
            get
            {
                if (_DoneInstruction == null)
                {
                    if (_INonVolatileData != null)
                    {
                        object o = _INonVolatileData[_DoneBooleanSaveName];
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
                    Debug.Log("saved");
                    _INonVolatileData[_DoneBooleanSaveName] = value;
                }
            }
        }
        public GenericOneTimeAlert(String doneBooleanSaveName):base()
        {
            _DoneBooleanSaveName=doneBooleanSaveName;
        }
        public override void Show()
        {
            if (!DoneInstruction)
            {
                base.Show();
                DoneInstruction = true;
            }
        }
        public override void Hide()
        {
            base.Hide();
        }
        public override void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(INonVolatileData).IsAssignableFrom(type))
                _INonVolatileData = (INonVolatileData)o;
            base.SetInterface(o);
        }
    }
}