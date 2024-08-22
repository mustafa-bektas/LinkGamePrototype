using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class LinkGameManager : MonoBehaviour
    {
        public ChipGrid ChipGrid;
        [SerializeField] private GameObject[] chipPrefabs;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Camera mainCamera;

        private int _gridX;
        private int _gridY;
        private int _goal;
        private int _moveLimit;
        private string _goalChipTag;
        private ChipLinker _chipLinker;

        private TMPro.TextMeshProUGUI _goalText;
        private TMPro.TextMeshProUGUI _moveLimitText;

        void Awake()
        {
            InitializeGameSettings();
            InitializeUIElements();
            SetupGrid();
            CenterCameraOnGrid();
        }

        void Start()
        {
            _chipLinker = GetComponent<ChipLinker>();
            _chipLinker.OnChipRemoved += UpdateScoreAndMovesLeft;
        }

        private void InitializeGameSettings()
        {
            _gridX = GameSettings.GridX;
            _gridY = GameSettings.GridY;
            _goal = GameSettings.Goal;
            _moveLimit = GameSettings.MoveLimit;
            _goalChipTag = GameSettings.GoalChipTag;
        }

        private void InitializeUIElements()
        {
            _goalText = GameObject.Find("GoalLeftText").GetComponent<TMPro.TextMeshProUGUI>();
            _goalText.text = _goal.ToString();

            _moveLimitText = GameObject.Find("MovesLeftText").GetComponent<TMPro.TextMeshProUGUI>();
            _moveLimitText.text = _moveLimit.ToString();

            SetGoalChipSprite();
        }

        private void SetupGrid()
        {
            ChipGrid = new ChipGrid(_gridX, _gridY, chipPrefabs);
        }

        private void CenterCameraOnGrid()
        {
            if (mainCamera)
            {
                mainCamera.orthographicSize = Mathf.Max(_gridX, _gridY);
                mainCamera.transform.position = new Vector3((_gridX - 1) / 2f, (_gridY - 1) / 2f, mainCamera.transform.position.z);
            }
        }

        private void UpdateScoreAndMovesLeft(string chipTag, int chipCountInLink)
        {
            _moveLimit--;
            _moveLimit = Mathf.Max(0, _moveLimit);
            _moveLimitText.text = _moveLimit.ToString();

            if (chipTag == _goalChipTag)
            {
                _goal -= chipCountInLink;
                _goal = Mathf.Max(0, _goal);
                _goalText.text = _goal.ToString();
            }

            CheckForWinOrLose();
        }

        private void SetGoalChipSprite()
        {
            foreach (var chipPrefab in chipPrefabs)
            {
                if (chipPrefab.CompareTag(_goalChipTag))
                {
                    GameObject.Find("GoalChipSprite").GetComponent<Image>().sprite =
                        chipPrefab.transform.Find("Chip").GetComponent<SpriteRenderer>().sprite;
                    break;
                }
            }
        }

        private void CheckForWinOrLose()
        {
            if (_goal <= 0)
            {
                // Win
                ShowGameOverPanel("You Win!");
            }
            else if (_moveLimit <= 0)
            {
                // Lose
                ShowGameOverPanel("You Lose!");
            }
        }

        private IEnumerator FadeInGameOverPanel(CanvasGroup canvasGroup)
        {
            float duration = 0.5f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        private void ShowGameOverPanel(string message)
        {
            gameOverPanel.SetActive(true);

            TMPro.TextMeshProUGUI gameOverText = gameOverPanel.transform.Find("GameOverText").GetComponent<TMPro.TextMeshProUGUI>();
            gameOverText.text = message;

            Button restartButton = gameOverPanel.transform.Find("RestartButton").GetComponent<Button>();
            restartButton.onClick.AddListener(RestartGame);

            CanvasGroup canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;

            StartCoroutine(FadeInGameOverPanel(canvasGroup));
        }

        private void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
        }
    }
}
