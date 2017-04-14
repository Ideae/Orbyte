using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbButton : MonoBehaviour {
  public Orb orb;
	// Use this for initialization
	void Start () {
    

	}
	
	// Update is called once per frame
	void Update () {
		
	}
  public void Setup(Orb orb)
  {
    this.orb = orb;
    var text = GetComponentInChildren<Text>();
    text.text = orb.Name;

  }
  public void OnClick()
  {
    print(orb.Name);
  }
}
