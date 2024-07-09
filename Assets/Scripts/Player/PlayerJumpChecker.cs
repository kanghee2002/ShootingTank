using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpChecker : MonoBehaviour
{
    public bool isGrounding = false;
    public bool isGroundingOneWayPlatform = false;

    public List<string> jumpableTags;

    private string oneWayPlatformTag = "OneWayPlatform";

    public Collider2D oneWayPlatformCollider = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach(var tag in jumpableTags)
        {
            if (other.CompareTag(tag))
            {
                isGrounding = true;
            }
        }

        if (other.CompareTag(oneWayPlatformTag))
        {
            isGroundingOneWayPlatform = true;

            oneWayPlatformCollider = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        foreach (var tag in jumpableTags)
        {
            if (other.CompareTag(tag))
            {
                isGrounding = false;
            }
        }

        if (other.CompareTag(oneWayPlatformTag))
        {
            isGroundingOneWayPlatform = false;

            oneWayPlatformCollider = null;
        }
    }
}
