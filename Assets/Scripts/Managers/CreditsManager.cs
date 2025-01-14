using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class CreditsScroll : MonoBehaviour
{
    public float scrollSpeed = 200f;  
    public float resetPositionY = -2000f; 
    public TextMeshProUGUI textMeshPro; 
    public CanvasGroup canvasGroup; 
    public RectTransform rectTransform; 

    private string creditsText =
        "<size=100><align=center>To be continued...</align></size>\n\n" +
        "<size=100><align=center>Thank you for playing.</align></size>\n\n" +

        "<size=150><align=center><b>Uriel Munguia</b></align></size>\n\n" +
        "<size=100><align=center>Project Management</align></size>\n" +
        "<size=100><align=center>Lead Programmer</align></size>\n" +
        "<size=100><align=center>Lead Game Designer</align></size>\n" +
        "<size=100><align=center>Art and Animation</align></size>\n" +

        "<size=150><align=center><b>Brandon Munguia Torres</b></align></size>\n\n" +
        "<size=100><align=center>Lead AI Programmer</align></size>\n" +
        "<size=100><align=center>Programmer</align></size>\n" +
        "<size=100><align=center>Game Designer</align></size>\n" +
        "<size=100><align=center>Music Designer</align></size>\n\n" +

        "<size=150><align=center><b>Michael Nguyen</b></align></size>\n\n" +
        "<size=100><align=center>Chrome Extension Creator</align></size>\n" +
         "<size=100><align=center>Programmer</align></size>\n" +
         "<size=100><align=center>Game Designer</align></size>\n" +

        "<size=100><align=center>Credit to Assets Used.</align></size>\n\n" +

        "<size=150><align=center><b>Sound Composers</b></align></size>\n\n" +
        "<size=100><align=center>Royalty Free Sounds-Youtube: Shadows in the Night (Credits)</align></size>\n" +
        "<size=100><align=center>Jonathan Barretto-Youtube: Original Random (Mover's House)</align></size>\n" +
        "<size=100><align=center>Music4Video-Youtube: Jazz Background (Tweaker's House CLEAR)</align></size>\n" +
        "<size=100><align=center>S3pt3mb3rw-Youtube: Deep Thoughts (Outside)</align></size>\n" +
        "<size=100><align=center>Epic Music Journey-Youtube 8 Bit RPG Battle (Tweaker's House)</align></size>\n" +
        "<size=100><align=center>Stream Cafe-Youtube Grandma's House (Grandma's House)</align></size>\n" +

        "<size=150><align=center><b>3D Artists</b></align></size>\n\n" +
        "<size=100><align=center>PolyOne Studio-SketchFab - Chibi Base Mesh</align></size>\n" +
        "<size=100><align=center>Merow - Sketchfab Pixel A-Frame</align></size>\n" +
        "<size=100><align=center>FlipBard-Sketfab Frying Pan with Fried Eggs</align></size>\n" +
        "<size=100><align=center>Soleil Neige-Sketchfab Pixel Art Bench</align></size>\n" +
        "<size=100><align=center>Elbolillo-Sketchfab ArmChair</align></size>\n" +
        "<size=100><align=center>Elbolillo-Sketchfab Debs Others</align></size>\n" +

        "<size=150><align=center><b>Special Thanks</b></align></size>\n\n" +
        "<size=100><align=center>DevPost</align></size>\n" +
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