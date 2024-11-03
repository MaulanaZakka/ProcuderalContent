using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSystemGenerator : MonoBehaviour
{
    public string axiom = "F";
    public Dictionary<char, string> rules = new Dictionary<char, string>();
    public int iterations = 1; // Default to 1
    public float angle = 25f;
    public float length = 1f;

    public Slider iterationSlider; // Reference to the UI Slider
    public Text iterationText; // Reference to the UI Text
    public GameObject linePrefab; // Reference to the Line Renderer prefab for branches
    public GameObject leafPrefab; // Reference to the prefab for leaves

    private string currentString;
    private List<GameObject> lineSegments = new List<GameObject>(); // List to hold line segments
    private List<GameObject> leaves = new List<GameObject>(); // List to hold leaves

    void Start()
    {
        // Define the rules for the L-system
        rules.Add('F', "F[+F]F[-F]L"); // Example rule for branches with leaves
        rules.Add('L', "L"); // Leaves do not expand further
        currentString = axiom;

        // Set up the slider
        iterationSlider.minValue = 1; // Minimum value
        iterationSlider.maxValue = 6;  // Maximum value
        iterationSlider.value = 1;      // Default value
        iterationSlider.onValueChanged.AddListener(UpdateIterations);
        UpdateIterations(iterationSlider.value); // Initialize with slider value
    }

    void UpdateIterations(float value)
    {
        iterations = (int)value;
        iterationText.text = "Iterations: " + iterations;
        GenerateLSystem();
    }

    void GenerateLSystem()
    {
        // Clear previous line segments and leaves
        foreach (GameObject segment in lineSegments)
        {
            Destroy(segment);
        }
        lineSegments.Clear(); // Clear the list

        foreach (GameObject leaf in leaves)
        {
            Destroy(leaf);
        }
        leaves.Clear(); // Clear the list

        currentString = axiom;

        // Generate the L-system string
        for (int i = 0; i < iterations; i++)
        {
            currentString = ApplyRules(currentString);
        }

        // Draw the L-system
        DrawLSystem();
    }

    string ApplyRules(string input)
    {
        string output = "";
        foreach (char c in input)
        {
            output += rules.ContainsKey(c) ? rules[c] : c.ToString();
        }
        return output;
    }

    void DrawLSystem()
    {
        Stack<Vector3> stack = new Stack<Vector3>();
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    // Create a new line segment for branches
                    GameObject lineSegment = Instantiate(linePrefab);
                    LineRenderer lineRenderer = lineSegment.GetComponent<LineRenderer>();

                    // Set the positions for the line segment
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, currentPosition);
                    currentPosition += currentRotation * Vector3.up * length; // Move forward in the current direction
                    lineRenderer.SetPosition(1, currentPosition);

                    // Set the position of the line segment in the scene
                    lineSegment.transform.position = currentPosition;

                    // Add the line segment to the list
                    lineSegments.Add(lineSegment);
                    break;

                case 'L':
                    // Create a new leaf at the current position
                    GameObject leaf = Instantiate(leafPrefab);
                    leaf.transform.position = currentPosition; // Position the leaf at the end of the branch
                    leaf.transform.rotation = currentRotation; // Optional: Set the rotation to match the branch
                    leaves.Add(leaf); // Add the leaf to the list
                    break;

                case '+':
                    currentRotation *= Quaternion.Euler(0, 0, -angle);
                    break;

                case '-':
                    currentRotation *= Quaternion.Euler(0, 0, angle);
                    break;

                case '[':
                    stack.Push(currentPosition);
                    stack.Push(currentRotation .eulerAngles);
                    break;

                case ']':
                    currentRotation.eulerAngles = stack.Pop();
                    currentPosition = stack.Pop();
                    break;
            }
        }
    }
}