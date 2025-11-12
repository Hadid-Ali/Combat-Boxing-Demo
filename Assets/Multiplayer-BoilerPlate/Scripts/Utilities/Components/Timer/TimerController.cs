using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TimerController : MonoBehaviour
{
   private float m_TimeToWait = 0;

   private Coroutine m_RequestRoutine = null;
   private GameEvent m_OnTimerCompletedEvent = new();
   private GameEvent<string> m_OnTimerTickEvent = new();

   private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();
   
   private void Start()
   {
      DontDestroyOnLoad(gameObject);
   }
   
   private void OnEnable()
   {
      GameEvents.TimerEvents.ExecuteActionRequest.Register(OnExecuteActionRequest);
      GameEvents.TimerEvents.CancelActionRequest.Register(OnRequestCancel);
   }

   private void OnDisable()
   {
      GameEvents.TimerEvents.ExecuteActionRequest.UnRegister(OnExecuteActionRequest);
      GameEvents.TimerEvents.CancelActionRequest.UnRegister(OnRequestCancel);
   }
   

    private void OnExecuteActionRequest(TimerDataObject timerDataObject)
    {
        // Use a unique identifier for this timer (e.g., the title or a generated ID)
        string timerId = timerDataObject.Title; 

        // Check if there's already an active timer with the same ID and don't start a new one
        if (activeCoroutines.ContainsKey(timerId)) return;

        // Start a new timer coroutine and add it to the dictionary
        Coroutine newTimer = StartCoroutine(StartTimer(timerDataObject, timerId));
        activeCoroutines[timerId] = newTimer;
    }

    private void OnRequestCancel(string timerId)
    {
        // Check if a timer with the given ID exists, and cancel it if found
        if (!activeCoroutines.TryGetValue(timerId, out Coroutine coroutine)) return;
        StopCoroutine(coroutine);
        activeCoroutines.Remove(timerId);
        print("Request Timer Found and cancelled");
    }

    private IEnumerator StartTimer(TimerDataObject timerDataObject, string timerId)
    {
        float time = timerDataObject.TimeDuration;
        Action onComplete = timerDataObject.ActionToExecute;
        Action<string> onTick = timerDataObject.TickTimeEvent;

        while (time > 0)
        {
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            string timerString = $"{minutes:0}:{seconds:00}";

            onTick?.Invoke(timerString);
            yield return new WaitForSeconds(1f);
            time--;
        }

        // When time runs out
        onTick?.Invoke("0:00");
        onComplete?.Invoke();

        // Remove the completed timer from the dictionary
        activeCoroutines.Remove(timerId);
    }
    


}
