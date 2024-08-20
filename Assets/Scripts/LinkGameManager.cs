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
    private string _goalChipTag;
    
    public ChipGrid ChipGrid;
    private ChipLinker _chipLinker;
    
    void Awake()
    {
        _gridX = GameSettings.GridX;
        _gridY = GameSettings.GridY;
        _goal = GameSettings.Goal;
        _moveLimit = GameSettings.MoveLimit;
        _goalChipTag = GameSettings.GoalChipTag;
        
        ChipGrid = new ChipGrid(_gridX, _gridY, chipPrefabs);
        CenterCameraOnGrid();
    }

    void Start()
    {
        _chipLinker = GetComponent<ChipLinker>();
        _chipLinker.OnChipRemoved += UpdateScoreAndMovesLeft;
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
    
    private void UpdateScoreAndMovesLeft(string chipTag, int chipCountInLink)
    {
        
    }
}
