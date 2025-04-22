// [v] make a 3d scene with capsules and walls like in the 3d running demo
// [v] move things from the 3d running around demo
// [v] import / install articy importer
// [v] move things from the articy demo
// [v] combine them gracefully in one thing
// [v] make dialogues appear by npc approaching
// [v] save to github
// [v] block movement when dialogue is active

// dialogues:
// [v] learn articy fully
// [v] final tutorial on articy importer: https://www.articy.com/en/importer-for-unity-tutorial-l7/
// [v] learn articy: seen / unseen - Red Hub 1 - only once
// [v] learn articy: add sprites to dialogues
// [v] learn articy: presentation view
// [v] articy naming - and using vars in dialogues, like for pronouns
// [_] dialogue UI prettying - layers, shadows, make models appear
// [_] some visual indicator if there's a dialogue available
// [_] calendar system
// [_] some kind of story manager that determines which things are available and which indicators to show
// [_] some kind of quests system that envelops everything, including the articy things

// persona fighting mechanics:
// [_] get transported to "fighting realm scene", then mechanics activate
// [_] camera angles, particle effects

// BRAINSTORMING STRUCTURE - how to structure the logic? how to structure the quests and activities? how do initiate fights and store combat stats?
// ANSWER: keep global variables (plot) in articy, but keep the overall story structure in Unity. Build quests as scriptable objects.
// Okay, then next question - how do I call things from unity after an Articy conversation, then return to Articy?
// I will probably have to do different Articy dialogues for before and after the thing
// So then, I need some kind of story flow manager which will:
// keep track of the current date in the schedule
// trigger articy dialogues and block movement and fighting when the dialogue is active
// trigger the next part of the story after the dialogue is done, like writing in your name, or triggering a battle
// then trigger the next Articy dialogue after the battle is done, or the name is written
// Okay... then what are quests?
// The quest system will work like this:
// - first few days, it's just straight cutscenes with no end
// - then, the player will be free to choose a morning and an evening activity, with a lunch / class cutscene in between
// - but the player has to choose to do the quest AS an activity
// - so then the quest system will be an array of objects that will have:
// "description", "type", "bool re reach stage completed or not", "bool re quest active or not" and maybe refs to articy dialogues
// so what does that mean then?
// the entire game will be structured as a series of quests overlaid on top of a calendar
// quests will have: booleans for each stage (completed or not), a title, a description, a type (main, relationship)
// hang on. what's the difference between a quest and an activity then?
// - an activity is a series of ten articy dialogues (like persona 5's confidantes)
// - a quest is a set of data that can include listening to an activity
// - the story manager will:
// (1) populate the 3D environment with available activities based on the date
// (2) keep track of the available quests and their stages, progressing them as the player completes the tasks
// [_] this means that I need a function that will take a quest and progress it to the next stage
// (3) trigger the next whatever when the player completes an articy dialogue, a battle or anything else
// [_] think - how to accomplish this? in articy? or in Unity?

// TECHNICAL DESIGN:
// Unity listens to Articy's state (e.g., variable or dialogue complete)
// You can wait for:
// A certain Articy variable to change
// The end of an Articy dialogue/playthrough

// Example:
// csharp
// void OnDialogueEnded()
// {
//     var state = ArticyDatabase.DefaultGlobalVariables;
//     if (state.quest_intro.finished)
//         QuestManager.Instance.Progress("main_story");
// }
