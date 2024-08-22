using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
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
            _gridXText = GetTextComponent("GridX");
            gridX = ParseTextToInt(_gridXText);

            _gridYText = GetTextComponent("GridY");
            gridY = ParseTextToInt(_gridYText);

            _goalText = GetTextComponent("GoalScoreText");
            goal = ParseTextToInt(_goalText);

            _moveLimitText = GetTextComponent("MoveLimitText");
            moveLimit = ParseTextToInt(_moveLimitText);
        }

        private static TMPro.TextMeshProUGUI GetTextComponent(string objectName)
        {
            return GameObject.Find(objectName)?.GetComponent<TMPro.TextMeshProUGUI>();
        }

        private static int ParseTextToInt(TMPro.TextMeshProUGUI textComponent)
        {
            return textComponent != null ? int.Parse(textComponent.text) : 0;
        }

        private void HandleInputForStartScene()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hitObject = GetClickedObject();
                if (hitObject != null)
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

                    if (actions.ContainsKey(hitObject.name)) actions[hitObject.name]();
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

            SceneManager.LoadScene("MainGameScene");
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
            var spriteRenderer = chipPrefabs[_goalChipIndex].transform.Find("Chip").GetComponent<SpriteRenderer>();
            GameObject.Find("GoalChipSprite").GetComponent<Image>().sprite = spriteRenderer.sprite;
        }
    }
}
