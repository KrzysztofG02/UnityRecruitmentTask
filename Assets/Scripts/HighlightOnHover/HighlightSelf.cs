using System.Collections.Generic;
using UnityEngine;


public class HighlightSelf : HighlightOnHover
{
    protected override List<Renderer> GetTargetRenderers()
    {
        return this.GetComponent<Renderer>() != null ? 
            new List<Renderer> {this.GetComponent<Renderer>()} : new List<Renderer>();
    }
}