using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(HueShifter))]
public class HueShifter : Orb<HueShifter>, IDrawOrb
{
	float hue;
	public float saturation = 1f;
	public float speed = 0.005f;
	public float value = 1f;

	public void Draw()
	{
		hue = (hue + speed) % 1f;
		var c = Color.HSVToRGB(hue, saturation, value);
		Node.MR.material.color = c;
	}
}