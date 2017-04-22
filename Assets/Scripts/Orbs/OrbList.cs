using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class OrbList : IList<Orb>
{
	public List<Orb> items;
	public List<OrbState> states;
	public event Action<EventArgs> OnOrbsChanged;
	public List<Orb>.Enumerator GetEnumerator() => items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator(); //#unperf

	IEnumerator<Orb> IEnumerable<Orb>.GetEnumerator() => items.GetEnumerator(); //#unperf

	public void Add(Orb item) => Add(item, OrbState.None);

	public void Add(Orb item, OrbState state)
	{
		states.Add(state);
		items.Add(item);
		OnOrbsChanged?.Invoke(new EventArgs(Event.Added, state, item, Count-1));
	}

	public void Clear()
	{
		for (var i = 0; i < items.Count; i++)
		{
			this[i] = null;
		}
		items.Clear();
		states.Clear();

	}

	public bool Contains(Orb item) => items.Contains(item);

	public void CopyTo(Orb[] array, int arrayIndex)=> items.CopyTo(array, arrayIndex);
	public void CopyTo(OrbState[] array, int arrayIndex)=> states.CopyTo(array, arrayIndex);

	public bool Remove(Orb item)
	{		
		var i = items.IndexOf(item);
		if (i == -1) return false;
		states.RemoveAt(i);
		items.RemoveAt(i);
		return true;
	}

	public int Count => items.Count;

	public bool IsReadOnly => false;

	public int IndexOf(Orb item) => items.IndexOf(item);

	public void Insert(int index, Orb item) => Insert(index, item, OrbState.None);
	public void Insert(int index, Orb item, OrbState state)
	{
		items.Insert(index, item);
		states.Insert(index, state);
		OnOrbsChanged?.Invoke(new EventArgs(Event.Added, state, item, index));
	}


	public void RemoveAt(int index)
	{
		var orb = items[index];
		var state = states[index];
		
		items.RemoveAt(index);
		states.RemoveAt(index);

		OnOrbsChanged?.Invoke(new EventArgs(Event.Removed,state, orb, index));
		
	}

	public Orb this[int index]
	{
		get { return items[index]; }
		set
		{
			var old = items[index];
			var oldState = states[index];
			if(old!= null) OnOrbsChanged?.Invoke(new EventArgs(Event.Removed, oldState,old,index)); 
			items[index] = value;
			OnOrbsChanged?.Invoke(new EventArgs(Event.Added, oldState, old, index));

		}
	}

	public enum Event
	{
		Removed,
		Added,
		Equiped,
		Locked
	}

	public struct EventArgs
	{
		public Event eventType;
		public OrbState state;
		public Orb orb;
		public int index;

		public EventArgs(Event eventType, OrbState state, Orb orb, int index)
		{
			this.eventType = eventType;
			this.state = state;
			this.orb = orb;
			this.index = index;
		}
	}
}


