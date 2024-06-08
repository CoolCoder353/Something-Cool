using UnityEngine;
using Mirror;

public class Unit : MonoBehaviour
{
    public float speed = 1.0f;
    public float turningSpeed = 1.0f;
    public bool autoAttack = true;


    private Vector2 goalPosition;
    private float tolerance = 0.1f;


    public void SetGoalPosition(Vector2 position)
    {
        goalPosition = position;

    }





}