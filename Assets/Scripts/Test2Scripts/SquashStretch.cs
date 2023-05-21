using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashStretch : MonoBehaviour
{
    public SkinnedMeshRenderer characterMeshRenderer;
    public string[] shapeKeyNames = { "Shnoz_Stretch_L", "Shnoz_Stretch_R", "Shnoz_Stretch_U", "Shnoz_Stretch_D" }; // Names of the shape keys you want to control

    public Transform shnozBaseTarget;
    public Transform shnozSpringTarget;

    private float[] shapeKeyValues; // Values for each shape key

    private void Start()
    {
        shapeKeyValues = new float[shapeKeyNames.Length];
    }

    private void Update()
    {
        // Measure the distance between ShnozBaseTarget and ShnozSpringTarget
        Vector3 distance = shnozSpringTarget.position - shnozBaseTarget.position;
        float xDistance = distance.x;
        float yDistance = distance.y;
        float zDistance = distance.z;        

        // Update the shape key values based on the distance
        for (int i = 0; i < shapeKeyNames.Length; i++)
        {
            string shapeKeyName = shapeKeyNames[i];
            float shapeKeyValue = CalculateShapeKeyValue(shapeKeyName, xDistance, yDistance, zDistance);
            shapeKeyValues[i] = shapeKeyValue;
        }

        // Apply the shape key values to the character's mesh
        for (int i = 0; i < shapeKeyNames.Length; i++)
        {
            string shapeKeyName = shapeKeyNames[i];
            int shapeKeyIndex = FindShapeKeyIndex(shapeKeyName); // Find the index of the shape key in the SkinnedMeshRenderer
            if (shapeKeyIndex != -1)
            {
                characterMeshRenderer.SetBlendShapeWeight(shapeKeyIndex, shapeKeyValues[i]);
            }
        }

    }
    private int FindShapeKeyIndex(string shapeKeyName)
    {
        for (int i = 0; i < characterMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            if (characterMeshRenderer.sharedMesh.GetBlendShapeName(i) == shapeKeyName)
            {
                return i;
            }
        }
        return -1; // Shape key not found
    }
    private float CalculateShapeKeyValue(string shapeKeyName, float xDistance, float yDistance, float zDistance)
    {
        float stretchFactor = 0.7f; // Adjust this value to control the influence of the script on the shape keys

        if (shapeKeyName == "Shnoz_Stretch_L" && xDistance < 0)
        {
            return -xDistance * 100f * stretchFactor;
        }
        else if (shapeKeyName == "Shnoz_Stretch_R" && xDistance > 0)
        {
            return xDistance * 100f * stretchFactor;
        }
        else if (shapeKeyName == "Shnoz_Stretch_U" && yDistance < 0)
        {
            return yDistance * 100f * stretchFactor;
        }
        else if (shapeKeyName == "Shnoz_Stretch_D" && yDistance > 0)
        {
            return -yDistance * 100f * stretchFactor;
        }

        // Default shape key value if no condition is met
        return 0f;
    }
}