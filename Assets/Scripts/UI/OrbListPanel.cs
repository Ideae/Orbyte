using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbListPanel : MonoBehaviour {
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

    HashSet<Orb> orbButtons = new HashSet<Orb>();
  public void UpdateButtonList(Node n)
  {

        orbButtons.Clear();
      for (var i = 0; i < content.transform.childCount; i++)
      {
          var c = content.transform.GetChild(i);
          var orb = c.GetComponent<OrbButton>()?.orb;
          orbButtons.Add(orb);

      }
      foreach (var o in n.orbs)
      {
          if (orbButtons.Contains(o)) continue;
          var b = Instantiate(buttonPrefab, content.transform);
          var orbButton = b.GetComponent<OrbButton>();
          orbButton.Setup(o);
      }
  }
}
