using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Roles { Archer, Infantry, Medic, Scout, Workers }
public enum Type { Range, Melee }

[CreateAssetMenu(fileName = "New Card", menuName = "Character Card")]
public class Cards : ScriptableObject
{
    public string charaName;
    public Sprite image;
    public int atk;
    public int hp;
    public int def;
    public Roles role;
    public Type type;
    public GameObject character;
    public bool unlocked;
    public bool inTeam;
}