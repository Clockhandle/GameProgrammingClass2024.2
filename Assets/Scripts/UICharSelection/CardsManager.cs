using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardsManager : MonoBehaviour
{
    [SerializeField] private UIDocument root;
    [SerializeField] private TeamManager teamManager;

    public List<Cards> allCards;
    [SerializeField] private SlotManager slotManager;

    private Button lastSelectedButton = null;

    private void OnEnable()
    {
        VisualElement content = root.rootVisualElement.Q("Content");

        int count = Mathf.Min(content.childCount, allCards.Count);

        for (int i = 0; i < count; i++)
        {
            VisualElement cardUI = content[i];
            Cards card = allCards[i];
            SetupCard(cardUI, card);
        }
    }

    private void SetupCard(VisualElement cardUI, Cards card)
    {
        Button button = cardUI.Q<Button>();
        button.style.backgroundImage = new StyleBackground(card.image);
        button.style.unityBackgroundImageTintColor = Color.white;

        bool alreadySelected = teamManager.tempListTeam.Contains(card);


        if (alreadySelected)
        {
            button.style.unityBackgroundImageTintColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        button.clicked += () =>
        {
            bool isSelected = teamManager.tempListTeam.Contains(card);

            if (isSelected)
            {
                // === DESELECT ===
                if (TeamManager.selectionMode == SelectionMode.Single && SlotManager.index != -1)
                {
                    if (SlotManager.index < teamManager.tempListTeam.Count &&
                        teamManager.tempListTeam[SlotManager.index] == card)
                    {
                        teamManager.tempListTeam[SlotManager.index] = null;
                        button.style.unityBackgroundImageTintColor = Color.white;
                    }
                }
                else if (TeamManager.selectionMode == SelectionMode.Multiple)
                {
                    teamManager.tempListTeam.Remove(card);
                    button.style.unityBackgroundImageTintColor = Color.white;
                }

                CharaDetail.cardDetail = null;
                if (lastSelectedButton == button)
                    lastSelectedButton = null;
            }
            else
            {
                // === SELECT ===
                if (TeamManager.selectionMode == SelectionMode.Multiple)
                {
                    if (teamManager.tempListTeam.Count >= SlotManager.teamMaxSize)
                        return;

                    teamManager.tempListTeam.Add(card);
                }
                else // SINGLE mode
                {
                    if (SlotManager.index == -1)
                        return;

                    while (teamManager.tempListTeam.Count <= SlotManager.index)
                        teamManager.tempListTeam.Add(null);

                    teamManager.tempListTeam[SlotManager.index] = card;
                }

                CharaDetail.cardDetail = card;
            }

            RefreshCardVisuals();
         
        };


    }

    public void RefreshCardVisuals()
    {
        VisualElement content = root.rootVisualElement.Q("Content");

        int count = Mathf.Min(content.childCount, allCards.Count);

        for (int i = 0; i < count; i++)
        {
            VisualElement cardUI = content[i];
            Cards card = allCards[i];
            Button button = cardUI.Q<Button>();

            if (teamManager.tempListTeam.Contains(card))
                button.style.unityBackgroundImageTintColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            else
                button.style.unityBackgroundImageTintColor = Color.white;
        }
    }

   



}
