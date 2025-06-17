using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySpecial : StateMachineBehaviour
{
    // Override the OnStateEnter method to play the sound when the state is entered
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("Skill Special SFX");
        }
    }
}
