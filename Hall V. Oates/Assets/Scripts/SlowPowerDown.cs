
using UnityEngine;

public class SlowPowerDown : PowerUp
{
    private void Awake()
    {
        duration = 3.3f;
    }

    protected override void Apply(PlayerController player)
    {
        player.SetPowerDown(true);
    }

    protected override void Remove(PlayerController player)
    {
        player.SetPowerDown(false);
    }
}
