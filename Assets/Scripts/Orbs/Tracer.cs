using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Tracer))]
public class Tracer : Orb<Tracer>
{
	TrailRenderer _tr;


	public Material _material;

	public TrailRenderer TR
	{
		get
		{
			if (_tr == null)
			{
				_tr = Node.gameObject.AddComponent<TrailRenderer>();
			}
			
			return _tr;
		}
	}
	[SerializeField] [HideInInspector] Color _color = Color.white;
	[Show, CustomProperty(nameof(_color))]
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
	[Show, CustomProperty(nameof(_time))]
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
	[Show, CustomProperty(nameof(_width))]
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

	public override void OnActivate()
	{
		TR.startColor = _color;
		TR.endColor = new Color(0,0,0,0);
		TR.time = _time;
		TR.material = _material;
		Width = _width;

	}

	public override void OnDeactivate()
	{
		var tr = Node.GetComponent<TrailRenderer>();
		if (tr != null)
		{
			Destroy(tr);
		}
	}
}