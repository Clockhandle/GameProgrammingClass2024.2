using UnityEngine;
using System.Collections;
public class SimpleCameraZoom : MonoBehaviour
{
    private Camera cam;
    private Vector3 originalPosition;
    private float originalZoom;

    public float zoomAmount = 3f;
    public float zoomSpeed = 5f;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        originalPosition = cam.transform.position;
        originalZoom = cam.orthographicSize;
    }

    public void ZoomTo(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomIn(target));
    }

    public void ResetZoom()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomOut());
    }

    private IEnumerator ZoomIn(Transform target)
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, cam.transform.position.z);
        float targetZoom = originalZoom - zoomAmount;

        while (Vector3.Distance(cam.transform.position, targetPos) > 0.05f || Mathf.Abs(cam.orthographicSize - targetZoom) > 0.05f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, Time.unscaledDeltaTime * zoomSpeed);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.unscaledDeltaTime * zoomSpeed);
            yield return null;
        }

        cam.transform.position = targetPos;
        cam.orthographicSize = targetZoom;
    }

    private IEnumerator ZoomOut()
    {
        while (Vector3.Distance(cam.transform.position, originalPosition) > 0.05f || Mathf.Abs(cam.orthographicSize - originalZoom) > 0.05f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, originalPosition, Time.unscaledDeltaTime * zoomSpeed);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalZoom, Time.unscaledDeltaTime * zoomSpeed);
            yield return null;
        }

        cam.transform.position = originalPosition;
        cam.orthographicSize = originalZoom;
    }
}
