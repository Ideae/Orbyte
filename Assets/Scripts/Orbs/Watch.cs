using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Watch))]
public class Watch : Orb<Watch>, IAffectSelfOrb
{
	public void AffectSelf()
	{
		Camera.main.transform.position = new Vector3(Node.transform.position.x, Node.transform.position.y, Camera.main.transform.position.z);
	}
}