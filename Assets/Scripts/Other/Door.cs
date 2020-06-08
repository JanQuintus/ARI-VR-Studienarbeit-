using UnityEngine.SceneManagement;
using UnityEngine;

public class Door : MonoBehaviour
{

    private void Start()
    {
        GetComponent<OVRGrabbable>().OnGrabBegin += () => {
            SceneManager.LoadScene("MainMenu");
        };
    }
}
