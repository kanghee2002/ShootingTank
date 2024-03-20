using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float smoothing = 3f;

    private void FixedUpdate()
    {
        if (GameManager.Instance.playerObj)
        {
            Vector3 targetPos = GameManager.Instance.playerObj.transform.position;
            targetPos.z = -10f;
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
        }
    }
}
