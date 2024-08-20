using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LinkGameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] chipPrefabs;
    [SerializeField] private Camera mainCamera;
    private int _gridX;
    private int _gridY;
    private int _goal;
    private int _moveLimit;
    
    public ChipGrid ChipGrid;
    
    void Awake()
    {
        _gridX = GameSettings.GridX;
        _gridY = GameSettings.GridY;
        _goal = GameSettings.Goal;
        _moveLimit = GameSettings.MoveLimit;
        //GameObject.Find("GoalChip").GetComponent<SpriteRenderer>().sprite = Array.Find(chipPrefabs, chip => chip.name == GameSettings.GoalChipName).GetComponent<SpriteRenderer>().sprite;
        
        ChipGrid = new ChipGrid(_gridX, _gridY, chipPrefabs);
        CenterCameraOnGrid();
    }

    void Update()
    {
        
    }
    
    private void CenterCameraOnGrid()
    {
        if (mainCamera)
        {
            mainCamera.orthographicSize = Mathf.Max(_gridX, _gridY);
            mainCamera.transform.position = new Vector3( (_gridX-1) / 2f, (_gridY-1) / 2f, mainCamera.transform.position.z);
        }
    }
}
