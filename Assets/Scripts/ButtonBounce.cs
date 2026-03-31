using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonBounce : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float pressedScale = 0.9f;
    [SerializeField] float animationSpeed = 10f;

    Vector3 originalScale;
    bool isPressed = false;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale * pressedScale));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale));
    }

    IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.0001f)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                target,
                Time.unscaledDeltaTime * animationSpeed
            );
            yield return null;
        }
        transform.localScale = target;
    }
}