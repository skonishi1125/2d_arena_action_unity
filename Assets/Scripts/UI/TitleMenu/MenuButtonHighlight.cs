using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MenuButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Targets")]
    [SerializeField] private Image frame;     // 枠 or 背景
    [SerializeField] private TMP_Text label;  // テキスト

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private bool _selected;

    private void Awake()
    {
        Apply(false);
    }
    public void OnSelect(BaseEventData eventData)
    {
        _selected = true;
        Apply(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _selected = false;
        Apply(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Apply(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Apply(_selected);
    }

    private void Apply(bool highlight)
    {
        if (frame != null) frame.color = highlight ? selectedColor : normalColor;
        if (label != null) label.color = highlight ? selectedColor : normalColor;
    }



}
