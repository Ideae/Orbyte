using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(ForceMove))]
public class Bomb : Orb<Bomb>, IActionOrb
{

    public void OnActionDown()
    {
        throw new NotImplementedException();
    }

    public void OnActionHeld()
    {
        throw new NotImplementedException();
    }

    public void OnActionUp()
    {
        throw new NotImplementedException();
    }
}
