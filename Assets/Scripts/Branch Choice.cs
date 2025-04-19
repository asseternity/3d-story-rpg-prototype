using System.Collections;
using System.Collections.Generic;
using Articy.Unity;
using Articy.Unity.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class BranchChoice : MonoBehaviour
{
    private Branch branch;
    private ArticyFlowPlayer flowPlayer;

    [SerializeField]
    Text buttonText;

    // This method will be called each time a button
    // is created to represent a single branch in the flow
    public void AssignBranch(ArticyFlowPlayer articyFlowPlayer, Branch aBranch)
    {
        branch = aBranch;
        flowPlayer = articyFlowPlayer;
        IFlowObject target = aBranch.Target;
        buttonText.text = string.Empty;

        // check if we have any menu text that we ould display
        var objectWithMenuText = target as IObjectWithMenuText;
        if (objectWithMenuText != null)
        {
            buttonText.text = objectWithMenuText.MenuText;
        }

        // to cover the case if there's no menu text
        if (string.IsNullOrEmpty(buttonText.text))
        {
            buttonText.text = ">>>";
        }
    }

    // what happens when a button is clicked
    public void OnBranchSelected()
    {
        flowPlayer.Play(branch);
    }
}
