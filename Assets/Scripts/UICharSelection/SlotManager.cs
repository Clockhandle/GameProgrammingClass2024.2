using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotManager : MonoBehaviour
{
    private VisualElement content; // #Content
    private Button[] slotButtons;
    private Slot[] _slot;
    private TeamManager teamManager;
    private CharacterManager characterManager;

    public static int teamMaxSize;
    public static int index;

    [Header("UIDocument")]
    [SerializeField] private UIDocument uiDocument;
    private VisualElement teamCanvas;
    private Button multiSelectButton;
    private Button clearButton;
    private Button startButton;

    [Header("Selection UI")]
    [SerializeField] private UIDocument selectionUIDocument;
    private VisualElement selectionCanvas;

    private CardsManager cardsManager;

    private void Awake()
    {
        cardsManager = FindObjectOfType<CardsManager>();
        teamManager = FindObjectOfType<TeamManager>();
        characterManager = FindObjectOfType<CharacterManager>();

        var root = uiDocument.rootVisualElement;
        var selectionRoot = selectionUIDocument.rootVisualElement;

        content = root.Q<VisualElement>("Content");
        teamCanvas = root.Q<VisualElement>("Panel");
        selectionCanvas = selectionRoot.Q<VisualElement>("Panel");

        multiSelectButton = root.Q<Button>("MultiSelectButton");
        clearButton = root.Q<Button>("ClearButton");
        startButton  = root.Q<Button>("PlayButton");


        int count = content.childCount;
        teamMaxSize = count;

        slotButtons = new Button[count];
        _slot = new Slot[count];

        for (int i = 0; i < count; i++)
        {
            var button = content.ElementAt(i) as Button;
            slotButtons[i] = button;

            Slot slot = new Slot(button); // Button is the root VisualElement for the Slot
            _slot[i] = slot;
            button.userData = slot;

            int idx = i; // capture index for closure
            button.clicked += () => SelectChara(idx);
        }

        multiSelectButton.clicked += MultiSelectButton;
        clearButton.clicked += ClearButton;
        //startButton.clicked +=
      
    }

    private void OnEnable()
    {
        foreach (var card in characterManager.cards)
        {
            card.inTeam = false;
        }

        if (teamManager.listTeam.Count > 0)
        {
            for (int i = 0; i < teamMaxSize; i++)
            {
                _slot[i].card = null;
            }

            int idx = 0;
            foreach (var c in teamManager.listTeam)
            {
                _slot[idx].cardIdx = teamManager.listTeam.IndexOf(c);
                _slot[idx].SetCard(c);
                idx++;

                var cardInManager = System.Array.Find(characterManager.cards, card => card == c);
                if (cardInManager != null) cardInManager.inTeam = true;
            }
        }
    }

    public void UpdateSlot(int idx, Cards card)
    {
        if (idx < 0 || idx >= _slot.Length)
        {
            Debug.LogWarning($"UpdateSlot: Invalid index {idx}. _slot length is {_slot.Length}.");
            return;
        }

        _slot[idx].SetCard(card);
    }

    private void SelectChara(int idx)
    {
        TeamManager.selectionMode = SelectionMode.Single;
        index = idx;

        teamManager.tempListTeam.Clear();
        foreach (Cards c in teamManager.listTeam)
        {
            teamManager.tempListTeam.Add(c);
        }

        selectionCanvas.style.display = DisplayStyle.Flex;
        teamCanvas.style.display = DisplayStyle.None;
    }

    private void MultiSelectButton()
    {
        TeamManager.selectionMode = SelectionMode.Multiple;
        index = 0;

        teamManager.tempListTeam.Clear();
        foreach (Cards c in teamManager.listTeam)
        {
            teamManager.tempListTeam.Add(c);
        }

        selectionCanvas.style.display = DisplayStyle.Flex;
        teamCanvas.style.display = DisplayStyle.None;
    }

    public void ClearButton()
    {
        teamManager.listTeam.Clear();
        teamManager.tempListTeam.Clear();

        foreach (var s in _slot)
        {
            s.cardIdx = -1;
            s.SetCard(null); // triggers Hide()
        }
        cardsManager?.RefreshCardVisuals();
    }

    public void ClearSlot(int idx)
    {
        if (idx >= 0 && idx < _slot.Length)
        {
            _slot[idx].cardIdx = -1;
            _slot[idx].card = null;

            var button = slotButtons[idx];
            button.style.backgroundImage = null;
            button.style.unityBackgroundImageTintColor = Color.white;
        }
    }
}
