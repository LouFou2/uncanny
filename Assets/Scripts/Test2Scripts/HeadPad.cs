using UnityEngine;
using UnityEngine.EventSystems;

public class HeadPad : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private RectTransform touchpadRect; // Reference to the square panel's RectTransform
    private RectTransform buttonRect; // Reference to the button's RectTransform

    private bool isDragging = false; // Track if the button is being dragged

    private float maxX; // Maximum X value of the touchpad
    private float maxY; // Maximum Y value of the touchpad

    public float xValue;
    public float yValue;
    public bool headPadActive = false;

    private void Start()
    {
        touchpadRect = GetComponent<RectTransform>();
        buttonRect = transform.GetChild(0).GetComponent<RectTransform>();

        maxX = touchpadRect.rect.width / 2f; 
        maxY = touchpadRect.rect.height / 2f;
        Debug.Log("maxX: " + maxX + ", maxY: " + maxY);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if the pointer is clicking on the button
        if (RectTransformUtility.RectangleContainsScreenPoint(buttonRect, eventData.position))
        {
            isDragging = true;
            UpdateButtonPosition(eventData.position);
            headPadActive = true; 
            Debug.Log("Head Pad On");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            UpdateButtonPosition(eventData.position);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        headPadActive = false;
        Debug.Log("Head Pad Off");
    }

    private void UpdateButtonPosition(Vector2 position)
    {
        Vector2 touchpadCenter = touchpadRect.rect.center;       
        Vector2 buttonOffset = new Vector2(buttonRect.rect.width / 2f, buttonRect.rect.height / 2f);
        Vector2 localPosition = position - new Vector2(touchpadRect.position.x, touchpadRect.position.y);

        // Calculate the clamping bounds based on the touchpad's size and position
        float minXClamp = touchpadCenter.x - maxX + buttonOffset.x;
        float maxXClamp = touchpadCenter.x + maxX - buttonOffset.x;
        float minYClamp = touchpadCenter.y - maxY + buttonOffset.y;
        float maxYClamp = touchpadCenter.y + maxY - buttonOffset.y;

        // Clamp the button's position within the touchpad's bounds
        localPosition.x = Mathf.Clamp(localPosition.x, minXClamp, maxXClamp);
        localPosition.y = Mathf.Clamp(localPosition.y, minYClamp, maxYClamp);

        // Set the button's position
        buttonRect.localPosition = localPosition;

        // Map the button's position to range between -1 and 1 for X and Y
        float mappedX = Mathf.InverseLerp(minXClamp, maxXClamp, localPosition.x);
        float mappedY = Mathf.InverseLerp(minYClamp, maxYClamp, localPosition.y);

        // Return the X and Y values
        xValue = Mathf.Clamp(mappedX * 2f - 1f, -1f, 1f);
        yValue = Mathf.Clamp(mappedY * 2f - 1f, -1f, 1f);

        // Use xValue and yValue for your desired functionality
        //Debug.Log("X: " + xValue + ", Y: " + yValue);
    }

    /*
    private void UpdateButtonPosition(Vector2 position)
    {
        Vector2 localPosition = ClampToTouchpadBounds(position);
        Vector2 buttonOffset = new Vector2(buttonRect.rect.width / 2f, buttonRect.rect.height / 2f);
        buttonRect.anchoredPosition = localPosition - buttonOffset;

        // Calculate the clamping bounds with button size adjustment
        float minXClamp = -maxX + buttonOffset.x;
        float maxXClamp = maxX - buttonOffset.x;
        float minYClamp = -maxY + buttonOffset.y;
        float maxYClamp = maxY - buttonOffset.y;

        // Clamp the button's position within the touchpad's bounds
        Vector2 clampedPosition = buttonRect.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minXClamp, maxXClamp);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minYClamp, maxYClamp);
        buttonRect.anchoredPosition = clampedPosition;

        // Map the button's position to range between -1 and 1 for X and Y
        float mappedX = clampedPosition.x / maxXClamp;
        float mappedY = clampedPosition.y / maxYClamp;

        // Return the X and Y values
        xValue = Mathf.Clamp(mappedX, -1f, 1f);
        yValue = Mathf.Clamp(mappedY, -1f, 1f);

        // Use xValue and yValue for your desired functionality
        Debug.Log("X: " + xValue + ", Y: " + yValue);

    }

    private Vector2 ClampToTouchpadBounds(Vector2 position)
    {
        Vector2 touchpadCenter = touchpadRect.rect.center;
        Vector2 localPosition = position - touchpadCenter;

        localPosition.x = Mathf.Clamp(localPosition.x, -maxX, maxX);
        localPosition.y = Mathf.Clamp(localPosition.y, -maxY, maxY);

        return localPosition;
    }
    */
}