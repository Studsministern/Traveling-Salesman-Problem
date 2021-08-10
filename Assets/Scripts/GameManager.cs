using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Variables for keeping track of Indice prefab, what text to change and placed indices (encapsulated)
    [SerializeField] private GameObject Indice;
    [SerializeField] private Text descriptionText;
    public List<GameObject> Indices { get; private set; }
    
    // Using camera position to keep track of the necessary length of the ray
    private float rayLength;

    // References to other components
    private LineController lc;
    private SolveProblem sp;
    
    void Start()
    {
        // Components
        lc = GameObject.Find("Line Renderer").GetComponent<LineController>();
        sp = GetComponent<SolveProblem>();

        // Ray length from the camera position
        rayLength = Mathf.Abs(Camera.main.transform.position.z);

        // Initialise and update in the beginning
        Indices = new List<GameObject>();
    }

    // Handle adding more indices and clicking existing ones
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            HandleRayCast(0);
        }

        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            HandleRayCast(1);
        }
    }

    private void HandleRayCast(int mouseButtonPressed)
    {
        // Ray and hit from where the camera is to where the screen is
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayLength);

        // Debug
        // Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.green, 10);

        // CREATE indice if no collider is where the LEFT mouse button is clicked
        if (mouseButtonPressed == 0 && hit.collider == null)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 10;
            pos = Camera.main.ScreenToWorldPoint(pos);
            Instantiate(Indice, pos, Indice.transform.rotation);
            UpdateIndices();
        }
        // REMOVE indice if there is one where the RIGHT mouse button is clicked
        else if (mouseButtonPressed == 1 && hit.collider != null && hit.collider.gameObject.CompareTag("Indice"))
        {
            Destroy(hit.collider.gameObject);
            Indices.Remove(hit.collider.gameObject);
            descriptionText.text = "Solving " + Indices.Count + " point problem";
            sp.StopSolving();
            lc.ClearLine();
        }
    }

    // Update the list by looping through all Indices and only adding the newest Indice
    private void UpdateIndices()
    {
        GameObject[] tempList = GameObject.FindGameObjectsWithTag("Indice");

        for(int i = 0; i < tempList.Length; i++)
        {
            if (!Indices.Contains(tempList[i]))
            {
                Indices.Add(tempList[i]);
            }
        }

        // Update text
        descriptionText.text = "Solving " + Indices.Count + " point problem";

        sp.StopSolving();
        lc.ClearLine();
    }
}
