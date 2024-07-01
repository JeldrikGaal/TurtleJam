public class SpeedIncreasePowerUpLogic : BasePowerUpLogic
{
   private SpeedIncreasePowerUpHolder _speedData;

   protected override void OnDestroy()
   {
      base.OnDestroy();
   }
   
   protected override void Start()
   {
      base.Start();
      _speedData = (SpeedIncreasePowerUpHolder)_data;
   }

   protected override void ActivatePowerUp()
   {
      base.ActivatePowerUp();
      _playerController.RequestSpeedChange(_speedData.SpeedIncrease);
   }
   
   protected override void DeactivatePowerUp()
   {
      base.DeactivatePowerUp();
      _playerController.RequestSpeedReset();
   }
}
