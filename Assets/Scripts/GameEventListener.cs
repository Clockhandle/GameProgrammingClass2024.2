using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private EventChannelSO gameEvent;

    private void OnEnable() => gameEvent.RegisterListener(this);
    private void OnDisable() => gameEvent.UnregisterListener(this);

    public void OnEventRaised()
    {
        Debug.Log("Prefab received the event!");
    }
}
