using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbList : MonoBehaviour {
  public GameObject content;
  public GameObject buttonPrefab;
  private Node node;
	// Use this for initialization
  private Dictionary<Orb, OrbButton> orbButtons = new Dictionary<Orb, OrbButton>();

  public void SetNode(Node n) {

	  if (node != n)
	  {
	    foreach (var orbButton in orbButtons.Values)
	    {
	      Destroy(orbButton.gameObject);
	    }
      orbButtons.Clear();
	    if (n != null)
	    {
	      n.OnOrbsChanged -= UpdateButtonList;
	    }
	    node = n;
	    if (node != null)
	    {
	      node.OnOrbsChanged += UpdateButtonList;
        UpdateButtonList(node);
	    }
      
	    gameObject.SetActive(node != null);

	  }
	}

	
	// Update is called once per frame
	public void Update () {

	  
  }

  public void UpdateButtonList(Node n)
  {
    foreach (Orb o in n.orbs.Where(o=>!orbButtons.ContainsKey(o)))
    {
      var b = Instantiate(buttonPrefab, content.transform);
      var orbButton = b.GetComponent<OrbButton>();
      orbButtons[o] = orbButton;
      orbButton.Setup(o);
    }
  }
}
