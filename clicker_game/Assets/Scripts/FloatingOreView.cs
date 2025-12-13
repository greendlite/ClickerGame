using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatingOreView : MonoBehaviour
{
    [SerializeField] private Image oreIcon;
    [SerializeField] private Text amountText;

    private CanvasGroup canvasGroup;
    private RectTransform rect;

    private float lifeTime = 1f;
    private float moveUpDistance = 80f;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(Sprite icon, int amount)
    {
        oreIcon.gameObject.SetActive(true);
        oreIcon.sprite = icon;

        amountText.text = $"+{amount}";

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * moveUpDistance;

        float t = 0f;

        while (t < lifeTime)
        {
            t += Time.deltaTime;
            float k = t / lifeTime;

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, k);
            canvasGroup.alpha = 1f - k;

            yield return null;
        }

        Destroy(gameObject);
    }
}
