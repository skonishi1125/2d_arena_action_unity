using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Targets")]
    [SerializeField] private Image frame; // 枠 or 背景
    [SerializeField] private TMP_Text label; // テキスト

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private bool isSelected;

    // 選択、決定のSEは UITitleMenu に任せた

    private void Awake()
    {
        Apply(false);
    }
    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        Apply(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        Apply(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Apply(true);
        Debug.Log("onpointer.enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Apply(isSelected);
    }

    // 選ばれたときボタンフレーム、テキストの色を変える
    public void Apply(bool highlight)
    {
        if (frame != null) frame.color = highlight ? selectedColor : normalColor;
        if (label != null) label.color = highlight ? selectedColor : normalColor;
    }



}
