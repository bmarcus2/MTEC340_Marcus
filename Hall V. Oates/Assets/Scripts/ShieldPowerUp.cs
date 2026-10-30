using UnityEngine;

public class ShieldPowerUp : PowerUp
{
    protected override void Apply(PlayerController player)
    {
        player.ShieldActive = true;
    }

    protected override void Remove(PlayerController player)
    {
        player.ShieldActive = false;
    }
}
