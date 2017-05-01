using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
	[NonSerialized] public Room room;

	public OrbList Orbs;
	public Rigidbody2D RB => GetComponent<Rigidbody2D>();
	public MeshRenderer MR => GetComponent<MeshRenderer>();

	[NonSerialized] public bool HasPositionTarget = false;
	//This could be either a target position or direction depending on HasPositionTarget
	Vector2 _movementGoal; 
	public Vector2 MovementDirection
	{
		get { return HasPositionTarget ? _movementGoal - Position : _movementGoal; }
		set
		{
			HasPositionTarget = false;
			_movementGoal = value;
		}
	}

	public Vector2? MovementPositionTarget
	{
		get { return HasPositionTarget ? (Vector2?)_movementGoal : null; }
		set
		{
			HasPositionTarget = value.HasValue;
			_movementGoal = value ?? Vector2.zero;
		}
	}

	public float rotationGoal { get; set; }

	public Vector2 Position
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	public void OnValidate()
	{

		if (Orbs == null) Orbs = new OrbList(this);
		else Orbs.owner = this;
	}
	public void Awake()
	{
		//Ordering is important here.
		Orbs.InstantiateAll();
		Orbs.OnOrbsChanged += OnOrbListChanged;

		if (Orbs.Core != null) return;
		var newCore = Core.GetDefault();
		Orbs.Add(newCore, OrbState.Active | OrbState.EqCore);
	}
	
	public void Update()
	{
		foreach (var orb in Orbs.AllActiveOrbs)
		{
			(orb as IAffectSelfOrb)?.AffectSelf();
			(orb as IDrawOrb)?.Draw();
			var ao = (orb as IAffectOtherOrb);
			if (ao != null)
			{
				foreach (var other in room.nodes)
				{
					//This if condition might justify turning room.nodes into a hashset :(
					if (other == this) continue;
					ao.AffectOther(other);
				}
			}
		}
	}

	public void FixedUpdate()
	{
		foreach (var orb in Orbs.AllActiveOrbs)
		{
			(orb as IFixedAffectSelf)?.FixedAffectSelf();
			var ao = (orb as IFixedAffectOther);
			if (ao != null)
			{
				foreach (var other in room.nodes)
				{
					//This if condition might justify turning room.nodes into a hashset :(
					if (other == this) continue;
					ao.FixedAffectOther(other);
				}
			}
		}
		Orbs.MovementOrb?.ProcessMovement();
	}
	public void DeleteNode()
	{
		Orbs.Clear();
		Destroy(gameObject);
	}

	public void AimedActionDown(Vector2 worldPos)
	{
		Orbs.AimedActionOrb?.OnAimedActionDown(worldPos);
	}

	public void AimedActionHeld(Vector2 worldPos)
	{
		Orbs.AimedActionOrb.OnAimedActionDown(worldPos);
	}

	public void AimedActionUp(Vector2 worldPos)
	{
		Orbs.AimedActionOrb.OnAimedActionDown(worldPos); 
	}

	void OnOrbListChanged(OrbList.EventArgs args)
	{
		
	}
	
	public void LookTowards(Vector2 v, bool immediately = false)
	{
		v.Normalize();
		rotationGoal = Mathf.Atan2(v.y, v.x);
		if (immediately) RB.rotation = rotationGoal;
	}

	public void LookAt(Vector2 v, bool immediately = false) => LookTowards(v - Position, immediately);
	
	
}