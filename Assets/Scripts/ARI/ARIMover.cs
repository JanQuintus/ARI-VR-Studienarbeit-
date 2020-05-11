using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARIMover : MonoBehaviour
{
    public Vector3 BoundsCenter;
    public Vector3 BoundsSize;
    public AudioSource EngineSource;

    #region Private Variables
    private bool _isMoving = false;
    private bool _isRotating = false;
    private Vector3 _initialPosition = Vector3.zero;
    private Vector3 _moveDelta = Vector3.zero;
    private Vector3 _initialRotation = Vector3.zero;
    private Vector3 _rotationDelta = Vector3.zero;
    private float _lerpValue = 0;
    #endregion

    private void Update()
    {
        if (_isMoving)
        {
            transform.position = _initialPosition + _moveDelta * _lerpValue;
            _lerpValue += 2f * Time.deltaTime;
            EngineSource.pitch = (1 + Mathf.Sin(_lerpValue)) * Random.Range(0.75f, 1f);
            if (_lerpValue >= 1f)
            {
                transform.position = _initialPosition + _moveDelta;
                _isMoving = false;
                EngineSource.pitch = 1;
            }
        }
        if (_isRotating)
        {
            transform.rotation = Quaternion.Euler(_initialRotation + _rotationDelta * _lerpValue);
            _lerpValue += 2f * Time.deltaTime;
            EngineSource.pitch = 1 + Mathf.Sin(_lerpValue) * Random.Range(0.75f, 1f);
            if (_lerpValue >= 1f)
            {
                transform.rotation = Quaternion.Euler(_initialRotation + _rotationDelta);
                _isRotating = false;
                EngineSource.pitch = 1;
            }
        }
    }

    public void Move(Vector3 direction)
    {
        if (_isMoving || _isRotating)
            return;

        _moveDelta = transform.rotation * direction * 0.5f;
        if (!InBounds(transform.position + _moveDelta))
        {
            ARI.Instance.Die();
            return;
        }

        _isMoving = true;
        _lerpValue = 0;
        _initialPosition = transform.position;
        
    }

    public void Rotate(int direction)
    {
        if (_isMoving || _isRotating)
            return;
        _isRotating = true;
        _lerpValue = 0;
        _initialRotation = transform.eulerAngles;
        _rotationDelta = new Vector3(0, 90f * direction, 0);
    }

    public void StopAction()
    {
        _isMoving = false;
        _isRotating = false;
    }

    public bool InAction()
    {
        return _isMoving || _isRotating;
    }

    private bool InBounds(Vector3 point)
    {
        return  (point.x < BoundsCenter.x + (BoundsSize.x / 2f)) &&
                (point.x > BoundsCenter.x - (BoundsSize.x / 2f)) &&
                (point.y < BoundsCenter.y + (BoundsSize.y / 2f)) &&
                (point.y > BoundsCenter.y - (BoundsSize.y / 2f)) &&
                (point.z < BoundsCenter.z + (BoundsSize.z / 2f)) &&
                (point.z > BoundsCenter.z - (BoundsSize.z / 2f));

    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(BoundsCenter, BoundsSize);
    }
}
