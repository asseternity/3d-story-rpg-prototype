using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonDisabler : MonoBehaviour
{
    public PlayerController pc;
    public Button saveButton;

    void OnEnable()
    {
        saveButton.interactable = !pc.isInBattle;
    }
}
