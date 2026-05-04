using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private float swipeThreshold = 50f;

    public Action<Vector3> OnSwipe;

    // Update is called once per frame
    void Update()
    {
        HandleSwipe();
    }
    private void HandleSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            endPos = Input.mousePosition;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        float swipeDistance = Vector3.Distance(startPos, endPos);
        if (swipeDistance < swipeThreshold) return;

        Vector2 swipeDir = endPos - startPos;
        swipeDir.Normalize();

        OnSwipe?.Invoke(swipeDir);
    }
}
