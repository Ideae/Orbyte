using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Tracer))]
public class Tracer : Orb<Tracer>
{
	TrailRenderer _tr;


	public Material _material;


	public TrailRenderer TR => Node.gameObject.GetComponent<TrailRenderer>();

	[SerializeField] [HideInInspector] Color _color = Color.white;
	[CustomProperty(nameof(_color))]
	public Color Color
	{
		get { return _color; }
		set
		{
			_color = value;
			TR.startColor = value;
		}
	}

	[SerializeField] [HideInInspector] float _time;
	[CustomProperty(nameof(_time))]
	public float Time
	{
		get { return _time; }
		set
		{
			_time = value;
			TR.time = value;
		}
	}

	[SerializeField] [HideInInspector] float _width = 0.2f;
	[CustomProperty(nameof(_width))]
	public float Width
	{
		get { return _width; }
		set
		{
			_width = value;
			TR.startWidth = value;
			TR.endWidth = value;
		}
	}

	protected override void OnAttach()
	{
		if (TR == null) Node.gameObject.AddComponent<TrailRenderer>();
		
	}

	protected override void OnDetach()
	{
		if(TR!= null) Destroy(TR);
	}

	protected override void OnActivate()
	{
		TR.startColor = _color;
		TR.endColor = new Color(0,0,0,0);
		TR.time = _time;
		TR.material = _material;
		Width = _width;

	}

	protected override void OnDeactivate()
	{
		var tr = Node.GetComponent<TrailRenderer>();
		if (tr != null)
		{
			Destroy(tr);
		}
	}
}