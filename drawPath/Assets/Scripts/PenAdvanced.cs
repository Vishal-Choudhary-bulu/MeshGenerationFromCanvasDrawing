using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenAdvanced : MonoBehaviour
{
    public int MaxLengthOfPath = 1000;

    public GameObject penDot;

    public RectTransform gridRect;

    private RectTransform penRect;

    private bool InsideBoard = false;

    private bool Draw = false;

    private Vector2 lastPenPos;
    private Vector2 currentPenPos;

    private float widhtOfOneDot = 0f;

    private bool IsNewDrawing = true;

    private List<GameObject> currentLine = new List<GameObject>();

    private Queue<GameObject> dotsPool = new Queue<GameObject>();

    void Awake()
    {
        InsideBoard = false;
        IsNewDrawing = true;
        widhtOfOneDot = penDot.GetComponent<RectTransform>().rect.width;


        penRect = GetComponent<RectTransform>();
        
        for (int i = 0; i < MaxLengthOfPath; i++)
        {
            GameObject newDot = Instantiate(penDot, Vector2.zero, Quaternion.identity);
            newDot.transform.SetParent(transform);
            newDot.SetActive(false);
            dotsPool.Enqueue(newDot);
        }

    }

    public void StartDrawing()
    {
        
        if (InsideBoard)
        {
            ClearLast();
            Draw = true; 
        }
    }

    public void StopDrawing()
    {
        if (Draw)
        {
            PathCreator.path.DeletePath();
            AddMeshPoints();
            PathCreator.path.CalculatePoints();
            IsNewDrawing = true;
            Draw = false;
        }
    }

    public void OutOfBoard()
    {
        InsideBoard = false;
        StopDrawing();
    }

    public void InBoard()
    {
        InsideBoard = true;
    }

    private void ClearLast()
    {
        foreach(GameObject dot in currentLine)
        {
            dot.SetActive(false);
        }
        currentLine.Clear();
    }

    private void Update()
    {
        if (Draw)
        {
            if (IsNewDrawing)
            {
                IsNewDrawing = false;
                lastPenPos = Input.mousePosition;
            }
            else
            {
                currentPenPos = Input.mousePosition;

                if (Vector2.Distance(currentPenPos, lastPenPos) > widhtOfOneDot/2f)
                {
                    int numberOfDots = Mathf.CeilToInt(Vector2.Distance(currentPenPos, lastPenPos) 
                        / widhtOfOneDot);

                    Vector2 dir = (currentPenPos - lastPenPos).normalized;

                    for (int n = 0; n < numberOfDots; n++)
                    {
                        GameObject dot = dotsPool.Dequeue();
                        var dotRect = dot.GetComponent<RectTransform>();
                        if(dotRect != null)
                        {
                            dotRect.anchorMin = new Vector2(0.0f, 1.0f);
                            dotRect.anchorMax = new Vector2(0.0f, 1.0f);
                            dotRect.pivot = new Vector2(0.5f, 0.5f);
                        }
                        
                        var newPos = lastPenPos + dir * widhtOfOneDot / 2f;

                        dot.transform.position = newPos;

                        dot.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                        dot.SetActive(true);
                        currentLine.Add(dot);
                        dotsPool.Enqueue(dot);
                        lastPenPos = dot.transform.position;
                    }
                }
            }
        }
    }

    private void AddMeshPoints()
    {
        if(currentLine.Count < 6)
        {
            return;
        }

        foreach(var point in currentLine)
        {
            float x = point.GetComponent<RectTransform>().anchoredPosition.x;
            float y = point.GetComponent<RectTransform>().anchoredPosition.y;

            Vector3 newPoint = new Vector3();
            newPoint.x = ReMap(0f, 950f, x);
            newPoint.z = ReMap(-950f, 0f, y);
            newPoint.y = 0.5f;
            PathPoint newPathPoint = new PathPoint(newPoint);
            PathCreator.path.AddToPoints(newPathPoint);
        }
    }

    public float ReMap(float OldMin, float OldMax, float OldValue)
    {
        float NewMin = -2.5f;
        float NewMax = 2.5f;
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}
