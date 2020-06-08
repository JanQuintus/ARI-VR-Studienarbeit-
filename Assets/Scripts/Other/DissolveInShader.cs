using UnityEngine;

public class DissolveInShader : MonoBehaviour
{
    private Material _mat;
    private float _curr = -1f;

    private void Awake()
    {
        _mat = GetComponent<Renderer>().material;
        _mat.SetFloat("_Dissolve", -1);
    }

    private void Update()
    {
        _curr = Mathf.Lerp(_curr, 0, Time.deltaTime);
        _mat.SetFloat("_Dissolve", _curr);

        if (_curr >= 0)
            Destroy(this);
    }
}
