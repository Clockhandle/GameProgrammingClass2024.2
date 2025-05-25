using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement content;

    public Cards[] cards;


    void OnEnable()
    {
        root = uiDocument.rootVisualElement;
        content = root.Q<VisualElement>("Content"); // "#Content" in UXML

     

        int index = 0;

        foreach (Cards card in cards)
        {
            if (!card.unlocked || index >= content.childCount)
                continue;

            var button = content.ElementAt(index) as Button;
            if (button == null) continue;

            // Set the button background image
            button.style.backgroundImage = new StyleBackground(card.image);

            // Get the label child and set the name
            var label = button.Q<Label>();
            if (label != null)
                label.text = card.charaName;

            index++;
        }
    }
}