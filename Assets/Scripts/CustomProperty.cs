using UnityEngine;
using System.Collections;
using System;

public class CustomProperty : Attribute
{
  public string fieldName;
  public CustomProperty(string fieldName)
  {
    this.fieldName = fieldName;
  }
}
