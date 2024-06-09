using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


public struct UnitGoal
{
    public Vector2 position;
    public float radius;
}

[System.Serializable]
public class UnitGroup
{
    public List<Unit> units = new List<Unit>();

    public List<UnitGoal> lastGoals = new List<UnitGoal>();


    public void SetGoalPosition(Vector2 position, float spacingFactor)
    {
        List<UnitGoal> goals = new List<UnitGoal>();

        int i = 0;
        float totalRadius = 0; // Keep track of the total radius
        foreach (Unit unit in units)
        {
            // Use the Fibonacci spiral and sunflower seed pattern to distribute the units around the goal position.
            float angle = i * Mathf.PI * 2 / ((1 + Mathf.Sqrt(5)) / 2);
            totalRadius += unit.radius; // Add the radius of the unit to the total radius

            UnitGoal goal = new UnitGoal();
            goal.radius = unit.radius;
            goal.position = position + Mathf.Sqrt(totalRadius * spacingFactor) * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)); // set the center of the spiral at the given position

            goals.Add(goal);

            unit.SetGoalPosition(goal.position);
            i++;
        }

        lastGoals = goals;
    }
}
public class UnitManager : MonoBehaviour
{
    public bool debugMode = true;

    [ShowIf("debugMode")]
    public bool showGizmos = true;

    [ShowIf("showGizmos")]
    public bool showTestPoints = true;

    [ShowIf("showGizmos")]
    public bool showPath = true;

    [ShowIf("showPath"), ShowAssetPreview(128, 128)]
    public GameObject UnitPrefab;

    [ShowIf("debugMode")]
    public int numberOfTestPoints = 100;

    [ShowIf("debugMode")]
    public float spacingFactor = 1.0f;

    [ShowIf("showPath")]
    public Vector3 unitStartPosition = new Vector3(0, 0, 0);

    [ShowIf("debugMode")]
    public Vector2Int TestGoalPosition = new Vector2Int(0, 0);




    public UnitGroup testGroup = new UnitGroup();

    [ShowIf("showTestPoints"), Button("Test Point Spreading")]
    public void TestPointSpreading()
    {
        if (testGroup.units.Count > numberOfTestPoints)
        {
            testGroup.units.Clear();
        }
        if (testGroup.units.Count < numberOfTestPoints)
        {
            for (int i = 0; i < numberOfTestPoints; i++)
            {
                Unit unit = new Unit();
                unit.radius = Random.Range(0.5f, 1f);
                testGroup.units.Add(unit);
            }
        }

        testGroup.SetGoalPosition(TestGoalPosition, spacingFactor);

        Debug.Log($"Drawing {testGroup.lastGoals.Count} test points");


    }

    [ShowIf("showPath"), Button("Test Pathfinding")]
    public void TestUnitMovement()
    {
        if (testGroup.units.Count > numberOfTestPoints)
        {
            testGroup.units.Clear();
        }
        if (testGroup.units.Count < numberOfTestPoints)
        {
            for (int i = 0; i < numberOfTestPoints; i++)
            {
                GameObject unitObject = Instantiate(UnitPrefab, unitStartPosition, Quaternion.identity);

                Unit unit = new Unit(unitObject);
                unit.radius = Random.Range(0.5f, 1f);

                testGroup.units.Add(unit);
            }
        }
        testGroup.SetGoalPosition(TestGoalPosition, spacingFactor);

    }


    public void Update()
    {
        if (showPath)
        {
            foreach (Unit unit in testGroup.units)
            {
                unit.MoveWithVelocity();
            }

        }
    }
    public void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            if (showTestPoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(new Vector3(TestGoalPosition.x, TestGoalPosition.y, 0) + new Vector3(0.5f, 0.5f, 0), 1.0f);

                foreach (UnitGoal goal in testGroup.lastGoals)
                {

                    Gizmos.color = Color.red;

                    Gizmos.DrawSphere(new(goal.position.x, goal.position.y, 0), goal.radius);
                }
            }
            if (showPath)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(new Vector3(TestGoalPosition.x, TestGoalPosition.y, 0) + new Vector3(0.5f, 0.5f, 0), 1.0f);


            }
        }
    }


}