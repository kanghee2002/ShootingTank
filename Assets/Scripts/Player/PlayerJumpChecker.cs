﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpChecker : MonoBehaviour
{
    public bool isGrounding = false;
    public List<string> jumpableTags;

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach(var tag in jumpableTags)
        {
            if (other.CompareTag(tag))
            {
                isGrounding = true;
            }
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
    }
}
