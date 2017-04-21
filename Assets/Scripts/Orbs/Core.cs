using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Core))]
public class Core : Orb<Core>, IEquippable
{
	const float MIN_RADIUS = 0.01f;

	[SerializeField] [HideInInspector] bool _Collidable = true;

	[SerializeField] [HideInInspector] LayerMask _LayerMask = 1;

	[SerializeField] [HideInInspector] float _Radius = 0.5f;

	[SerializeField] [HideInInspector] bool _Fixed = false;

	[SerializeField] [HideInInspector] Color _mainColor = Color.white;

	[CustomProperty(nameof(_mainColor))]
	public Color MainColor
	{
		get { return _mainColor; }
		set
		{
			_mainColor = value;

			if (Node != null)
			{
				Material material = Application.isEditor && !Application.isPlaying ? _node.GetComponent<MeshRenderer>().sharedMaterial : Node.MR.material;
				material.color = value;
			}
		}
	}

	
	[CustomProperty(nameof(_Collidable))]
	public bool Collidable
	{
		get { return _Collidable; }
		set
		{
			_Collidable = value;
			if (Node != null)
				Node.GetComponent<Collider2D>().enabled = value;
		}
	}
	
	[CustomProperty(nameof(_Radius))]
	public float Radius
	{
		get { return _Radius; }
		set
		{
			if (value < MIN_RADIUS) value = MIN_RADIUS;
			var ratio = value / _Radius;
			_Radius = value;

			if (!Node) return;
			Node.transform.localScale *= ratio;
			//var c = Node?.GetComponent<CircleCollider2D>();
			//if (c != null) c.radius = value;
		}
	}
	[CustomProperty(nameof(_LayerMask))]
	public LayerMask LayerMask
	{
		get { return _LayerMask; }
		set
		{
			if (value == 0)
			{
				_LayerMask = 1;
				if (Node != null) Node.gameObject.layer = _LayerMask;
			}
			else if ((value & (value - 1)) == 0) //floor(log(value))==log(value)
			{
				_LayerMask = value;
				if (Node != null) Node.gameObject.layer = _LayerMask;
			}
		}
	}
	
	[CustomProperty(nameof(_Fixed))]
	public bool Fixed
	{
		get { return _Fixed; }
		set
		{
			_Fixed = value;
			if (Node != null)
			{
				if (value)
				{
					Node.RB.constraints = RigidbodyConstraints2D.FreezePosition;
					Node.RB.bodyType = RigidbodyType2D.Static;
				}
				else
				{
					Node.RB.constraints = RigidbodyConstraints2D.None;
					Node.RB.bodyType = RigidbodyType2D.Dynamic;
				}
			}
		}
	}

	public override void OnActivate()
	{
		Collidable = _Collidable;
		Radius = _Radius;
		LayerMask = _LayerMask;
		Fixed = _Fixed;
		MainColor = _mainColor;
	}
	
}