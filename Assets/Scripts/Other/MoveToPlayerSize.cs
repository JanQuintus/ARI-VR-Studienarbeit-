using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MoveToPlayerSize : MonoBehaviour
{
    public float MoveTime = 1f;

    private bool _wasTakenOff = false;
    private float _initialY;
    private static float _playerHeight = 0f;

#if UNITY_EDITOR
    [Header("Debug")]
    public float SetToHeighOnStart = 0;
    private static float _startHeightDebug = 0;
#endif

    private void Awake()
    {
        _initialY = transform.position.y;

#if UNITY_EDITOR
        if(SetToHeighOnStart > 0)
            _startHeightDebug = SetToHeighOnStart;
#endif
    }

    void Start()
    {
        if (_playerHeight == 0)
            StartCoroutine(UpdatePlayerHeight());
        else
            AdjustHeight();
        
        StartCoroutine(CheckHeadsetOnHead());

        OVRManager.display.RecenteredPose += UpdateHeight; 
    }

    private void OnDestroy()
    {
        OVRManager.display.RecenteredPose -= UpdateHeight;
    }

    private void UpdateHeight()
    {
        StartCoroutine(UpdatePlayerHeight());
    }

    IEnumerator UpdatePlayerHeight()
    {
        yield return new WaitForSeconds(0.2f);
        _playerHeight = FindObjectOfType<OVRCameraRig>().centerEyeAnchor.position.y;
        AdjustHeight();
    }

    private void AdjustHeight()
    {
        float targetY = _initialY + _playerHeight - 1.2f;

#if UNITY_EDITOR
        targetY += _startHeightDebug;
#endif

        if (targetY < _initialY)
            targetY = _initialY;

        if (_playerHeight > 0)
            MoveObject(targetY);
    }

    private void MoveObject(float targetY)
    {
        if (MoveTime == 0)
        {
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }
        else
        {
            transform.DOMoveY(targetY, MoveTime);
        }
    }

    IEnumerator CheckHeadsetOnHead()
    {
        if (OVRManager.instance.isUserPresent && _wasTakenOff)
        {
            StartCoroutine(UpdatePlayerHeight());
            _wasTakenOff = false;
        }

        if (!OVRManager.instance.isUserPresent)
            _wasTakenOff = true;

        yield return null;
        StartCoroutine(CheckHeadsetOnHead());
    }


}
