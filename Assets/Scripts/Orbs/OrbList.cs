using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[Flags]
public enum EquipSlot
{
	Core = 16,
	Movement = 32,
	Action = 64,
	AimedAction = 128,
}

[Flags]
public enum OrbState
{
	None = 0,
	Active = 1,
	Locked = 2,
	Reserved = 4,
	Reserved2 = 8,
	EqCore = EquipSlot.Core,
	EqMovement = EquipSlot.Movement,
	EqAction = EquipSlot.Action,
	EqAimedAction = EquipSlot.AimedAction
}

[Serializable]
public class OrbList : IList<Orb>
{

	public enum Event
	{
		Removed,//Default event
		Added,
		Equipped,
		UnEquipped,
		Locked,
		Unlocked,
		Activated,
		Deactivated
	}
	
	public static readonly Type[] EquipTypes =
		{typeof(Core), typeof(IMovementOrb), typeof(IActionOrb), typeof(IAimedActionOrb)};

	public static readonly EquipSlot[] EquipSlots = (EquipSlot[])Enum.GetValues(FastType<EquipSlot>.type);

	readonly int[] _equipIndices = {-1, -1, -1, -1};

	public OrbList()
	{
		this.items = new List<Orb>();
		this.states = new List<OrbState>();
	}
	public OrbList(Node owner) : this()
	{
		this.owner = owner;
	}

	int integrityCheck;
	public readonly List<Orb> items;
	public readonly List<OrbState> states;
	public  Node owner;

	public event Action<EventArgs> OnOrbsChanged;

	public Core				Core			=> _equipIndices[0] == -1 ? null : items[_equipIndices[0]] as Core;
	public IMovementOrb		MovementOrb		=> _equipIndices[1] == -1 ? null : items[_equipIndices[1]] as IMovementOrb;
	public IActionOrb		ActionOrb		=> _equipIndices[2] == -1 ? null : items[_equipIndices[2]] as IActionOrb;
	public IAimedActionOrb	AimedActionOrb	=> _equipIndices[3] == -1 ? null : items[_equipIndices[3]] as IAimedActionOrb;
	
	public Orb this[int index]
	{
		get
		{
			return items[index];
		}
		set { SwapAt(index, value, false); }
	}

	void FireEvent(Event e, OrbState s, Orb item, int index)
	{
		integrityCheck = Random.Range(int.MinValue, int.MaxValue);
		var args = new EventArgs { eventType = e, state = s, orb = item, index = index };
		item?.OnStateChanged(args);
		OnOrbsChanged?.Invoke(args);
	}

	void FireEvent(Event e, int index) => FireEvent(e, states[index], items[index], index);

	IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator(); //#unperf

	IEnumerator<Orb> IEnumerable<Orb>.GetEnumerator() => items.GetEnumerator(); //#unperf

	public void Add(Orb item) => Add(item, OrbState.None);

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

		for (var j = 0; j < _equipIndices.Length; j++)
		{
			if (_equipIndices[j] >= index) _equipIndices[j]--;
		}
	} 

	public void Add(Orb item, OrbState state, bool overwriteEquip = false)
	{
		states.Add(OrbState.None);
		items.Add(item);
		var index = Count - 1;
		if(item != null) item._node = owner;
		FireEvent(Event.Added, index);
		SetState(state, index, overwriteEquip);
	}

	public void SetActive(int index, bool value)
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
		var eqNum = Array.IndexOf(EquipSlots, flag);
		if (eqNum != -1)
		{
			SetEquipped(index, eqNum, true, overwriteEquip);
			return;
		}
		switch (flag)
		{
			case OrbState.Active:
				SetActive(index, value);
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
		if(item!=null) item._node = owner;
		FireEvent(Event.Added, state, item, index);
		SetState(state, index, overwriteEquipment);

		for (var j = 0; j < _equipIndices.Length; j++)
		{
			if (_equipIndices[j] > index) _equipIndices[j]++;
		}
	}

	public void UnEquip(int index)
	{
		for (int i = 0; i < EquipSlots.Length; i++)
		{
			SetEquipped(index,i,false,false);
		}
	}

	public bool IsEquipped(int index) => Array.IndexOf(_equipIndices, index) != -1;

	public bool IsEquipped(EquipSlot slot, int index) => (states[index] & (OrbState)slot) == (OrbState)slot;

	public void SetEquipped(EquipSlot slot, int index, bool equipped)
	{

		var eqNum = Array.IndexOf(EquipSlots, slot);
		var type = EquipTypes[eqNum];
		if (!type.IsInstanceOfType(items[index])) throw new ArgumentException("Orb doesn't match type");
		SetEquipped(index, eqNum, equipped, true);
	}
	
	void SetEquipped(int index, int eqNum, bool equipped, bool overwrite)
	{
		var state = EquipSlots[eqNum];
		var oldIndex = _equipIndices[eqNum];
		if (oldIndex == -1)
		{
			if (equipped == false) return; // There is no orb equipped in that slot;
		}
		else
		{
			//Remove currently equiped orb
			if ((oldIndex == index) && equipped) return; //Orb at index is already equipped
			if ((oldIndex != index) && !overwrite) return;
			Debug.Assert(states[oldIndex].HasFlag((OrbState)state));
			states[oldIndex] &= (OrbState)~state;
			_equipIndices[eqNum] = -1;
			FireEvent(Event.UnEquipped, states[oldIndex], items[oldIndex], oldIndex);
			if (oldIndex == index) return; // Previous orb was the one we wanted to unequip.
		}
		states[index] |= (OrbState)state;
		_equipIndices[eqNum] = index;
		FireEvent(Event.Equipped, states[index], items[index], index);

	}
	public void InstantiateAll(Node node)
	{
		for (var index = 0; index < items.Count; index++)
		{
			items[index] = items[index].Clone();
			items[index]._node = node;
			SetState(states[index], index, true);
		}
	}

	public void AddAll(OrbList orbs, bool cloneOrbs, bool overwriteEquip = false)
	{
		for (var i = 0; i < orbs.Count; i++)
		{
			Add(cloneOrbs ? orbs[i].Clone() : orbs[i], orbs.states[i], overwriteEquip);
		}
	}

	public bool IsLocked(int index) => 
		states[index].HasFlag(OrbState.Locked);
	public bool IsActive(int index) => 
		states[index].HasFlag(OrbState.Active);


	public struct EventArgs
	{
		public Event eventType;
		public OrbState state;
		public Orb orb;
		public int index;
	}

	public T Get<T>(bool active = false) where T : class 
	{
		foreach (var orb in GetAll<T>(active))
		{
			return orb;
		}
		return null;
	}

	public TypeEnumerator<T> GetAll<T>(bool activeOnly = false) where T : class => new TypeEnumerator<T>(this, activeOnly);

	public TypeEnumerator<Orb> AllActiveOrbs => GetAll<Orb>(true);


	public struct TypeEnumerator<T> : IEnumerable<T>, IEnumerator<T> where T : class
	{
		int listCheck;
		private int index;
		private readonly OrbList list;
		private readonly bool activeOnly;
		public IEnumerator<T> GetEnumerator() => this; 
		IEnumerator IEnumerable.GetEnumerator() => this;

		public TypeEnumerator(OrbList list, bool activeOnly)
		{
			this.index = 0;
			this.list = list;
			listCheck = list.integrityCheck;
			this.activeOnly = activeOnly;
		}

		public void Dispose(){}

		public bool MoveNext()
		{
			if (listCheck != list.integrityCheck) throw new Exception("Collection Was Modified During iteration");
			index++;
			for (; index < list.Count; index++)
			{
				if (list[index] is T && (!activeOnly || list[index].IsActive)) return true;
			}
			return false;

		}

		public void Reset()
		{
			index = 0;
			listCheck = list.integrityCheck;
		}

		public T Current
		{
			get
			{
				if (listCheck != list.integrityCheck) throw new Exception("Collection Was Modified During iteration");
				return list[index] as T;
			}
		}

		object IEnumerator.Current => Current;
	}
}