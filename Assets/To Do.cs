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
// [_] (4) dialogue UI prettying - layers, shadows, make models appear
// [_] (5) calendar system
// [_] (5) activities as per below
// [_] (5) quests as per below
// [_] saving / loading
// [_] main menu
// [_] pause menu
// [_] settings menu
// [_] game over
// [_] some visual indicator if there's a dialogue available

// persona fighting mechanics:
// [v] build the story manager script with 2 functions: startDialogue (articyObject, nextThing) and startBattle (battleID, nextThing)
// [v] create a scriptable object for battle participant
// [v] create a scriptable object for battle data that will include participants and battleID for lookup
// [v] create a function that will take battleID as an argument to be called from story manager to start the battle
// [v] get transported to "fighting realm scene", spawn enemies, then mechanics activate
// [v] meat of the battle --- this function will take away movement controls, change camera, and start the battle mechanics
// [v] show health bars in prefabs for all participants
// [_] (0) make enemies clickable and outline the currently clicked enemy
// [_] (1) working battle loop, including: animations, health bars, damage numbers, and turn order
// [_] (2) add spells and abilities
// [_] (3) add items and consumables
// [_] using stats and abilities for all participants and where to store them
// [_] camera angles, particle effects
// [_] build the quest and activity scriptable objects

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
// Story manager functions: start dialogue (ArticyObject), start battle (battleID)
// GOT IT! both functions there have be a second argument - a queue of the next things to do after the dialogue or the battle is finished (if any)
// Each activity - when activated - will trigger a dialogue and include that second argument in the function call
