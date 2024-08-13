using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISnapCenter : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public float snapSpeed = 10f; // Speed at which it snaps to the nearest element
    public float snapThreshold = 0.2f; // Threshold to consider snapping

    private RectTransform[] contentElements;
    private Vector2[] elementPositions;
    private Vector2 targetPosition;
    private bool isDragging = false;

    void Start()
    {
        // Delay the position calculation until after the layout has been fully applied
        Invoke("CalculateElementPositions", 0.1f);
    }

    void CalculateElementPositions()
    {
        // Get only the direct children of the content panel
        int childCount = contentPanel.childCount;
        contentElements = new RectTransform[childCount];
        elementPositions = new Vector2[childCount];

        for (int i = 0; i < childCount; i++)
        {
            contentElements[i] = contentPanel.GetChild(i) as RectTransform;
            // Calculate the positions based on the anchored positions
            elementPositions[i] = new Vector2(-contentElements[i].anchoredPosition.x + 250, -contentElements[i].anchoredPosition.y);
        }
    }

    void Update()
    {
        if (!isDragging)
        {
            // Smoothly move the content to the target position
            contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, snapSpeed * Time.deltaTime);
        }
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
            float distance = Vector2.Distance(contentPanel.anchoredPosition, elementPositions[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetPosition = elementPositions[i];
                targetPosition.y = 0f;
            }
        }

        // If within the snap threshold, snap to the nearest element
        if (closestDistance < snapThreshold)
        {
            targetPosition = contentPanel.anchoredPosition;
            targetPosition.y = 0f;

        }
    }
}
