using UnityEngine;

public interface IOrbType
{
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

public interface IAimedActionOrb : IOrbType
{
	new bool IsActive { get; set; }
	void OnAimedActionDown(Vector2 target);
	void OnAimedActionHeld(Vector2 target);
	void OnAimedActionUp(Vector2 target);
}

public interface IActionOrb : IOrbType
{
	new bool IsActive { get; set; }
	void OnActionDown();
	void OnActionHeld();
	void OnActionUp();
}

public interface IMovementOrb : IOrbType
{
	new bool IsActive { get; set; }
	void ProcessMovement();
	bool ReachedRotationTarget(float target);
	bool ReachedPositionTarget(Vector2 target);
}