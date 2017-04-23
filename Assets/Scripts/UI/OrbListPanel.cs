using System.Collections.Generic;
using UnityEngine;

public class OrbListPanel : MonoBehaviour
{
	readonly HashSet<Orb> orbButtons = new HashSet<Orb>();
	public GameObject buttonPrefab;
	public GameObject content;
	public Node node;

	public void SetNode(Node n)
	{
		gameObject.SetActive(n != null);
		if (node == n) return;
		foreach (var orbButton in content.transform.Children())
			Destroy(orbButton.gameObject);
		if (n != null)
			n.Orbs.OnOrbsChanged -= OnOrbsChanged;
		node = n;
		if (node == null) return;
		node.Orbs.OnOrbsChanged += OnOrbsChanged;
		UpdateButtonList();
	}


	// Update is called once per frame
	public void Update() {}

	public void OnOrbsChanged(OrbList.EventArgs o) => UpdateButtonList();
	public void UpdateButtonList()
	{
		orbButtons.Clear();
		for (var i = 0; i < content.transform.childCount; i++)
		{
			var c = content.transform.GetChild(i);
			var orb = c.GetComponent<OrbButton>()?.orb;
			orbButtons.Add(orb);
		}
		foreach (var o in node.Orbs)
		{
			if (orbButtons.Contains(o)) continue;
			var b = Instantiate(buttonPrefab, content.transform);
			var orbButton = b.GetComponent<OrbButton>();
			orbButton.Setup(o);
		}
	}
}