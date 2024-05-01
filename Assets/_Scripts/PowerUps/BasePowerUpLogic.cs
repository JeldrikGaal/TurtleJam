using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BasePowerUpLogic : MonoBehaviour
{

   [SerializeField] protected BasePowerUpHolder _data;

   protected PlayerController _playerController;
   
   
   private bool _activated;
   private float _startingTme;
   private BoxCollider2D _boxCollider2D;
   private UIManager _powerUpUI;

   private void OnEnable()
   {
      EnemyController.EnemyDeath += EnemyKill;
   }

   private void OnDestroy()
   {
      EnemyController.EnemyDeath -= EnemyKill;
   }

   protected virtual void Start()
   {
      _boxCollider2D = GetComponent<BoxCollider2D>();
      _playerController = FindObjectOfType<PlayerController>();
      _powerUpUI = FindObjectOfType<UIManager>();
   }

   
   protected virtual void Update()
   {
      if (_activated)
      {
         if (IsDurationOver())
         {
            DeactivatePowerUp();
         }
         else
         {
            _powerUpUI.PositionBarToPercent(GetCurrentProgressPercentage());
         }
      }
   }

   private float GetCurrentProgressPercentage()
   {
      return 1 - (Time.time - _startingTme) / _data.InitialDuration;
   }
   

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player") && !_activated)
      {
         ActivatePowerUp();
      }
   }

   protected virtual void ActivatePowerUp()
   {
      _activated = true;
      StartTimer();
      _powerUpUI.ShowPowerUpUI(_data);
      HidePowerUp();
   }
   
   private void HidePowerUp()
   {
      _boxCollider2D.enabled = false;
      // TODO: Hide visuals properly
      GetComponent<SpriteRenderer>().enabled = false;
   }
   
   protected virtual void DeactivatePowerUp()
   {
      _activated = false;
      _powerUpUI.HidePowerUpUI();
   }
   
   public void EnemyKill()
   {
      RefreshTimer(_data.DurationAddedEnemyKill);
   }

   public void LevelCompleted()
   {
      RefreshTimer(_data.DurationAddedLevelCompleted);
   }
   
   private void RefreshTimer(float time)
   {
      _startingTme += time;
   }

   private void StartTimer()
   {
      _startingTme = Time.time;
   }

   private bool IsDurationOver()
   {
      return Time.time - _startingTme > _data.InitialDuration;
   }
}
