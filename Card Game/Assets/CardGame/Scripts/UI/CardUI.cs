using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _cardNameText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _abilityType;

    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _selectBoundary;
    private CardData _cardData;
    private bool _isSelected = false;
    private bool _interactable = false;
    public CardData CardData => _cardData;
    public bool IsSelected => _isSelected;


    public void Initialize(CardData data,bool interactable = true)
    {
        _cardData = data;

        _cardNameText.text = data.name;
        _costText.text = data.cost.ToString();
        _powerText.text = data.power.ToString();
        _bgImage.color = data.bgColor;
        _abilityType.SetText($"{data.ability.type} + {data.ability.value}");
        _interactable = interactable;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!_interactable) return;
        TurnManager.Instance.ToggleCardSelection(this);

    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _selectBoundary.gameObject.SetActive(_isSelected);
    }
}