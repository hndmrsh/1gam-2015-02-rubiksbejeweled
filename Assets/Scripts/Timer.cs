using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer : MonoBehaviour {

    public interface TimerListener
    {
        void TimerTick(float remainingTime);
    }

    private float remainingTime;
    private bool started;

    private List<TimerListener> listeners;

    void Start()
    {
        remainingTime = 0f;
        started = false;
    }

    public void StartTimer(float startingTime)
    {
        remainingTime = startingTime;
        started = true;
    }

    void Update()
    {
        if (started)
        {
            remainingTime -= Time.deltaTime;

            if (listeners != null && listeners.Count > 0)
            {
                foreach (TimerListener l in listeners)
                {
                    l.TimerTick(remainingTime);
                }
            }

            if (remainingTime <= 0f)
            {
                started = false;
                remainingTime = 0f;
            }
        }
    }

    public void AddTime(float time)
    {
        remainingTime += time;
    }

    public void RegisterListener(TimerListener listener)
    {
        if (listeners == null)
        {
            listeners = new List<TimerListener>();
        }

        listeners.Add(listener);
    }

    public bool UnregisterListener(TimerListener listener)
    {
        if (listeners != null)
        {
            return listeners.Remove(listener);
        }

        return false;
    }

}
