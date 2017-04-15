using UnityEngine;
using System.Collections;
using System;
using Vexe.Runtime.Types;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Core))]
public class Core : Orb
{
  [SerializeField, HideInInspector]
  private bool _Collidable = true;
  [Show, CustomProperty(nameof(_Collidable))]
  public bool Collidable { get { return _Collidable; } set
    { _Collidable = value;
      node.GetComponent<Collider2D>().enabled = value;
    }
  }
  [SerializeField, HideInInspector]
  private float _Radius = 0.5f;
  [Show, CustomProperty(nameof(_Radius))]
  public float Radius { get { return _Radius; } set { _Radius = value; var c = node.GetComponent<CircleCollider2D>(); if (c != null) c.radius = value; } }
  [SerializeField, HideInInspector]
  private LayerMask _LayerMask = 1;
  [Show, CustomProperty(nameof(_LayerMask))]
  public LayerMask LayerMask { get { return _LayerMask; }
    set
    {
      if (value == 0)
      {
        _LayerMask = 1;
        node.gameObject.layer = _LayerMask;
      }
      else if ((value & (value-1)) == 0) //floor(log(value))==log(value)
      {
        _LayerMask = value;
        node.gameObject.layer = _LayerMask;
      }
    }
  }
}