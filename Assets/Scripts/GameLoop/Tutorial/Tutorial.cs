using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial", menuName = "Tutorial/Tutorial")]
public class Tutorial : ScriptableObject
{
    public List<TutorialPage> pages;
}

