using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum OrbState
{
	None = 0,
	Active = 1,
	Locked = 2,
	Reserved = 4,
	EqCore = 8,
	EqMovement = 16,
	EqAction = 32,
	EqAimedAction = 64
}

[Serializable]
public class OrbList : IList<Orb>
{
	public enum Event
	{
		Removed,
		Added,
		Equipped,
		UnEquipped,
		Locked,
		Unlocked,
		Activated,
		Deactivated
	}
	
	public static readonly Type[] equipTypes =
		{typeof(Core), typeof(IMovementOrb), typeof(IActionOrb), typeof(IAimedActionOrb)};

	public static readonly OrbState[] equipStates =
		{OrbState.EqCore, OrbState.EqMovement, OrbState.EqAction, OrbState.EqAimedAction};

	readonly int[] equipIndices = {-1, -1, -1, -1};

	public List<Orb> items; 
	public List<OrbState> states; 
	public event Action<EventArgs> OnOrbsChanged;

	public Orb this[int index]
	{
		get { return items[index]; }
		set { SwapAt(index, value, false); }
	}

	void FireEvent(Event e, OrbState s, Orb item, int index)
	{
		var args = new EventArgs { eventType = e, state = s, orb = item, index = index };
		item.OnStateChanged(args);
		OnOrbsChanged?.Invoke(args);
	}
	void FireEvent(Event e, int index) => FireEvent(e, states[index], items[index], index);

	IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator(); //#unperf

	IEnumerator<Orb> IEnumerable<Orb>.GetEnumerator() => items.GetEnumerator(); //#unperf

	public void Add(Orb item) => Add(item, OrbState.None, false);

	public void Clear()
	{
		for (var i = 0; i < items.Count; i++)
			this[i] = null;
		items.Clear();
		states.Clear();
	}

	public bool Contains(Orb item) => items.Contains(item);

	public void CopyTo(Orb[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);


	public int Count => items.Count;

	public bool IsReadOnly => false;

	public int IndexOf(Orb item) => items.IndexOf(item);

	public void Insert(int index, Orb item) => Insert(index, item, OrbState.None, false);

	public Orb SwapAt(int index, Orb orb,bool overwriteEquipment, OrbState state = OrbState.None)
	{
		var old = items[index];
		RemoveAt(index);
		Insert(index,orb, state, overwriteEquipment);
		return old;
	}

	public bool Remove(Orb item)
	{
		var i = items.IndexOf(item);
		if (i == -1) return false;
		RemoveAt(i);
		return true;
	}

	public void RemoveAt(int index)
	{
		var orb = items[index];
		var state = states[index];
		SetState(OrbState.None, index, false);
		items.RemoveAt(index);
		states.RemoveAt(index);
		FireEvent(Event.Removed, state, orb, index);
	} 

	public void Add(Orb item, OrbState state, bool overwriteEquip)
	{
		states.Add(OrbState.None);
		items.Add(item);
		var index = Count - 1;
		FireEvent(Event.Added, index);
		SetState(state, index, overwriteEquip);
	}

	public void SetActiveAt(int index, bool value)
	{
		var oldstate = states[index];
		if (value)
		{
			states[index] |= OrbState.Active;
			if (states[index] != oldstate) FireEvent(Event.Activated, index);
		}
		else
		{
			states[index] &= ~OrbState.Active;
			if (states[index] != oldstate) FireEvent(Event.Deactivated, index);
		}
	}

	public List<Orb>.Enumerator GetEnumerator() => items.GetEnumerator();

	void SetState(OrbState state, int index, bool overwriteEquip)
	{
		var flags = (OrbState[])Enum.GetValues(FastType<OrbState>.type);
		for (var i = 0; i < flags.Length; i++)
			SetFlag(index, flags[i], state.HasFlag(flags[i]), overwriteEquip);
	}

	void SetFlag(int index, OrbState flag, bool value, bool overwriteEquip = false)
	{
		var eqNum = Array.IndexOf(equipStates, flag);
		if (eqNum != -1)
		{
			SetEquipped(index, eqNum, true, overwriteEquip);
			return;
		}
		switch (flag)
		{
			case OrbState.Active:
				SetActiveAt(index, value);
				break;
			case OrbState.Locked:
				SetLockedAt(index, value);
				break;
		}
	}

	public void SetLockedAt(int index, bool value)
	{
		var oldstate = states[index];
		if (value)
		{
			states[index] |= OrbState.Locked;
			if (states[index] != oldstate) FireEvent(Event.Locked, index);
		}
		else
		{
			states[index] &= ~OrbState.Locked;
			if (states[index] != oldstate) FireEvent(Event.Unlocked, index);
		}
	}

	public void CopyTo(OrbState[] array, int arrayIndex) => states.CopyTo(array, arrayIndex);

	public void Insert(int index, Orb item, OrbState state, bool overwriteEquipment)
	{
		items.Insert(index, item);
		states.Insert(index, state);
		FireEvent(Event.Added, state, item, index);
		SetState(state, index, overwriteEquipment);
	}

	public void UnEquip(int index)
	{
		for (int i = 0; i < equipStates.Length; i++)
		{
			SetEquipped(index,i,false,false);
		}
	}

	// T should be a type associated with an equip slot
	public void SetEquipped<T>(Orb orb, bool equipped) where T : IEquippable
	{
		if (!(orb is T)) throw new ArgumentException("Orb doesn't match type");
		SetEquipped<T>(IndexOf(orb), equipped);
	}

	public void SetEquipped<T>(int index, bool equipped) where T : IEquippable
	{
		var type = FastType<T>.type;
		var eqNum = Array.IndexOf(equipTypes, type);
		if (eqNum == -1) throw new ArgumentException("Type is not an equip slot");
		SetEquipped(index, eqNum, equipped, true);
	}

	void SetEquipped(int index, int eqNum, bool equipped, bool overwrite)
	{
		var state = equipStates[eqNum];
		var oldIndex = equipIndices[eqNum];
		if (oldIndex == -1)
		{
			if (equipped == false) return; // There is no orb equipped in that slot;
		}
		else
		{
			//Remove currently equiped orb
			if ((oldIndex == index) && equipped) return; //Orb at index is already equipped
			if ((oldIndex != index) && !overwrite) return;
			Debug.Assert(states[oldIndex].HasFlag(state));
			states[oldIndex] &= ~state;
			equipIndices[eqNum] = -1;
			FireEvent(Event.UnEquipped, states[oldIndex], items[oldIndex], oldIndex);
			if (oldIndex == index) return; // Previous orb was the one we wanted to unequip.
		}
		states[index] |= state;
		equipIndices[eqNum] = index;
		FireEvent(Event.Equipped, states[index], items[index], index);

	}

	public struct EventArgs
	{
		public Event eventType;
		public OrbState state;
		public Orb orb;
		public int index;
	}
}