using UnityEngine;
using TMPro;
using System.Collections;

public class CreditsScroll : MonoBehaviour
{
    public float scrollSpeed = 200f;  
    public float resetPositionY = -2000f; 
    public TextMeshProUGUI textMeshPro; 
    public CanvasGroup canvasGroup; 
    public RectTransform rectTransform; 

    private string creditsText =
        "<size=150><align=center><b>Project Management</b></align></size>\n\n" +
        "<size=100><align=center>Uriel Munguia</align></size>\n\n" +

        "<size=150><align=center><b>Game Design</b></align></size>\n\n" +
        "<size=100><align=center>Uriel Munguia</align></size>\n" +
        "<size=100><align=center>Brandon Munguia Torres</align></size>\n" +
        "<size=100><align=center>Michael Nguyen</align></size>\n\n" +

        "<size=150><align=center><b>Chrome Extension Designer</b></align></size>\n\n" +
        "<size=100><align=center>Michael Nguyen</align></size>\n\n" +

        "<size=150><align=center><b>Art and Animation</b></align></size>\n\n" +
        "<size=100><align=center>Uriel Munguia</align></size>\n\n" +

        "<size=150><align=center><b>Programming and Engineering</b></align></size>\n\n" +
        "<size=100><align=center>Uriel Munguia</align></size>\n" +
        "<size=100><align=center>Michael Nguyen</align></size>\n" +
        "<size=100><align=center>Brandon Munguia Torres</align></size>\n\n" +

        "<size=150><align=center><b>Sound Composers</b></align></size>\n\n" +
        "<size=100><align=center>Fill in</align></size>\n" +

        "<size=150><align=center><b>3D Artists</b></align></size>\n\n" +
        "<size=100><align=center>PolyOne Studio on SketchFab - Chibi Base Mesh</align></size>\n" +

        "<size=150><align=center><b>Special Thanks</b></align></size>\n\n" +
        "<size=100><align=center>Amazon Web Services</align></size>\n" +
        "<size=100><align=center>Amazon Titan Text G1 - Premier</align></size>\n";

    void Start()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        if (textMeshPro != null)
        {
            textMeshPro.text = creditsText;
            AdjustTextBoxSize();
        }

        StartCoroutine(FadeInCredits());
    }

    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        if (rectTransform.anchoredPosition.y > Screen.height)
        {
            //rectTransform.anchoredPosition = new Vector2(0, resetPositionY);
        }
    }

    IEnumerator FadeInCredits()
    {
        float fadeDuration = 3f; 
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    void AdjustTextBoxSize()
    {
        textMeshPro.ForceMeshUpdate();

        float preferredHeight = textMeshPro.preferredHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);
    }
}