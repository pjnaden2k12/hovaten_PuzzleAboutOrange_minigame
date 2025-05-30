using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzelManager : MonoBehaviour
{
    public int currentLevel = 1;

    private Vector2Int[] originalPositions;
    public List<OrangePiece> pieces;
    public List<ObstaclePiece> obstacles;
    public RectTransform panel;
    public float cellSize = 170f;
    public float spacing = 10f;
    public TMP_Text timerText;
    public GameObject losePanel;
    public GameObject winPanel;

    private float timer = 45f;
    private bool timerRunning = false;
    private bool gameEnded = false;

    // Swipe support
    private Vector2 swipeStart;
    private Vector2 swipeEnd;
    private bool isSwiping = false;

    void Start()
    {
        foreach (var piece in pieces) MoveToAnchoredPos(piece);
        foreach (var obs in obstacles) MoveToAnchoredPos(obs);

        originalPositions = new Vector2Int[pieces.Count];
        for (int i = 0; i < pieces.Count; i++)
            originalPositions[i] = pieces[i].gridPos;

        ResetToInitialState();
    }

    void Update()
    {
        if (gameEnded) return;

        // Keyboard input
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) TriggerMove(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) TriggerMove(Vector2Int.right);
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) TriggerMove(Vector2Int.down);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) TriggerMove(Vector2Int.up);

        //Touch swipe (iOS)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                swipeEnd = touch.position;
                HandleSwipe();
                isSwiping = false;
            }
        }

 #if UNITY_EDITOR
        // Mouse swipe (for testing)
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            swipeEnd = Input.mousePosition;
            HandleSwipe();
            isSwiping = false;
        }
 #endif

        // Timer
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            int timeRemaining = Mathf.CeilToInt(timer);
            timerText.text = $"{timeRemaining}s";

            if (timeRemaining <= 10)
                timerText.color = Color.red;

            if (timer <= 0)
            {
                gameEnded = true;
                timerRunning = false;
                timerText.text = "0s";
                losePanel.SetActive(true);
                Debug.Log("Het thoi gian");
            }
        }
    }

    void HandleSwipe()
    {
        Vector2 delta = swipeEnd - swipeStart;
        if (delta.magnitude < 50f) return;

        Vector2 abs = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

        if (abs.x > abs.y)
        {
            if (delta.x > 0) TriggerMove(Vector2Int.right);
            else TriggerMove(Vector2Int.left);
        }
        else
        {
            if (delta.y > 0) TriggerMove(Vector2Int.down);  // Vuốt lên => di chuyển lên
            else TriggerMove(Vector2Int.up);                // Vuốt xuống => di chuyển xuống
        }
    }


    void TriggerMove(Vector2Int dir)
    {
        if (!timerRunning) timerRunning = true;
        TryMoveAll(dir);
    }

    void TryMoveAll(Vector2Int dir)
    {
        Dictionary<OrangePiece, Vector2Int> originalPos = new();
        Dictionary<OrangePiece, Vector2Int> targetPos = new();
        HashSet<Vector2Int> obstacleGrid = new();

        foreach (var piece in pieces) originalPos[piece] = piece.gridPos;
        foreach (var obs in obstacles) obstacleGrid.Add(obs.gridPos);

        foreach (var piece in pieces)
        {
            Vector2Int target = piece.gridPos + dir;
            if (target.x < 0 || target.x >= 4 || target.y < 0 || target.y >= 4 || obstacleGrid.Contains(target))
                target = piece.gridPos;

            targetPos[piece] = target;
        }

        foreach (var piece in pieces)
        {
            Vector2Int target = targetPos[piece];
            foreach (var other in pieces)
            {
                if (piece != other && originalPos[other] == target && targetPos[other] == originalPos[other])
                {
                    targetPos[piece] = originalPos[piece];
                    break;
                }
            }
        }

        Dictionary<Vector2Int, int> destCount = new();
        foreach (var kvp in targetPos)
        {
            if (!destCount.ContainsKey(kvp.Value)) destCount[kvp.Value] = 0;
            destCount[kvp.Value]++;
        }

        foreach (var piece in pieces)
        {
            Vector2Int target = targetPos[piece];
            if (destCount[target] > 1)
                targetPos[piece] = originalPos[piece];
        }

        foreach (var piece in pieces)
        {
            piece.gridPos = targetPos[piece];
            MoveToAnchoredPos(piece);
        }

        CheckWin();
    }

    void CheckWin()
    {
        List<Dictionary<Vector2Int, int>> winConditions = GetWinConditions(currentLevel);

        foreach (var goal in winConditions)
        {
            bool allMatch = true;
            foreach (var kvp in goal)
            {
                var match = pieces.Find(p => p.gridPos == kvp.Key && p.id == kvp.Value);
                if (match == null) { allMatch = false; break; }
            }

            if (allMatch)
            {
                Debug.Log("Ban da chien thang");
                winPanel.SetActive(true);
                gameEnded = true;
                timerRunning = false;
                PlayerPrefs.SetInt($"Level{currentLevel}_Completed", 1);
                return;
            }
        }
    }

    List<Dictionary<Vector2Int, int>> GetWinConditions(int level)
    {
        switch (level)
        {
            case 1:
                return new List<Dictionary<Vector2Int, int>>
                {
                    new Dictionary<Vector2Int, int>
                    {
                        [new Vector2Int(2, 0)] = 1,
                        [new Vector2Int(3, 0)] = 2,
                        [new Vector2Int(2, 1)] = 4,
                        [new Vector2Int(3, 1)] = 3,
                    },
                    new Dictionary<Vector2Int, int>
                    {
                        [new Vector2Int(2, 1)] = 1,
                        [new Vector2Int(3, 1)] = 2,
                        [new Vector2Int(2, 2)] = 4,
                        [new Vector2Int(3, 2)] = 3,
                    },
                    new Dictionary<Vector2Int, int>
                    {
                        [new Vector2Int(0, 2)] = 1,
                        [new Vector2Int(1, 2)] = 2,
                        [new Vector2Int(0, 3)] = 4,
                        [new Vector2Int(1, 3)] = 3,
                    }
                };
            case 2:
                return new List<Dictionary<Vector2Int, int>>
                {
                    new Dictionary<Vector2Int, int>
                    {
                        [new Vector2Int(0, 1)] = 1,
                        [new Vector2Int(1, 1)] = 2,
                        [new Vector2Int(0, 2)] = 4,
                        [new Vector2Int(1, 2)] = 3,
                    },
                    new Dictionary<Vector2Int, int>
                    {
                        [new Vector2Int(0, 2)] = 1,
                        [new Vector2Int(1, 2)] = 2,
                        [new Vector2Int(0, 3)] = 4,
                        [new Vector2Int(1, 3)] = 3,
                    }
                };
            case 3:
                return new List<Dictionary<Vector2Int, int>>
                {
                    new Dictionary<Vector2Int, int>
                    {
                        [new Vector2Int(1, 2)] = 1,
                        [new Vector2Int(2, 2)] = 2,
                        [new Vector2Int(1, 3)] = 4,
                        [new Vector2Int(2, 3)] = 3,
                    }
                };
            default:
                return new List<Dictionary<Vector2Int, int>>();
        }
    }

    void MoveToAnchoredPos(MonoBehaviour mb)
    {
        Vector2Int gridPos = mb is OrangePiece op ? op.gridPos : (mb as ObstaclePiece).gridPos;

        float startX = -(cellSize * 1.5f + spacing * 1.5f);
        float startY = (cellSize * 1.5f + spacing * 1.5f);
        float x = startX + gridPos.x * (cellSize + spacing);
        float y = startY - gridPos.y * (cellSize + spacing);

        mb.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }

    public void ResetToInitialState()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].gridPos = originalPositions[i];
            MoveToAnchoredPos(pieces[i]);
        }

        timer = 45f;
        timerRunning = false;
        gameEnded = false;

        if (timerText != null)
        {
            timerText.text = "45s";
            timerText.color = Color.white;
        }

        losePanel?.SetActive(false);
        winPanel?.SetActive(false);
    }
}
