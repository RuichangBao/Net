using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class Singleton<T> where T : class, Init, new()
    {
        private static T? instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.Init();
                }
                return instance;
            }
        }
    }
}
