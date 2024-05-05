using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StartingRoomPowerupInteraction : MonoBehaviour
{
   [FormerlySerializedAs("_text")] [SerializeField] private GameObject _textObject;

   private void Awake()
   {
      BasePowerUpLogic.PowerUpActivated += DestroyObjects;
   }

   private void OnDestroy()
   {
      BasePowerUpLogic.PowerUpActivated -= DestroyObjects;
   }

   private void DestroyObjects(BasePowerUpLogic b)
   {
      _textObject.SetActive(false);
      foreach (var VARIABLE in transform.GetComponentsInChildren<BasePowerUpLogic>())
      {
         if (VARIABLE != b)
         {
            Destroy(VARIABLE.gameObject);
         }
      }
   }
}
