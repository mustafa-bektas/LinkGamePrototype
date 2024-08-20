using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class StartSceneManager : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public int goal;
    public int moveLimit;
    
    public GameObject[] chipPrefabs;
    private int _goalChipIndex;
    
    private TMPro.TextMeshProUGUI _gridXText;
    private TMPro.TextMeshProUGUI _gridYText;
    private TMPro.TextMeshProUGUI _goalText;
    private TMPro.TextMeshProUGUI _moveLimitText;
    
    void Start()
    {
        InitializeUIElements();
    }

    void Update()
    {
        HandleInputForStartScene();
    }

    private void InitializeUIElements()
    {
        _gridXText = GameObject.Find("GridX").GetComponent<TMPro.TextMeshProUGUI>();
        gridX = _gridXText != null ? int.Parse(_gridXText.text) : 0;
        
        _gridYText = GameObject.Find("GridY").GetComponent<TMPro.TextMeshProUGUI>();
        gridY = _gridYText != null ? int.Parse(_gridYText.text) : 0;
        
        _goalText = GameObject.Find("GoalScoreText").GetComponent<TMPro.TextMeshProUGUI>();
        goal = _goalText != null ? int.Parse(_goalText.text) : 0;
        
        _moveLimitText = GameObject.Find("MoveLimitText").GetComponent<TMPro.TextMeshProUGUI>();
        moveLimit = _moveLimitText != null ? int.Parse(_moveLimitText.text) : 0;
    }

    private void HandleInputForStartScene()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hitObject = GetClickedObject();
            if (hitObject)
            {
                var actions = new Dictionary<string, Action>
                {
                    { "UpArrowXText", () => AdjustValue(ref gridX, _gridXText, 1) },
                    { "DownArrowXText", () => AdjustValue(ref gridX, _gridXText, -1) },
                    { "UpArrowYText", () => AdjustValue(ref gridY, _gridYText, 1) },
                    { "DownArrowYText", () => AdjustValue(ref gridY, _gridYText, -1) },
                    { "UpArrowGoalText", () => AdjustValue(ref goal, _goalText, 1) },
                    { "DownArrowGoalText", () => AdjustValue(ref goal, _goalText, -1) },
                    { "UpArrowMoveLimitText", () => AdjustValue(ref moveLimit, _moveLimitText, 1) },
                    { "DownArrowMoveLimitText", () => AdjustValue(ref moveLimit, _moveLimitText, -1) },
                    { "GoalChipSprite", UpdateGoalChipSprite },
                    { "PlayButtonText", SaveSettingsAndStartGame }
                };

                if (actions.ContainsKey(hitObject.name))
                {
                    actions[hitObject.name]();
                }
            }
        }
    }

    private void SaveSettingsAndStartGame()
    {
        GameSettings.GridX = gridX;
        GameSettings.GridY = gridY;
        GameSettings.Goal = goal;
        GameSettings.MoveLimit = moveLimit;
        GameSettings.GoalChipTag = chipPrefabs[_goalChipIndex].tag;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainGameScene");
    }

    private static GameObject GetClickedObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0 ? results[0].gameObject : null;
    }

    private static void AdjustValue(ref int value, TMPro.TextMeshProUGUI textComponent, int delta)
    {
        value += delta;
        textComponent.text = value.ToString();
    }
    
    private void UpdateGoalChipSprite()
    {
        _goalChipIndex = (_goalChipIndex + 1) % chipPrefabs.Length;
        GameObject.Find("GoalChipSprite").GetComponent<Image>().sprite = 
            chipPrefabs[_goalChipIndex].transform.Find("Chip").GetComponent<SpriteRenderer>().sprite;
    }
}
