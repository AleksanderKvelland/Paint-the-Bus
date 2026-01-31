using System;
using System.Diagnostics;

public class UpgradeEventsController
{
    public event Action onMoveSpeedUpgrade;
    public event Action onTruckMoveUpgrade;
    public event Action onFireRateUpgrade;
    public event Action onTapeGunUpgrade;
    private static UpgradeEventsController instance;

    public static UpgradeEventsController GetUpgradeEventsController()
    {
        if (instance == null)
        {
            instance = new UpgradeEventsController();
        }
        return instance;
    }

    public void TriggerMoveSpeedUpgrade()
    {
        onMoveSpeedUpgrade?.Invoke();
    }

    public void TriggerTruckMoveUpgrade()
    {
        onTruckMoveUpgrade?.Invoke();
    }

    public void TriggerFireRateUpgrade()
    {
        onFireRateUpgrade?.Invoke();
    }

    public void TriggerTapeGunUpgrade()
    {
        onTapeGunUpgrade?.Invoke();
    }
}