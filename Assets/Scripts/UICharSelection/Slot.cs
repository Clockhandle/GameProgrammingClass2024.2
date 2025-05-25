using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot
{
    public Cards card;
    public int cardIdx = -1;

    private VisualElement root;
    private Button slotButton;
    private Label cardName;

    public Slot(VisualElement rootElement)
    {
        root = rootElement;
        slotButton = root.Q<Button>("Button");
        cardName = root.Q<Label>("Name");
    }

    public void SetCard(Cards newCard)
    {
        card = newCard;

        if (card == null)
        {
            cardIdx = -1;
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        slotButton.style.backgroundImage = new StyleBackground(card.image);
        slotButton.style.unityBackgroundImageTintColor = Color.white;
        cardName.text = card.charaName;
        cardName.style.display = DisplayStyle.Flex;
    }

    private void Hide()
    {
        slotButton.style.backgroundImage = null;
        cardName.text = "";
        cardName.style.display = DisplayStyle.None;
    }
}
