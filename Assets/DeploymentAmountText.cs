using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



    public class DeploymentAmountText : MonoBehaviour
    {
       private UnitIconData unitIconData;
         private TextMeshProUGUI maxDeployText;
    private UnitDataSO data;



        private void Awake()
        {
            unitIconData = GetComponentInParent<UnitIconData>();
            maxDeployText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
     
            UpdateDeployCountText();
        }

    public void UpdateDeployCountText()
    {
        Unit unitComponent = unitIconData.unitPrefab.GetComponent<Unit>();
        data = unitComponent.unitDataSO;

        if (data == null || unitComponent == null) return;

        int max = data.maxNumberOfDeployments;
        int current = DeploymentManager.Instance.GetCurrentDeploymentCount(unitIconData.unitPrefab);
        Debug.Log($"{current}");
        int remaining = Mathf.Max(0, max - current); 

        maxDeployText.text = remaining.ToString();
    }
}

