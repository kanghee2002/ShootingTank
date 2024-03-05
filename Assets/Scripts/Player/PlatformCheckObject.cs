using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCheckObject : MonoBehaviour
{
    public bool isTouchingPlatform = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Platform")
        {
            isTouchingPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Platform")
        {
            isTouchingPlatform = false;
        }
    }
}
