using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeadPad : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform touchpadRect; // Reference to the square panel's RectTransform
    private RectTransform buttonRect; // Reference to the button's RectTransform
    private RectTransform buttonAnimRect; //Reference to the button glow (to be animated)

    private bool isDragging = false; // Track if the button is being dragged

    private float maxX; // Maximum X value of the touchpad
    private float maxY; // Maximum Y value of the touchpad

    public float xValue;
    public float yValue;
    public bool headPadActive = false;
    public bool headPadEnter = false;
   
    private Coroutine animCoroutine; // Reference to the running animation coroutine
    private Coroutine animFadeCoroutine;
    [SerializeField] private AnimationCurve ButtonAnimCurve;

    private void Start()
    {
        touchpadRect = GetComponent<RectTransform>();
        buttonRect = transform.GetChild(0).GetComponent<RectTransform>();
        buttonAnimRect = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        maxX = touchpadRect.rect.width / 2f; 
        maxY = touchpadRect.rect.height / 2f;
        Debug.Log("maxX: " + maxX + ", maxY: " + maxY);
        buttonAnimRect.localScale = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        headPadEnter = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        headPadEnter = false;
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
            if (animFadeCoroutine != null)
            {
                StopCoroutine(animFadeCoroutine);
            }                
            StartCoroutine(AnimateButton(1f, 1f));
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
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
        }
        StartCoroutine(ReverseAnimateButton(0f, 0f));

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
    private IEnumerator AnimateButton(float targetScale, float targetAlpha)
    {
        float duration = 0.5f; // Adjust the duration of the animation as desired
        float elapsed = 0f;
        Vector3 initialScale = Vector3.zero;  // or new Vector3(0.1f, 0.1f, 0.1f) for a small percentage scale
        Color initialColor = buttonAnimRect.GetComponent<Image>().color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Animate the scale
            buttonAnimRect.localScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, buttonAnimRect.localScale.z), ButtonAnimCurve.Evaluate(t));

            // Animate the alpha
            Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
            buttonAnimRect.GetComponent<Image>().color = Color.Lerp(initialColor, targetColor, t);

            yield return null;
        }

        // Ensure the final values are set
        buttonAnimRect.localScale = new Vector3(targetScale, targetScale, 1f);
        buttonAnimRect.GetComponent<Image>().color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
    }

    private IEnumerator ReverseAnimateButton(float targetScale, float targetAlpha)
    {
        float duration = 0.5f; // Adjust the duration of the animation as desired
        float elapsed = 0f;
        Vector3 initialScale = buttonAnimRect.localScale;
        Color initialColor = buttonAnimRect.GetComponent<Image>().color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Animate the scale
            buttonAnimRect.localScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, buttonAnimRect.localScale.z), ButtonAnimCurve.Evaluate(t));

            // Animate the alpha
            Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
            buttonAnimRect.GetComponent<Image>().color = Color.Lerp(initialColor, targetColor, t);

            yield return null;
        }

        // Ensure the final values are set
        buttonAnimRect.localScale = new Vector3(targetScale, targetScale, 1f);
        buttonAnimRect.GetComponent<Image>().color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
    }
}