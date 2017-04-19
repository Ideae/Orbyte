using UnityEngine;

public interface IOrbType
{
}

public interface IEquippable
{
	void OnEquip();
	void OnUnequip();
}
public interface IDrawOrb : IOrbType
{
	void Draw();
}

public interface IAffectSelfOrb : IOrbType
{
	void AffectSelf();
}

public interface IAffectOtherOrb : IOrbType
{
	void AffectOther(Node other);
}

public interface IFixedAffectOther : IOrbType
{
	void FixedAffectOther(Node other);
}

public interface IFixedAffectSelf : IOrbType
{
	void FixedAffectSelf();
}

public interface IAimedActionOrb : IEquippable
{
	void OnAimedActionDown(Vector2 target);
	void OnAimedActionHeld(Vector2 target);
	void OnAimedActionUp(Vector2 target);
}

public interface IActionOrb : IEquippable
{
	void OnActionDown();
	void OnActionHeld();
	void OnActionUp();
}

public interface IMovementOrb : IEquippable
{
	void ProcessMovement();
	bool ReachedRotationTarget(float target);
	bool ReachedPositionTarget(Vector2 target);
}

public static partial class Utils
{
	public static bool IsEquipped(this IEquippable equippableOrb)
	{
		Orb o = (Orb)equippableOrb;
		if (o.Node == null) return false;
		if (equippableOrb is IAimedActionOrb)
		{
			return o.Node.aimedActionOrb == equippableOrb;
		}
		else if (equippableOrb is IActionOrb)
		{
			return o.Node.actionOrb == equippableOrb;
		}
		else if (equippableOrb is IMovementOrb)
		{
			return o.Node.movementOrb == equippableOrb;
		}
		return false;
	}

	public static void SetEquipped(this IEquippable equippableOrb)
	{
		Orb o = (Orb)equippableOrb;
		if (o.Node == null) return;
		if (equippableOrb is IAimedActionOrb)
		{
			if (equippableOrb != o.Node.aimedActionOrb)
			{
				o.Node.aimedActionOrb?.OnUnequip();
				o.Node.aimedActionOrb = (IAimedActionOrb)equippableOrb;
				o.Node.aimedActionOrb.OnEquip();
			}
		}
		else if (equippableOrb is IActionOrb)
		{
			if (equippableOrb != o.Node.actionOrb)
			{
				o.Node.actionOrb?.OnUnequip();
				o.Node.actionOrb = (IActionOrb)equippableOrb;
				o.Node.actionOrb.OnEquip();
			}
		}
		else if (equippableOrb is IMovementOrb)
		{
			if (equippableOrb != o.Node.movementOrb)
			{
				o.Node.movementOrb?.OnUnequip();
				o.Node.movementOrb = (IMovementOrb)equippableOrb;
				o.Node.movementOrb.OnEquip();
			}
		}
	}
}