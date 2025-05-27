using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public SaveMenuPopulator smp;
    public StateController stateController;
    public SaveSystem saveSystem;
    public GameObject mainMenuUI;
    public GameObject mainPanel;
    public GameObject loadPanel;
    public Button save1button;
    public Button save2button;
    public Button save3button;
    private List<Button> saveSlotButtons = new List<Button>();

    // helper function
    public void Start()
    {
        saveSlotButtons.Add(save1button);
        saveSlotButtons.Add(save2button);
        saveSlotButtons.Add(save3button);
    }

    // on click Back from the save panel
    public void ToMainPanel()
    {
        mainPanel.SetActive(true);
        loadPanel.SetActive(false);
    }

    // helper function
    public void ToSavePanel()
    {
        mainPanel.SetActive(false);
        loadPanel.SetActive(true);
    }

    // helper function
    public void PopulateSaveButtons()
    {
        for (int i = 0; i < saveSlotButtons.Count; i++)
        {
            Text currentTextComp = saveSlotButtons[i].GetComponentInChildren<Text>();
            string path = Path.Combine(Application.persistentDataPath, $"savefile{i}.json");
            if (!File.Exists(path))
            {
                currentTextComp.text = "Empty";
                continue;
            }

            string jsonString = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonString);
            string currentSaveFileDate = data.calendar.day.ToString();
            var c = data.calendar;
            currentTextComp.text =
                $"Day {c.day:D2}/{c.month:D2}/{c.year}  (Saved at {c.totalDaysPassed}d)";
        }
    }

    // on click "load" from the pause menu
    public void LoadButtonHandler()
    {
        PopulateSaveButtons();
        ToSavePanel();

        for (int i = 0; i < saveSlotButtons.Count; i++)
        {
            int slotIndex = i; // capture
            saveSlotButtons[i].onClick.RemoveAllListeners();
            saveSlotButtons[i].onClick.AddListener(() => LoadSlotHandler(slotIndex));
        }
    }

    // method to bind as a listener for save slot buttons (on load)
    public void LoadSlotHandler(int slot)
    {
        stateController.OpenConfirmationWindow(
            () =>
            {
                saveSystem.LoadGame(slot);
                PopulateSaveButtons();
                ToMainPanel();
                mainMenuUI.SetActive(false);
            },
            "Are you sure you want to load?"
        );
    }
}
