using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCooldownManager : MonoBehaviour
{
    public static SkillCooldownManager Instance;

    private void Awake()
    {
     
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartCooldown(Button button, float delay)
    {
        StartCoroutine(ReactivateButtonAfterDelay(button, delay));
    }

    private IEnumerator ReactivateButtonAfterDelay(Button button, float delay)
    {
        yield return new WaitForSeconds(delay);
       
        if(button!= null)
        {
            button.interactable = true;
        }
           
        
    }
}
