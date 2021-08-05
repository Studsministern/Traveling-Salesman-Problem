using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolveProblem : MonoBehaviour
{
    [SerializeField] private GameObject Indice;
    [SerializeField] private List<GameObject> Indices;

    private int rayLength = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        Indices = new List<GameObject>();
        UpdateIndices();
    }

    // Handle adding more indices and clicking existing ones
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleRayCast(0);
        }

        else if (Input.GetMouseButtonDown(1))
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
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.green, 10);

        // Create indice if no collider is where the left mouse button is clicked
        if (mouseButtonPressed == 0 && hit.collider == null)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 10;
            pos = Camera.main.ScreenToWorldPoint(pos);
            Instantiate(Indice, pos, Indice.transform.rotation);
            UpdateIndices();
        }
        // Remove indice if there is one where the right mouse button is clicked
        else if (mouseButtonPressed == 1 && hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Indice"))
            {
                Destroy(hit.collider.gameObject);
                Indices.Remove(hit.collider.gameObject);
            }
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
    }
}
