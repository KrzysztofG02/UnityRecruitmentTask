using System.Collections.Generic;
using UnityEngine;


public class HighlightChildren : HighlightOnHover
{
    protected override List<Renderer> GetTargetRenderers()
    {
        List<Renderer> targetRenderers = new();

        foreach(Transform child in this.transform)
        {
            targetRenderers.AddRange(child.GetComponentsInChildren<Renderer>());
        }

        return targetRenderers;
    }
}