using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Assets.Scripts
{
    public class SaveDoneInfo
    {
        public Exception Exception;
        public bool Successful
        {
            get
            {
                return Exception == null;
            }
        }
        public SaveDoneInfo(Exception ex)
        {
            Exception = ex;
        }
        public SaveDoneInfo()
        {

        }
    }
}