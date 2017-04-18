using UnityEngine;
[CreateAssetMenu(menuName = "Orbs/" + nameof(HueShifter))]
public class HueShifter : Orb<HueShifter>, IDrawOrb {
  public float speed = 0.005f;
  public float saturation = 1f;
  public float value = 1f;
  private float hue = 0f;

  public void Draw()
  {
    hue = (hue + speed) % 1f;
    Color c = Color.HSVToRGB(hue, saturation, value);
    Node.mr.material.color = c;
  }
}
