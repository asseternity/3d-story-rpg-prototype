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
// [v] some kind of story manager that determines which things are available and which indicators to show
// [v] some kind of quests system that envelops everything, including the articy things
// [v] big game loop - cutscenes following fights following cutscenes:
// [v] - queues SO
// [v] - tie a queue to NPC conversations instead of direct Articy Dialogue
// [v] - activities SO
// [v] - main story state controller
// [v] - calendar system UI and a bed to progress the day
// [v] - adjust PlayerController to also alternatively accept an Activity instead of a Queue
// [v] - populate the world with with NPC holding Activities
// [v] - adjust the main story state controller to disable some NPCs on certain days!
// [_] - dialogue UI prettying - layers, shadows, make models appear
// [_] - quests SO
// [_] - saving / loading
// [_] - main menu
// [_] - pause menu
// [_] - settings menu
// [_] - game over
// [_] - some visual indicator if there's a dialogue available

// persona fighting mechanics:
// [v] build the story manager script with 2 functions: startDialogue (articyObject, nextThing) and startBattle (battleID, nextThing)
// [v] create a scriptable object for battle participant
// [v] create a scriptable object for battle data that will include participants and battleID for lookup
// [v] create a function that will take battleID as an argument to be called from story manager to start the battle
// [v] get transported to "fighting realm scene", spawn enemies, then mechanics activate
// [v] meat of the battle --- this function will take away movement controls, change camera, and start the battle mechanics
// [v] show health bars in prefabs for all participants
// [v] make enemies clickable and outline the currently clicked enemy
// [v] make the buttons feel good and reactive, making it visible which one is clicked
// [v] add submenus, spells and abilities
// [v] using stats and abilities for all participants and where to store them
// [_] (1) fix that battle SOs change after testing
// [_] (2) pretty the fighting system:
// [v] - fluidity
// [v] - animations
// [v] - enemies go one after another not all at once
// [v] - one animation for moves with multiple targets
// [_] - damage numbers and MP costs
// [_] - camera angles
// [_] - particle effects
// [_] - ui and turn order
// [_] - add items and consumables

// --- GAME STRUCTURE DOCUMENTATION ---

// The story elements are Queue, Activity and Quest.
// - Queue is a sequence of events (dialogue, battle, name entry) that launch automatically one after another until the queue is empty.
// - Activity is a branching series of ten Queues, like a Persona 5 confidante.
// Completing a level may complete a Quest, affect relationship levels/stats/resources, etc.
// - Quest is a Scriptable Object that includes: triggers, description, type, bool re reach stage completed or not, bool re quest active or not.

// The skeleton of the game is the StartSectionFrom functions. They operate Queues in a non-stop way.
// The game will be structured as so: for the first few days, the initial Queue starts and does not let go.
// Then, the player is placed in the 3D environment, will be provided with Quests and will be able to run around and choose Activities.
// During this free time, the player will be able to do a morning and an evening Acvitity, with a lunch cutscene in between.
// In essence, the entire game is structured as a series of quests overlaid on top of a calendar.

// The story manager needs to:
// (1) populate the 3D environment with available activities based on the date
// (2) keep track of the available quests and their stages, progressing them as the player completes the tasks
// [_] this means that I need a function that will take a quest and progress it to the next stage
// (3) trigger the next whatever when the player completes an articy dialogue, a battle or anything else
// [v] this will use the StartSectionFrom functions
