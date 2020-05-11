using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperSpace : MonoBehaviour
{
    public bool IsFree { get; private set; }

    private void Start()
    {
        IsFree = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "ProgramBlock")
        {
            IsFree = false;
        }   
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ProgramBlock")
        {
            IsFree = true;
        }
    }
}
