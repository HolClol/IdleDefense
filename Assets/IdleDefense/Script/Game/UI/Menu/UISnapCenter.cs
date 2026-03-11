using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISnapCenter : MonoBehaviour
{
    public ScrollRect ScrollRect;
    public RectTransform ContentPanel;
    public float SnapSpeed = 10f; 
    public float SnapThreshold = 0.2f; 

    private RectTransform[] contentElements;
    private Vector2[] elementPositions;
    private Vector2 targetPosition;
    private bool isDragging = false;

    void Start()
    {
        Invoke("CalculateElementPositions", 0.1f);
    }

    void CalculateElementPositions()
    {
        // Get only the direct children of the content panel
        int childCount = ContentPanel.childCount;
        contentElements = new RectTransform[childCount];
        elementPositions = new Vector2[childCount];

        for (int i = 0; i < childCount; i++)
        {
            contentElements[i] = ContentPanel.GetChild(i) as RectTransform;
            // Calculate the positions based on the anchored positions
            elementPositions[i] = new Vector2(-contentElements[i].anchoredPosition.x + 250, -contentElements[i].anchoredPosition.y);
        }
    }

    void Update()
    {
        if (!isDragging)
            ContentPanel.anchoredPosition = Vector2.Lerp(ContentPanel.anchoredPosition, targetPosition, SnapSpeed * Time.deltaTime);
    }

    public void OnBeginDrag()
    {
        isDragging = true;
    }

    public void OnEndDrag()
    {
        isDragging = false;

        // Find the closest element to snap to
        float closestDistance = float.MaxValue;
        for (int i = 0; i < elementPositions.Length; i++)
        {
            float distance = Vector2.Distance(ContentPanel.anchoredPosition, elementPositions[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetPosition = elementPositions[i];
                targetPosition.y = 0f;
            }
        }

        if (closestDistance < SnapThreshold)
        {
            targetPosition = ContentPanel.anchoredPosition;
            targetPosition.y = 0f;

        }
    }
}
