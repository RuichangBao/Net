using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator>, IInit
{
    private Dictionary<Type, object> m_services;
    public ServiceLocator()
    {
        m_services = new Dictionary<Type, object>();
    }

    void IInit.Init()
    {

    }
    public T Singleton<T>()
    {
        Type t = typeof(T);
        if(m_services.ContainsKey(t))
        {
            return (T)m_services[t];
        }
        object obj =  Activator.CreateInstance(t);
        m_services[t] = obj;
        return (T)obj;
    }
}