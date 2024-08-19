using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LinkGameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] chipPrefabs;
    [SerializeField] private int width;
    [SerializeField] private int height;
    
    public ChipGrid ChipGrid;
    
    // Start is called before the first frame update
    void Awake()
    {
        ChipGrid = new ChipGrid(width, height, chipPrefabs);
        CenterCameraOnGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void CenterCameraOnGrid()
    {
        if (Camera.main != null)
            Camera.main.transform.position = new Vector3( (width-1) / 2f, (height-1) / 2f, Camera.main.transform.position.z);
    }
}
