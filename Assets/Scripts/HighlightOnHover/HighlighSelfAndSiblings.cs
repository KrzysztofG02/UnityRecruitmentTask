using System.Collections.Generic;
using UnityEngine;


public class HighlighSelfAndSiblings : HighlightOnHover
{
    protected override List<Renderer> GetTargetRenderers()
    {
        List<Renderer> targetRenderers = new List<Renderer>();

        foreach(Transform sibling in this.transform.parent)
        {
            targetRenderers.AddRange(sibling.GetComponentsInChildren<Renderer>());
        }

        return targetRenderers;
    }
}