using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 currentMousePos;
    private Vector2 direction;

    private bool drag;

    [Range(1f, 10f)]
    [SerializeField] private float dragSpeed = 7f;
    [Range(1f, 20f)]
    [SerializeField] private float stopSpeed = 7f;

    [SerializeField] private float xLimitPosition = 2f;
    [SerializeField] private float yLimitPosition = 2f;

    private void OnValidate()
    {
        if (xLimitPosition < 0f)
        {
            xLimitPosition = 0f;
        }
        if (yLimitPosition < 0f)
        {
            yLimitPosition = 0f;
        }
    }

    private void OnEnable()
    {
        EventsController.Subscribe<EventModels.Game.NodeTappedNull>(this, OnNodeTappedNull);
        EventsController.Subscribe<EventModels.Game.PlayerFingerRemoved>(this, OnPlayerFingerRemoved);
    }
    private void OnDisable()
    {
        EventsController.Unsubscribe<EventModels.Game.NodeTappedNull>(OnNodeTappedNull);
        EventsController.Unsubscribe<EventModels.Game.PlayerFingerRemoved>(OnPlayerFingerRemoved);
    }

    private void OnNodeTappedNull(EventModels.Game.NodeTappedNull tapped)
    {
        drag = true;
        startPos = Input.mousePosition;
    }

    private void OnPlayerFingerRemoved(EventModels.Game.PlayerFingerRemoved playerFinger) => drag = false;

    private void Update()
    {
        if (drag)
        {
            currentMousePos = Input.mousePosition;

            direction = Vector2.Lerp(direction, currentMousePos - startPos, Time.deltaTime * stopSpeed);

            startPos = currentMousePos;
        }
        else
        {
            if (direction.magnitude > 0)
            {
                direction = Vector2.Lerp(direction, Vector2.zero, Time.deltaTime * stopSpeed);
            }
        }

        transform.position += new Vector3(-direction.x, -direction.y, 0) * Time.deltaTime * dragSpeed;

        PositionLimiter();
    }

    private void PositionLimiter()
    {
        var limitX = Mathf.Clamp(transform.position.x, -xLimitPosition, xLimitPosition);
        var limitY = Mathf.Clamp(transform.position.y, -yLimitPosition, yLimitPosition);

        transform.position = new Vector3(limitX, limitY, transform.position.z);
    }
}
