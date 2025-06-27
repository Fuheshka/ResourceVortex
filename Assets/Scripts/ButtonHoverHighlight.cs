using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonHoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Text buttonText;
    private TextMeshProUGUI tmpText;

    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color normalTextColor = Color.white;
    public Color highlightTextColor = Color.yellow;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        SetNormalColor();
        SetNormalTextColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlightColor();
        SetHighlightTextColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetNormalColor();
        SetNormalTextColor();
    }

    private void SetHighlightColor()
    {
        ColorBlock colors = button.colors;
        colors.normalColor = highlightColor;
        colors.highlightedColor = highlightColor;
        button.colors = colors;
    }

    private void SetNormalColor()
    {
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = normalColor;
        button.colors = colors;
    }

    private void SetHighlightTextColor()
    {
        if (buttonText != null)
        {
            buttonText.color = highlightTextColor;
        }
        if (tmpText != null)
        {
            tmpText.color = highlightTextColor;
        }
    }

    private void SetNormalTextColor()
    {
        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
        }
        if (tmpText != null)
        {
            tmpText.color = normalTextColor;
        }
    }
}
