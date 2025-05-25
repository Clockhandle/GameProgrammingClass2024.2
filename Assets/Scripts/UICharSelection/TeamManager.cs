using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SelectionMode { Single, Multiple }
public class TeamManager : MonoBehaviour
{
    public List<Cards> listTeam = new List<Cards>();
    public List<Cards> tempListTeam = new List<Cards>();
    public static SelectionMode selectionMode;
}
