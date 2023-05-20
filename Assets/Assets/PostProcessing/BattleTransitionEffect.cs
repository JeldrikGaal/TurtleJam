using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class BattleTransitionEffect : MonoBehaviour
{

    [SerializeField] private Shader _shader;

    public Texture2D Transition;

    [Range(0, 1)] public float Cutoff;

    private Material _material;


    private void OnEnable()
    {
        _material = new Material(_shader);

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _material.SetFloat("_Cutoff", Cutoff);
        _material.SetTexture("_Transition", Transition); // not good for performance 
        Graphics.Blit(source, destination, _material);


    }
}