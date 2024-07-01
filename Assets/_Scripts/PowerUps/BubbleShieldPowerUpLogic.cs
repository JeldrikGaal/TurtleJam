public class BubbleShieldPowerUpLogic : BasePowerUpLogic
{
   private BubbleShieldPowerUpHolder _shieldData;

   private void Awake()
   {
      PlayerController.NewBubbleShieldValueJustDropped += NoWayTheyDroppedANewShieldValue;
   }
   protected override void OnDestroy()
   {
      base.OnDestroy();
      PlayerController.NewBubbleShieldValueJustDropped -= NoWayTheyDroppedANewShieldValue;
   }

   protected override void Start()
   {
      base.Start();
      _shieldData = (BubbleShieldPowerUpHolder)_data;
   }

   private void NoWayTheyDroppedANewShieldValue(int newShieldValue)
   {
      if (newShieldValue <= 0 )
      {
         DeactivatePowerUp();
      }
   }
   
   protected override void ActivatePowerUp()
   {
      base.ActivatePowerUp();
      // TODO: again decide on float or int 
      _playerController.RequestShieldAdd((int)_shieldData.bubbleAmount);
   }
   
   protected override void DeactivatePowerUp()
   {
      base.DeactivatePowerUp();
      _playerController.RequestShieldReset();
     
   }
}
