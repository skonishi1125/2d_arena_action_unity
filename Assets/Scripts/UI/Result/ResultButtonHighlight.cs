using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ResultButtonHighlight : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, // マウス回り
    ISelectHandler, IDeselectHandler // キー操作でセレクトしたときの動作回り
{

    [Header("Targets")]
    [SerializeField] private Image frame; // 枠 or 背景
    [SerializeField] private TMP_Text label; // テキスト

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private bool isSelected;

    private void Awake()
    {
        if (frame == null || label == null)
            Debug.LogWarning($"{name}: frame/label が未設定です", this);

        // UIResultから呼ばれたときにtrue -> この falseが呼ばれて、
        // 選択されていないように見えてしまうのでコメントアウトした
        //Apply(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Apply(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Debug.Log("deseclect");
        Apply(false);
    }

    // マウス選択時
    public void OnPointerEnter(PointerEventData eventData)
    {
        Apply(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("exit");
        Apply(false);
    }


    // 選ばれたときボタンフレーム、テキストの色を変える
    public void Apply(bool highlight)
    {
        //Debug.Log("Apply: " + highlight);
        if (frame != null) frame.color = highlight ? selectedColor : normalColor;
        if (label != null) label.color = highlight ? selectedColor : normalColor;
    }

}
