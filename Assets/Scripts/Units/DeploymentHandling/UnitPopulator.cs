using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;


[System.Serializable]
public class CharacterPrefabMapping
{
    public Cards cardData;
    public GameObject characterPrefab;
}


public class UnitPopulator : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The single, generic prefab for a Unit Icon that has the UnitIconData script on it.")]
    public GameObject genericUnitIconPrefab;

    [Tooltip("The container object that DragToScreenManager watches for icons.")]
    public Transform unitIconContainer;

    [Header("Character Prefab Master Map")]
    [Tooltip("Create a mapping for every character. Link the Card ScriptableObject directly to its corresponding 3D character prefab.")]
    public List<CharacterPrefabMapping> characterMappings;

    void Start()
    {
        if (!ValidateSetup()) return;

        List<Cards> unitList = ProgressManager.Instance.selectedCards;

        PopulateHandWithCards(unitList);

        if (DragToScreenManager.Instance != null)
        {
            DragToScreenManager.Instance.RefreshIconList();
        }
    }

    private void PopulateHandWithCards(List<Cards> team)
    {
        // Now, loop through each character card the player selected.
        foreach (Cards cardData in team)
        {
            // --- This is the new, improved logic ---

            // A. Find the correct 3D character prefab from our mapping list.
            //    This directly compares the ScriptableObject assets, which is fast and safe.
            GameObject characterPrefabToAssign = FindCharacterPrefabByCard(cardData);

            if (characterPrefabToAssign == null)
            {
                Debug.LogWarning($"Could not find a mapping for the card '{cardData.charaName}' in the 'characterMappings' list. Skipping this card.", this);
                continue; // Skip to the next card in the team
            }

            // B. Instantiate the generic UI icon prefab and place it in the hand container.
            GameObject uiIconInstance = Instantiate(genericUnitIconPrefab, unitIconContainer);
            UnitIconData iconDataScript = uiIconInstance.GetComponent<UnitIconData>();

            iconDataScript.unitPrefab = characterPrefabToAssign;

            // Set the icon sprite using the new method
            if (cardData.image != null)
            {
                iconDataScript.SetIconSprite(cardData.image);
            }

            SetupEventTrigger(uiIconInstance);
        }
    }

    public void InstantiateUnitIconForPrefab(GameObject characterPrefabToAssign)
    {
        // Find the cardData for this prefab (if possible)
        Cards cardData = null;
        foreach (var mapping in characterMappings)
        {
            if (mapping.characterPrefab == characterPrefabToAssign)
            {
                cardData = mapping.cardData;
                break;
            }
        }

        // Instantiate the generic UI icon prefab and place it in the hand container (deploymentList panel)
        GameObject uiIconInstance = Instantiate(genericUnitIconPrefab, unitIconContainer);
        UnitIconData iconDataScript = uiIconInstance.GetComponent<UnitIconData>();

        iconDataScript.unitPrefab = characterPrefabToAssign;

        // Set the icon sprite using the new method
        Sprite iconSprite = (cardData != null && cardData.image != null)
            ? cardData.image
            : characterPrefabToAssign.GetComponent<Unit>()?.unitDataSO?.icon;
        iconDataScript.SetIconSprite(iconSprite);

        iconDataScript.StoreInitialState();
        SetupEventTrigger(uiIconInstance);
    }

    private GameObject FindCharacterPrefabByCard(Cards cardToFind)
    {
        CharacterPrefabMapping mapping = characterMappings.FirstOrDefault(m => m.cardData == cardToFind);

        return mapping?.characterPrefab;
    }

    private bool ValidateSetup()
    {
        if (ProgressManager.Instance == null)
        {
            Debug.LogError("ProgressManager not found! Cannot load team.", this);
            return false;
        }
        if (genericUnitIconPrefab == null || unitIconContainer == null)
        {
            Debug.LogError("UnitHandLoader is missing references to its UI prefab or container.", this);
            return false;
        }
        if (characterMappings == null || characterMappings.Count == 0)
        {
            Debug.LogError("The 'characterMappings' list is empty! Set up your card-to-prefab links in the Inspector.", this);
            return false;
        }
        return true;
    }

    private void SetupEventTrigger(GameObject uiIconInstance)
    {
        EventTrigger trigger = uiIconInstance.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            // If the prefab doesn't have one, add it.
            trigger = uiIconInstance.AddComponent<EventTrigger>();
        }

        // 2. Clear any "dead" listeners that might have come from the prefab template.
        trigger.triggers.Clear();

        // 3. Create an entry for the "Begin Drag" event.
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        // 4. Tell it to call the HandleBeginDrag method on our live DragToScreenManager instance.
        beginDragEntry.callback.AddListener((eventData) => { DragToScreenManager.Instance.HandleBeginDrag(eventData); });
        // 5. Add this new rule to the trigger's list.
        trigger.triggers.Add(beginDragEntry);

        // --- Repeat for the other events ---

        // Create the "Drag" event entry
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((eventData) => { DragToScreenManager.Instance.HandleDrag(eventData); });
        trigger.triggers.Add(dragEntry);

        // Create the "End Drag" event entry
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener((eventData) => { DragToScreenManager.Instance.HandleEndDrag(eventData); });
        trigger.triggers.Add(endDragEntry);
    }
}