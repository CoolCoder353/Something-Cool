using UnityEngine;
using Mirror;
using System.Collections.Generic;
using NaughtyAttributes;
using Mono.CompilerServices.SymbolWriter;

[System.Serializable]
public class Unit
{
    public float speed = 1.0f;
    public float turningSpeed = 1.0f;
    public bool autoAttack = true;

    public GameObject gameObject;

    public float RotationOffset = 90;
    private Vector2 goalPosition;
    private List<Vector2> path;

    private float tolerance = 0.01f;
    private float turningTolerance = 0.5f;
    public float radius = 0.5f;
    public bool debug = false;
    public Vector2 DebugGoalPosition;

    public Unit(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    public Unit()
    {

    }

    [Server]
    public void SetGoalPosition(Vector2 position, Vector2 offset = default)
    {
        if (offset == default) offset = new Vector2(0.5f, 0.5f);
        goalPosition = position + offset;
        Vector2 pos = new(gameObject.transform.position.x, gameObject.transform.position.y);
        path = AStarManager.Instance.GetPath(pos, goalPosition);

    }
    [Server]
    public void Move()
    {
        Vector2 currentPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to follow");
            if (debug && !(Vector2.Distance(currentPosition, DebugGoalPosition) < tolerance))
            {
                SetGoalPosition(DebugGoalPosition);
            }
            return;
        }
        if (gameObject == null)
        {
            Debug.LogError("Game object is null");
            return;
        }

        // Check if the game object has reached the next point in the path
        if (Vector2.Distance(currentPosition, path[0]) < tolerance)
        {
            // Remove the next point in the path
            path.RemoveAt(0);

            // Check if the game object has reached the goal position
            if (path.Count == 0)
            {
                // Stop moving the game object
                return;
            }
        }

        // Calculate the direction to the next point in the path
        Vector2 direction = (path[0] - currentPosition).normalized;

        // Calculate the angle of this direction vector relative to the positive X-axis
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Create a new Quaternion representing the desired rotation
        Quaternion toRotation = Quaternion.Euler(0, 0, angle - RotationOffset);

        // Rotate the game object towards the next point in the path at the specified turn speed
        gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, toRotation, turningSpeed * Time.deltaTime);

        // Check if the game object is facing the next point in the path
        if (Quaternion.Angle(gameObject.transform.rotation, toRotation) < turningTolerance)
        {
            // Move the game object towards the next point in the path at the specified move speed
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, path[0], speed * Time.deltaTime);
        }
    }

    [Server]
    public void MoveWithRigidbody()
    {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Game object does not have a Rigidbody2D component");
            return;
        }

        Vector2 currentPosition = rb.position;
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to follow");
            if (debug && !(Vector2.Distance(currentPosition, DebugGoalPosition) < tolerance))
            {
                SetGoalPosition(DebugGoalPosition);
            }
            return;
        }

        // Check if the game object has reached the next point in the path
        if (Vector2.Distance(currentPosition, path[0]) < tolerance)
        {
            // Remove the next point in the path
            path.RemoveAt(0);

            // Check if the game object has reached the goal position
            if (path.Count == 0)
            {
                // Stop moving the game object
                return;
            }
        }

        // Calculate the direction to the next point in the path
        Vector2 direction = (path[0] - currentPosition).normalized;

        // Calculate the angle of this direction vector relative to the positive X-axis
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle by the rotation offset
        float toRotation = angle - RotationOffset;

        // Rotate the game object towards the next point in the path at the specified turn speed
        float newRotation = Mathf.MoveTowardsAngle(rb.rotation, toRotation, turningSpeed * Time.deltaTime);
        rb.MoveRotation(newRotation);

        // Check if the game object is facing the next point in the path
        if (Mathf.Abs(Mathf.DeltaAngle(newRotation, toRotation)) < turningTolerance)
        {
            // Move the game object towards the next point in the path at the specified move speed
            rb.MovePosition(Vector2.MoveTowards(rb.position, path[0], speed * Time.deltaTime));
        }
    }

    [Server]
    public void MoveWithVelocity()
    {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Game object does not have a Rigidbody2D component");
            return;
        }

        Vector2 currentPosition = rb.position;
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to follow");
            if (debug && !(Vector2.Distance(currentPosition, DebugGoalPosition) < tolerance))
            {
                SetGoalPosition(DebugGoalPosition);
            }
            return;
        }

        // Check if the game object has reached the next point in the path
        if (Vector2.Distance(currentPosition, path[0]) < tolerance)
        {
            // Remove the next point in the path
            path.RemoveAt(0);

            // Check if the game object has reached the goal position
            if (path.Count == 0)
            {
                // Stop moving the game object
                rb.velocity = Vector2.zero;
                return;
            }
        }

        // Calculate the direction to the next point in the path
        Vector2 direction = (path[0] - currentPosition).normalized;

        // Calculate the angle of this direction vector relative to the positive X-axis
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle by the rotation offset
        float toRotation = angle - RotationOffset;

        // Rotate the game object towards the next point in the path at the specified turn speed
        float newRotation = Mathf.MoveTowardsAngle(rb.rotation, toRotation, turningSpeed * Time.deltaTime);
        rb.MoveRotation(newRotation);

        // Check if the game object is facing the next point in the path
        if (Mathf.Abs(Mathf.DeltaAngle(newRotation, toRotation)) < turningTolerance)
        {
            // Move the game object towards the next point in the path at the specified move speed
            rb.velocity = direction * speed;
        }
        else
        {
            // Stop moving the game object if it's not facing the next point in the path
            rb.velocity = Vector2.zero;
        }
    }





}