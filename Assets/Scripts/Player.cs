using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Player))]
public class Player : Orb {
  public override void OnAttach()
  {
    UIManager.Instance.RegisterPlayer(this);
  }
}
