using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour
{
    public bool Enabled = true;
    public AudioSource Source;
    public AudioClip ClickAC;
    public UnityEvent OnButtonPressed;

#if UNITY_EDITOR
    [Header("Debug")]
    public bool Press = false;

    private void Update()
    {
        if (Press)
        {
            Press = false;
            if (!Enabled) return;
            OnButtonPressed?.Invoke();
            Source.PlayOneShot(ClickAC);
        }
    }

#endif

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finger")
        {
            if (!Enabled) return;
            OnButtonPressed?.Invoke();
            Source.PlayOneShot(ClickAC);
        }
    }
}
