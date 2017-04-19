using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Lifetime))]
public class Lifetime : Orb<Lifetime>
{
	public float CurrentLifetime = 100;
	public float MaxLifeTime = 100;
}