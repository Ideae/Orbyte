using UnityEngine;
using Vexe.Runtime.Types;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Core))]
public class Core : Orb<Core>
{
	[SerializeField] [HideInInspector] bool _Collidable = true;

	[SerializeField] [HideInInspector] LayerMask _LayerMask = 1;

	[SerializeField] [HideInInspector] float _Radius = 0.5f;

	[Show]
	[CustomProperty(nameof(_Collidable))]
	public bool Collidable
	{
		get { return _Collidable; }
		set
		{
			_Collidable = value;
			Node.GetComponent<Collider2D>().enabled = value;
		}
	}

	[Show]
	[CustomProperty(nameof(_Radius))]
	public float Radius
	{
		get { return _Radius; }
		set
		{
			_Radius = value;
			var c = Node.GetComponent<CircleCollider2D>();
			if (c != null) c.radius = value;
		}
	}

	[Show]
	[CustomProperty(nameof(_LayerMask))]
	public LayerMask LayerMask
	{
		get { return _LayerMask; }
		set
		{
			if (value == 0)
			{
				_LayerMask = 1;
				Node.gameObject.layer = _LayerMask;
			}
			else if ((value & (value - 1)) == 0) //floor(log(value))==log(value)
			{
				_LayerMask = value;
				Node.gameObject.layer = _LayerMask;
			}
		}
	}
}