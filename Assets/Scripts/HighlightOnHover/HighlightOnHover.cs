using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class HighlightOnHover : MonoBehaviour, IHoverable
{
    [Header("Hover Animation Settings")]
    [SerializeField] private Color glowColor = new Color(255, 255, 4, 15);
    [SerializeField] private float fadeDuration = 0.5f;


    private class MaterialInfo
    {
        public Material material;
        public Color originalColor;
    }


    private List<MaterialInfo> targetMaterials;
    private Coroutine currentCoroutine;



    private void Awake()
    {
        this.ExtractMaterialsFromRenderers();
    }


    private void ExtractMaterialsFromRenderers()
    {
        List<Renderer> renderers = this.GetTargetRenderers();

        this.targetMaterials = new List<MaterialInfo>();

        foreach(Renderer renderer in renderers)
        {
            var materials = renderer.materials;

            if(materials.Length > 0)
            {
                Material lastMaterial = materials[materials.Length - 1];

                if(lastMaterial.HasProperty("_Color"))
                {
                    this.targetMaterials.Add(new MaterialInfo
                    {
                        material = lastMaterial,
                        originalColor = lastMaterial.color
                    });
                }
            }            
        }
    }

    protected abstract List<Renderer> GetTargetRenderers();


    public void OnHover()
    {
        if (this != null)
        {
            if (this.currentCoroutine != null)
            {
                this.StopCoroutine(this.currentCoroutine);
            }

            if (this.targetMaterials.Count > 0)
            {
                this.currentCoroutine = this.StartCoroutine(this.FadeToColor(this.glowColor));
            }
        }
    }


    public void OnHoverExit()
    {
        if (this != null)
        {
            if (this.currentCoroutine != null)
            {
                this.StopCoroutine(this.currentCoroutine);
            }

            if (this.targetMaterials.Count > 0)
            {
                this.currentCoroutine = this.StartCoroutine(this.FadeToOriginal());
            }
        }  
    }


    private IEnumerator FadeToColor(Color targetColor)
    {
        float elapsed = 0f;
        Color[] startColors = new Color[this.targetMaterials.Count];
        
        for(int i = 0; i < this.targetMaterials.Count; ++i)
        {
            startColors[i] = this.targetMaterials[i].material.color;
        }

        while(elapsed < this.fadeDuration)
        {
            elapsed += Time.deltaTime;

            for(int i = 0; i < this.targetMaterials.Count; ++i)
            {
                this.targetMaterials[i].material.color = Color.Lerp(
                    startColors[i], targetColor, elapsed / this.fadeDuration);
            }    

            yield return null;
        }

        foreach(var materialInfo in this.targetMaterials)
        {
            materialInfo.material.color = targetColor;
        } 

        this.currentCoroutine = null;
    }


    private IEnumerator FadeToOriginal()
    {
        float elapsed = 0f;
        Color[] startColors = new Color[this.targetMaterials.Count];
        Color[] originalColors = new Color[this.targetMaterials.Count];

        for(int i = 0; i < this.targetMaterials.Count; ++i)
        {
            startColors[i] = this.targetMaterials[i].material.color;
            originalColors[i] = this.targetMaterials[i].originalColor;
        }

        while(elapsed < this.fadeDuration)
        {
            elapsed += Time.deltaTime;

            for(int i = 0; i < this.targetMaterials.Count; ++i)
            {
                this.targetMaterials[i].material.color = Color.Lerp(
                    startColors[i], originalColors[i], elapsed / this.fadeDuration);
            }  

            yield return null;
        }

        for(int i = 0; i < this.targetMaterials.Count; ++i)
        {
            this.targetMaterials[i].material.color = originalColors[i];
        } 

        this.currentCoroutine = null;
    }
}