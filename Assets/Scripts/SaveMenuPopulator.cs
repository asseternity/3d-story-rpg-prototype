using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuPopulator : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject pausePanel;
    public GameObject savePanel;
    public Button save1button;
    public Button save2button;
    public Button save3button;
    private List<Button> saveSlotButtons = new List<Button>();
    public SaveSystem saveSystem;
    public StateController stateController;

    // helper function
    public void Start()
    {
        saveSlotButtons.Add(save1button);
        saveSlotButtons.Add(save2button);
        saveSlotButtons.Add(save3button);
    }

    // on click Back from the save panel
    public void ToPausePanel()
    {
        pausePanel.SetActive(true);
        savePanel.SetActive(false);
    }

    // helper function
    public void ToSavePanel()
    {
        pausePanel.SetActive(false);
        savePanel.SetActive(true);
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

    // on click "save" from the pause menu
    public void SaveButtonHandler()
    {
        PopulateSaveButtons();
        ToSavePanel();

        for (int i = 0; i < saveSlotButtons.Count; i++)
        {
            int slotIndex = i; // capture
            saveSlotButtons[i].onClick.RemoveAllListeners();
            saveSlotButtons[i].onClick.AddListener(() => SaveSlotHandler(slotIndex));
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

    // method to bind as a listener for save slot buttons
    public void SaveSlotHandler(int slot)
    {
        stateController.OpenConfirmationWindow(
            () =>
            {
                saveSystem.SaveGame(slot);
                PopulateSaveButtons();
            },
            "Are you sure you want to save?"
        );
    }

    // method to bind as a listener for save slot buttons (on load)
    public void LoadSlotHandler(int slot)
    {
        stateController.OpenConfirmationWindow(
            () =>
            {
                saveSystem.LoadGame(slot);
                PopulateSaveButtons();
                ToPausePanel();
                pauseUI.SetActive(false);
            },
            "Are you sure you want to load?"
        );
    }
}
