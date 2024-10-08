using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ChipLinker : MonoBehaviour
    {
        public event Action<string, int> OnChipRemoved;
        
        [SerializeField] private Camera mainCamera;
        private ChipGrid _chipGrid;
        private readonly List<GameObject> _linkedChips = new List<GameObject>();
        private bool _isLinking = false;
        private GameObject _firstChip;
        private int _startX, _startY;
        private LineRenderer _lineRenderer;

        void Start()
        {
            _chipGrid = FindObjectOfType<LinkGameManager>().ChipGrid;
            if(_chipGrid == null)
                throw new NullReferenceException("ChipGrid is null");
            
            _lineRenderer = GameObject.Find("LinkDrawer").GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 0;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) OnMouseDown();
            if (Input.GetMouseButton(0) && _isLinking) OnMouseDrag();
            if (Input.GetMouseButtonUp(0) && _isLinking) OnMouseUp();
        }

        private void OnMouseDown()
        {
            if (!mainCamera) return;

            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider)
            {
                _firstChip = hit.collider.gameObject;
                _isLinking = true;
                _linkedChips.Add(_firstChip);
                _startX = (int)_firstChip.transform.position.x;
                _startY = (int)_firstChip.transform.position.y;
                _lineRenderer.positionCount = 1;
                _lineRenderer.SetPosition(0, _firstChip.transform.position);
            }
        }

        private void OnMouseDrag()
        {
            if (!mainCamera) return;

            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider)
            {
                GameObject newChip = hit.collider.gameObject;
                int newX = (int)newChip.transform.position.x;
                int newY = (int)newChip.transform.position.y;

                if (IsAdjacent(_startX, _startY, newX, newY) && newChip != _firstChip && newChip.CompareTag(_firstChip.tag) && !_linkedChips.Contains(newChip))
                {
                    _linkedChips.Add(newChip);
                    _startX = newX;
                    _startY = newY;
                    
                    _lineRenderer.positionCount = _linkedChips.Count;
                    _lineRenderer.SetPosition(_linkedChips.Count - 1, newChip.transform.position);
                }
            }
        }

        private void OnMouseUp()
        {
            _isLinking = false;

            if (_linkedChips.Count >= 3)
            {
                List<int> columnsToCheck = new List<int>();

                foreach (var chip in _linkedChips)
                {
                    int x = (int)chip.transform.position.x;
                    int y = (int)chip.transform.position.y;
                    Destroy(chip);
                    _chipGrid.GridArray[x, y] = null;

                    if (!columnsToCheck.Contains(x))
                    {
                        columnsToCheck.Add(x);
                    }
                }
                
                OnChipRemoved?.Invoke(_firstChip.tag, _linkedChips.Count);

                _linkedChips.Clear();
                _lineRenderer.positionCount = 0;

                StartCoroutine(HandleFallingAndSpawning(columnsToCheck));
            }
            else
            {
                _linkedChips.Clear();
                _lineRenderer.positionCount = 0;
            }
        }

        private IEnumerator HandleFallingAndSpawning(List<int> columnsToCheck)
        {
            yield return DropAllCubes(columnsToCheck);
            yield return FillAllColumnsWithNewCubes(columnsToCheck);
        }

        private IEnumerator DropAllCubes(List<int> columnsToCheck)
        {
            List<IEnumerator> moveCoroutines = new List<IEnumerator>();

            foreach (int column in columnsToCheck)
            {
                for (int y = 0; y < _chipGrid.Height; y++)
                {
                    if (!_chipGrid.GridArray[column, y])
                    {
                        for (int aboveY = y + 1; aboveY < _chipGrid.Height; aboveY++)
                        {
                            if (_chipGrid.GridArray[column, aboveY])
                            {
                                _chipGrid.GridArray[column, y] = _chipGrid.GridArray[column, aboveY];
                                _chipGrid.GridArray[column, aboveY] = null;

                                moveCoroutines.Add(SmoothMoveChip(_chipGrid.GridArray[column, y], new Vector3(column, y, 0)));
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var coroutine in moveCoroutines)
            {
                StartCoroutine(coroutine);
            }

            foreach (var coroutine in moveCoroutines)
            {
                yield return coroutine;
            }
        }

        private IEnumerator FillAllColumnsWithNewCubes(List<int> columnsToCheck)
        {
            List<IEnumerator> spawnCoroutines = new List<IEnumerator>();

            foreach (int column in columnsToCheck)
            {
                for (int y = 0; y < _chipGrid.Height; y++)
                {
                    if (!_chipGrid.GridArray[column, y])
                    {
                        _chipGrid.CreateAndPlaceChip(column, y);
                        var newChip = _chipGrid.GridArray[column, y];
                        spawnCoroutines.Add(SmoothMoveChip(newChip, new Vector3(column, y, 0)));
                    }
                }
            }

            foreach (var coroutine in spawnCoroutines)
            {
                StartCoroutine(coroutine);
            }

            foreach (var coroutine in spawnCoroutines)
            {
                yield return coroutine;
            }
        }

        private static IEnumerator SmoothMoveChip(GameObject chip, Vector3 targetPosition)
        {
            float elapsedTime = 0f;
            float duration = 0.1f;
            Vector3 startPosition = chip.transform.position;

            while (elapsedTime < duration)
            {
                chip.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            chip.transform.position = targetPosition;
        }
        
        private static bool IsAdjacent(int x1, int y1, int x2, int y2)
        {
            return Mathf.Abs(x1 - x2) <= 1 && Mathf.Abs(y1 - y2) <= 1;
        }
    }
}
