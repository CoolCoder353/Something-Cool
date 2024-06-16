using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;


[System.Serializable]
public class Unit : Health
{
    public float speed = 1.0f;
    public float turningSpeed = 1.0f;
    public bool autoAttack = true;
    public float attackRange = 100.0f;
    public float attackSpeed = 1.0f;
    public float attackDamage = 1.0f;
    public float accuracy = 1f;

    public float RotationOffset = 90;
    private Vector2 goalPosition;
    private List<Vector2> path;

    private float tolerance = 0.01f;
    private float turningTolerance = 0.5f;
    public float radius = 0.5f;
    public bool debug = false;
    public Vector2 DebugGoalPosition;


    private Health target;
    private float lastAttackTime;


    [Server]
    public void SetGoalPosition(Vector2 position, Vector2 offset = default)
    {
        if (offset == default) offset = new Vector2(0.5f, 0.5f);
        goalPosition = position + offset;
        Vector2 pos = new(gameObject.transform.position.x, gameObject.transform.position.y);
        path = AStarManager.Instance.GetPath(pos, goalPosition);

    }

    [ServerCallback]
    public void Update()
    {

        if (autoAttack)
        {
            AutoAttack();
        }

        MoveWithVelocity();
    }

    [Server]
    public void AutoAttack()
    {
        if (target == null)
        {
            // Get all colliders within the attack radius
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

            // Filter out the unit itself
            colliders = colliders.Where(collider => collider.gameObject != gameObject).ToArray();
            if (colliders.Length > 0)
                //TODO: Get the closest, not the first
                if (colliders[0].gameObject.TryGetComponent(out Health health))
                {

                    Shoot(health);
                }
        }
        else
        {
            Shoot(target);

        }
    }

    [Server]
    private void Shoot(Health health)
    {
        //TODO: Visualize shooting
        Debug.Log($"Attacking {health.gameObject.name}");
        if (health == null)
        {
            target = null; return;
        }
        if (health.IsDead())
        {
            target = null; return;
        }
        if (Time.time - lastAttackTime < attackSpeed)
        {
            return;
        }
        //Raycast to check if we hit the target, based on hit chance, Where 1 is 100% and 0 is 0%
        if (UnityEngine.Random.value <= accuracy)
        {
            //Check if there is a wall between us
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, health.gameObject.transform.position - gameObject.transform.position, attackRange);
            Debug.Log($"Hit {hit.collider.gameObject.name}");
            if (hit.collider != null)
            {

                health.TakeDamage(attackDamage);
                lastAttackTime = Time.time;
            }
            else
            {
                target = null;
            }
        }

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