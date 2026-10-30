using UnityEngine;

public class RapidFirePowerUp : PowerUp
{
    protected override void Apply(PlayerController player)
    {
        player.SetRapidFire(true);
    }

    protected override void Remove(PlayerController player)
    {
        player.SetRapidFire(false);
    }
}
