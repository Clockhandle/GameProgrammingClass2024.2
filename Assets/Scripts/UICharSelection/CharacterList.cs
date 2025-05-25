using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterList : MonoBehaviour
{
    Cards[] cards;
    CharacterManager characterManager;
    TeamManager teamManager;

    private void Awake()
    {
        teamManager = FindObjectOfType<TeamManager>();
        characterManager = FindObjectOfType<CharacterManager>();
    }

    //private void OnEnable() => SortingCards();
    //private void OnDisable() => SortingCards();

    //void SortingCards()
    //{
    //    cards = characterManager.listCards
    //        .OrderBy(_card => _card.inTeam ? 0 : 1)
    //        .ThenBy(_card => teamManager.listTeam.IndexOf(_card))
    //        .ToArray();

    //    int idx = 0;

    //    if (TeamManager.selectionMode == SelectionMode.Multiple)
    //    {
    //        foreach (Cards c in cards)
    //        {
    //            if (idx < characterManager.slotList.Count)
    //            {
    //                characterManager.slotList[idx].SetCard(c);
    //                idx++;
    //            }
    //        }
    //    }
    //    else // SelectionMode.Single
    //    {
    //        foreach (Cards c in cards)
    //        {
    //            if (idx < characterManager.slotList.Count)
    //            {
    //                int teamIdx = teamManager.listTeam.IndexOf(c);

    //                if (teamIdx == -1 || teamIdx == SlotManager.index)
    //                {
    //                    // Show this card if it's not in team or it's the one currently assigned to this slot
    //                    characterManager.slotList[idx].SetCard(c);
    //                }
    //                else
    //                {
    //                    // Hide this card visually
    //                    characterManager.slotList[idx].SetCard(null);
    //                }

    //                idx++;
    //            }
    //        }
    //    }

    //    // Hide any extra slots not used
    //    for (int i = idx; i < characterManager.slotList.Count; i++)
    //    {
    //        characterManager.slotList[i].SetCard(null);
    //    }
    //}
}