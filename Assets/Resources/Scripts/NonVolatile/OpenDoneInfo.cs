using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Assets.Scripts
{
    public class OpenDoneInfo<T>
    {
        public T Data;
        public Exception Exception;
        public bool Successful
        {
            get
            {
                return Exception == null;
            }
        }
        public OpenDoneInfo(Exception ex)
        {
            Exception = ex;
        }
        public OpenDoneInfo(T data)
        {
            Data = data;
        }
    }

}