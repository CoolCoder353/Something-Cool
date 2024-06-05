using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using Mirror;

public class ResourceMap : MonoBehaviour
{
    public static ResourceMap Instance;
    public GameObject[] resourceObjects; // Array of objects that represent resource locations
    public float maxResourceValue = 100; // Maximum resource value at the location of an object
    public float resourceDropOffRate = 1; // Rate at which resources drop off with distance
    public float randomVariation = 10; // Amount of random variation in resource value
    public float threshold = 1; // Threshold for resource value before we count it as 0 (before we multiply by the maxResourceValue)

    public GameObject previewObject; // Object used in EDITOR ONLY to preview the resource map

    public bool updatePreview = false;

    [ShowIf("updatePreview")]
    public float updateInterval = 1f;



    // Start is called before the first frame update
    [ServerCallback]
    void Start()
    {
        Instance = this;

        // Find all objects with the tag "Resource"
        resourceObjects = GameObject.FindGameObjectsWithTag("Resource");

        if (!updatePreview)
        {
            //Hide the preview
            previewObject.SetActive(false);
        }

    }
    [ServerCallback]
    private void Update()
    {
        if (updatePreview)
        {
            if (Time.time % updateInterval == 0)
            {
                GenerateResourceMap();
            }
        }
    }

    public Vector2 testPosition;

    [Button("Test local position"), Server]
    void TestPosition()
    {
        float resources = GetResourcesAtPosition(testPosition, true);
        Debug.Log($"Resources at position {testPosition}: {resources}");
    }



    [Button("Generate Resource Map"), Server]
    void GenerateResourceMap()
    {
        // Only create the texture when the preview object is active
        if (previewObject != null && previewObject.activeInHierarchy)
        {
            // Find all objects with the tag "Resource"
            resourceObjects = GameObject.FindGameObjectsWithTag("Resource");

            // Create a new texture
            int textureSize = 512; // Size of the texture in pixels
            Texture2D texture = new Texture2D(textureSize, textureSize);

            // Iterate over each pixel in the texture
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    // Assuming worldSize is a Vector2 representing the size of your world in world units
                    Vector2 worldSize = new Vector2(100, 100); // Replace with your actual world size

                    // Calculate the position corresponding to this pixel
                    Vector2 position = new Vector2(x - textureSize / 2f, y - textureSize / 2f) / textureSize * -worldSize;

                    // Calculate the resource value at this position
                    float resourceValue = GetResourcesAtPosition(position);

                    // Calculate the brightness of the pixel based on the resource value
                    float brightness = resourceValue / maxResourceValue;

                    // Set the color of the pixel
                    texture.SetPixel(x, y, new Color(brightness, brightness, brightness));
                }
            }

            // Apply the changes to the texture
            texture.Apply();

            // Assign the texture to the material of the preview object
            previewObject.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
        }
    }
    [Server]
    public float GetResourcesAtPosition(Vector2 position, bool debug = false)
    {
        float resources = 0;

        float maxDistance = (maxResourceValue - threshold) / (threshold * resourceDropOffRate); // Maximum distance to check for resource objects


        List<float> distances = new List<float>();
        foreach (GameObject resourceObject in resourceObjects)
        {
            // Calculate the distance to the resource object
            float distance = Vector2.Distance(position, new(resourceObject.transform.position.x, resourceObject.transform.position.z));

            if (debug)
            {
                Debug.Log($"Distance to resource object: {distance}");
            }


            // Only calculate resource value if the object is within the maximum distance
            if (distance <= maxDistance)
            {
                // Calculate the resource value based on the distance
                float resourceValue = maxResourceValue / (1 + resourceDropOffRate * distance);

                // Add some random variation
                resourceValue += Random.Range(-randomVariation, randomVariation);

                distances.Add(resourceValue);
            }
        }

        resources = maxDistances(distances);

        return resources;
    }


    public static float addDistances(List<float> distances)
    {
        distances.Add(0);
        return distances.Sum();
    }
    public static float averageDistances(List<float> distances)
    {
        return distances.Sum() / distances.Count;
    }
    public static float maxDistances(List<float> distances)
    {
        distances.Add(0);
        return distances.Max();
    }
}