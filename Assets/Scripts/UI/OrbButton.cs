using UnityEngine;
using UnityEngine.UI;

public class OrbButton : MonoBehaviour
{
	public Orb orb;

	// Use this for initialization
	void Start() {}

	// Update is called once per frame
	void Update() {}

	public void Setup(Orb _orb)
	{
		orb = _orb;
		var text = GetComponentInChildren<Text>();
		text.text = _orb.Name;
	}

	public void OnClick()
	{
		if (!orb.IsActive) orb.IsActive = true;
		else (orb as IActionOrb)?.OnActionDown();
	}
}