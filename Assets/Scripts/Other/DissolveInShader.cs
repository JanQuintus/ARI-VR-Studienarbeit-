using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveInShader : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Renderer>().material.SetFloat("_InitialTime", Time.time);
    }
}
