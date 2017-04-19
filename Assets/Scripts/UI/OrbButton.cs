using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrbButton : MonoBehaviour, IPointerClickHandler
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
		orb.OnClick();
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		//Debug.Log("what6");
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			UIManager.Instance.inspectorPanel.SetActive(true);
			
			var inspector = FindObjectOfType<OrbInspector>();
			inspector.SetOrb(orb);
		}
	}
}