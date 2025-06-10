using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharaDetail : MonoBehaviour
{
    public static Cards cardDetail;

    private Label charaNameLabel, atkLabel, hpLabel, defLabel, roleLabel, typeLabel, Skill;
    private VisualElement detailContainer;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get references to labels by their name
        charaNameLabel = root.Q<Label>("CharaName");
        atkLabel = root.Q<Label>("Atk");
        hpLabel = root.Q<Label>("Hp");
        defLabel = root.Q<Label>("Def");
        roleLabel = root.Q<Label>("Role");
        typeLabel = root.Q<Label>("Type");
        Skill = root.Q<Label>("Skill");

        detailContainer = root.Q<VisualElement>("InfoPanel");

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (cardDetail == null)
        {
            // Hide the whole container or labels
            detailContainer.style.display = DisplayStyle.None;
        }
        else
        {
            detailContainer.style.display = DisplayStyle.Flex;

            charaNameLabel.text = "NAME : " + cardDetail.name;
            atkLabel.text = "ATK : " + cardDetail.atk;
            hpLabel.text = "HP : " + cardDetail.hp;
            defLabel.text = "DEF : " + cardDetail.def;
            roleLabel.text = "ROLE : " + cardDetail.role.ToString();
            typeLabel.text = "TYPE : " + cardDetail.type.ToString();
            Skill.text = "SKILL: " + cardDetail.Skill.ToString();
        }
    }
}
