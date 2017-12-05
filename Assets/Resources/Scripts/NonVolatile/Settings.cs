using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace Assets.Scripts
{
    class Settings
    {
        private string fullPath;
        public Settings(string fullPath)
        {
            this.fullPath = fullPath;
            try
            {
                FileStream fStr = new FileStream(fullPath, FileMode.OpenOrCreate);
                try
                {

                    BinaryFormatter bf = new BinaryFormatter();
                    objects = (Dictionary<string, System.Object>)bf.Deserialize(fStr);

                }
                catch
                {


                }
                fStr.Close();
                fStr.Dispose();
            }
            catch
            {

            }
        }
        private Dictionary<string, System.Object> objects = new Dictionary<string, System.Object>();
        public System.Object getObject(string objectString)
        {
            try
            {
                return objects[objectString];

            }
            catch
            {
                return null;
            }

        }
        public Boolean ReplaceOrAdd(string name, System.Object ojct)
        {
            try
            {
                if (objects.Keys.Contains(name))
                {
                    objects.Remove(name);


                }
                objects.Add(name, ojct);
                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                return false;

            }
        }
        public Boolean Save()
        {
            try
            {
                FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, objects);
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;

            }
        }

    }
}
