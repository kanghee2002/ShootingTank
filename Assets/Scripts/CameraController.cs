using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothing = 3f;

    private void Start()
    {
        transform.position = GameManager.Instance.playerObject.transform.position;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.playerObject)
        {
            Vector3 targetPos = GameManager.Instance.playerObject.transform.position;
            targetPos.z = -10f;
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
        }
    }
}
