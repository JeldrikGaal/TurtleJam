using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAmountChangePowerUpLogic : BasePowerUpLogic
{
   private BounceAmountChangePowerUpHolder _bounceData;

   protected override void Start()
   {
      base.Start();
      _bounceData = (BounceAmountChangePowerUpHolder)_data;
   }

   protected override void ActivatePowerUp()
   {
      base.ActivatePowerUp();
      _playerController.RequestBounceAmountChange(_bounceData.NewBounceAmount);
   }
   
   protected override void DeactivatePowerUp()
   {
      base.DeactivatePowerUp();
      _playerController.RequestBounceAmountReset();
   }
}
