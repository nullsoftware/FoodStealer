using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 2f;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void ZoomToTarget(Transform target, float factor = .2f)
    {
        Vector3 targetPos = Vector3.Lerp(transform.position, target.position, factor);
        StartCoroutine(Zoom(targetPos));
    }

    private IEnumerator Zoom(Vector3 target)
    {
        while (true)
        {
            var step = _zoomSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.LookRotation(target - transform.position, Vector3.up),
                _rotationSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                break;
            }

            yield return null;
        }
    }
}
