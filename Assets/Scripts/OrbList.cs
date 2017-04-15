using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbList : MonoBehaviour {
  public GameObject content;
  public GameObject buttonPrefab;
  public Node node;

  public void SetNode(Node n) {

      gameObject.SetActive(n != null);
        if (node != n)
	  {
	    foreach (var orbButton in content.transform.Children())
	    {
	      Destroy(orbButton.gameObject);
	    }
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
      

	  }
	}



    // Update is called once per frame
    public void Update () {

	  
    }

  public void UpdateButtonList(Node n)
  {
      var orbButtons = content.transform.Children()
            .Select(c => c.GetComponent<OrbButton>()?.orb).ToSet();
    foreach (Orb o in n.orbs.Where(o=>!orbButtons.Contains(o)))
    {
      var b = Instantiate(buttonPrefab, content.transform);
      var orbButton = b.GetComponent<OrbButton>();
      orbButton.Setup(o);
    }
  }
}
