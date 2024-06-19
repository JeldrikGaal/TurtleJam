public class RangeDecreasePowerUpLogic : BasePowerUpLogic
{
    private RangeDecreasePowerUpHolder _decreaseData;
    
    protected override void Start()
    {
        base.Start();
        _decreaseData = (RangeDecreasePowerUpHolder)_data;
    }

    protected override void ActivatePowerUp()
    {
        base.ActivatePowerUp();
        _playerController.RequestRangeChange(_decreaseData.RangeDecrease);
    }
   
    protected override void DeactivatePowerUp()
    {
        base.DeactivatePowerUp();
        _playerController.RequestRangeReset();
    }
}
