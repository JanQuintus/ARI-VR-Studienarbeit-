using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Animator>().SetFloat("CycleOffset", Random.Range(0f, 1f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameState.Instance.IsPaused())
            return;
        if (other.gameObject.tag == "ARI")
        {
            ARI.Instance.Collect();
            Destroy(gameObject);
        }
    }
}
