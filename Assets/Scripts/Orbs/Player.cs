using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Player))]
public class Player : Orb<Player>, IAimedActionOrb, IAffectSelfOrb {

  public override void OnAttach()
    {
    UIManager.Instance.RegisterPlayer(this);
    }

    bool _isInspecting;
    bool IAimedActionOrb.IsActive
    {
        get { return base.IsActive && _isInspecting; }
        set { _isInspecting = value; }
    }

    public void AffectSelf()
    { 
        if (Input.GetMouseButton(1))
        {
            var v = UIManager.Instance.ScreenToWorldPoint();
            Node.movementPositionTarget = v ?? Vector2.zero;

        }
        else if(!Node.movementPositionTarget.HasValue)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Node.movementDirectionTarget = new Vector2(x,y);
            
        }
        if(Node.movementDirectionTarget.magnitude > .3f) Node.LookTowards(Node.movementDirectionTarget);

    }

    public void OnAimedActionDown(Vector2 target)
    {
    var coll = Physics2D.OverlapPoint(target);
    UIManager.Instance.SelectNode(coll?.gameObject.GetComponent<Node>());
    }

    public void OnAimedActionHeld(Vector2 target){}

    public void OnAimedActionUp(Vector2 target){}
}
