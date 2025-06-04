using UnityEngine;
using UnityEngine.UIElements;


public class ButtonManager : MonoBehaviour
{
    [SerializeField] private UIDocument teamUIDocument;
    [SerializeField] private UIDocument selectionUIDocument;

    private VisualElement teamCanvasUI;
    private VisualElement selectionCanvasUI;
    private TeamManager teamManager;

    [SerializeField] private SlotManager slotManager;
    [SerializeField] private CardsManager cardsManager;


    private void Awake()
    {
        teamManager = FindObjectOfType<TeamManager>();

        // Query root of each UIDocument
        var teamRoot = teamUIDocument.rootVisualElement;
        var selectionRoot = selectionUIDocument.rootVisualElement;

        // Get VisualElements from each document
        teamCanvasUI = teamRoot.Q<VisualElement>("Panel");
        selectionCanvasUI = selectionRoot.Q<VisualElement>("Panel");

        // Hook up buttons from selection UI (since they likely exist there)
        var confirmButton = selectionRoot.Q<Button>("ConfirmButton");
        var cancelButton = selectionRoot.Q<Button>("CancelButton");

        confirmButton.RegisterCallback<ClickEvent>(evt => ConfirmButton());
        cancelButton.RegisterCallback<ClickEvent>(evt => CancelButton());
    }

    public void ConfirmButton()
    {
        teamManager.listTeam.Clear();

        foreach (Cards c in teamManager.tempListTeam)
        {
            if (c != null)
                teamManager.listTeam.Add(c);
        }

        // Ensure all slots are correctly updated or cleared
        for (int i = 0; i < SlotManager.teamMaxSize; i++)
        {
            if (i < teamManager.listTeam.Count && teamManager.listTeam[i] != null)
            {
                slotManager.UpdateSlot(i, teamManager.listTeam[i]);
            }
            else
            {
                slotManager.ClearSlot(i);
            }
        }

        teamCanvasUI.style.display = DisplayStyle.Flex;
        selectionCanvasUI.style.display = DisplayStyle.None;
    }

    public void CancelButton()
    {
        teamManager.tempListTeam.Clear();

        foreach (Cards c in teamManager.listTeam)
        {
            teamManager.tempListTeam.Add(c);
        }

        for (int i = 0; i < SlotManager.teamMaxSize; i++)
        {
            if (i < teamManager.tempListTeam.Count && teamManager.tempListTeam[i] != null)
            {
                slotManager.UpdateSlot(i, teamManager.tempListTeam[i]);
            }
            else
            {
                slotManager.ClearSlot(i);
            }
        }


        cardsManager.RefreshCardVisuals();

        teamCanvasUI.style.display = DisplayStyle.Flex;
        selectionCanvasUI.style.display = DisplayStyle.None;
    }
}


