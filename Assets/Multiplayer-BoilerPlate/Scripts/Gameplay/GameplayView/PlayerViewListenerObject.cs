using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerViewListenerObject : MonoBehaviour
{
   protected virtual void Awake()
   {
      GameEvents.GameplayEvents.LocalPlayerJoined.Register(OnLocalPlayerJoined);
   }

   protected virtual void OnDestroy()
   {
      GameEvents.GameplayEvents.LocalPlayerJoined.UnRegister(OnLocalPlayerJoined);
   }

   protected abstract void OnLocalPlayerJoined(PlayerViewDataObject viewDataObject);
}
