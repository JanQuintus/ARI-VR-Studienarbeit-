using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MoveToPlayerSize : MonoBehaviour
{
    public float MoveTime = 1f;

    private bool _wasTakenOff = false;
    private float _initialY;

    private void Awake()
    {
        _initialY = transform.position.y;
    }

    void Start()
    {
        StartCoroutine(AdjustHeightCor());
        StartCoroutine(CheckHeadsetOnHead());

        OVRManager.display.RecenteredPose += AdjustHeight; 
    }

    private void OnDestroy()
    {
        OVRManager.display.RecenteredPose -= AdjustHeight;
    }

    private void AdjustHeight()
    {
        StartCoroutine(AdjustHeightCor());
    }

    IEnumerator AdjustHeightCor()
    {
        yield return new WaitForSeconds(0.2f);
        float targetY = _initialY + FindObjectOfType<OVRCameraRig>().centerEyeAnchor.position.y - 1.2f;

        if (targetY < _initialY)
            targetY = _initialY;

        if (FindObjectOfType<OVRCameraRig>().centerEyeAnchor.position.y > 0)
            transform.DOMoveY(targetY, MoveTime);
    }

    IEnumerator CheckHeadsetOnHead()
    {
        if (OVRManager.instance.isUserPresent && _wasTakenOff)
        {
            StartCoroutine(AdjustHeightCor());
            _wasTakenOff = false;
        }

        if (!OVRManager.instance.isUserPresent)
            _wasTakenOff = true;

        yield return null;
        StartCoroutine(CheckHeadsetOnHead());
    }


}
