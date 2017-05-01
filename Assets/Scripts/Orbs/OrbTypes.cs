using UnityEngine;

public interface IOrbType
{
	Node Node { get; }
}

public interface IEquippable: IOrbType { }

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

//public static partial class Utils
//{
//	public static bool IsEquipped(this IEquippable orb, OrbState equipSlot)
//	{
//		Orb o = orb as Orb;
//		if (o?.Node == null) return false;
//		var i = o.Node.Orbs.IndexOf(o);
//		return o.Node.Orbs.IsEquipped(i, equipSlot);
//	}
//
//	public static void SetEquipped(this IEquippable orb, OrbState equipSlot, bool equip)
//	{
//
//		Orb o = orb as Orb;
//		if (o?.Node == null) return;
//		var i = o.Node.Orbs.IndexOf(o);
//		return o.Node.Orbs.SetEquipped<>(i, equipSlot);
//
//	}
//}