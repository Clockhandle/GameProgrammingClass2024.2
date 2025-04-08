using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitIconData : MonoBehaviour
{
    // Assign the corresponding Unit Prefab in the Inspector for each UI icon
    [Tooltip("The Unit Prefab this UI icon represents.")]
    public GameObject unitPrefab;

    // You could add references to UI elements for cooldown overlays, count text etc. later
    // public Image cooldownOverlay;
    // public TMPro.TextMeshProUGU countText;
}
