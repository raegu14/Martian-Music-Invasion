// -------------------------------------------------------------------------------------------------
// GBL_Interface.cs
// Project: GBLXAPI-Unity
// Created: 2018/07/06
// Copyright 2018 Dig-It! Games, LLC. All rights reserved.
// This code is licensed under the MIT License (see LICENSE.txt for details)
// -------------------------------------------------------------------------------------------------
using UnityEngine;

// required for GBLXAPI
using DIG.GBLXAPI;
using TinCan;
using System;
using System.Collections.Generic;
using System.Text; // required for StringBuilder()

using Newtonsoft.Json.Linq; // for extensions

// --------------------------------------------------------------------------------------
// --------------------------------------------------------------------------------------
public static class GBL_Interface {

		public enum durationSlots
	{
		Application,
		Game,
		Tutorial,
		Level,
		TutorialDialog
	};

    // Fill in these fields for GBLxAPI setup.
	public static string lrsURL = "https://lrs.gblxapi.org/data/xAPI"; 				// endpoint
	public static string lrsUser = "2585bae065285208e5c5d6cf8a4c89f125d735f4";  		// key
	public static string lrsPassword = "c223900d06062192ce613ebb8c1b9c9e38bf7862";
	public static string standardsConfigDefault = "data/GBLxAPI_Vocab_Default";
	public static string standardsConfigUser = "data/GBLxAPI_Vocab_User";
	public static string gameURI = "http://martianmusicinvasion.com/game";
	public static string gameName = "Martian Music Invasion";
	public static string companyURI = "http://martianmusicinvasion.com/";
	public static string userUUID = "f1cd58aa-ad22-49e5-8567-d59d97d3b209";

    // ------------------------------------------------------------------------
	// Sample Gameplay GBLxAPI Triggers
	// ------------------------------------------------------------------------
	/*
	Here is where you will put functions to be called whenever you want to send a GBLxAPI statement.
	 */

	public static void SendIncorrectNoteMatched(uint levelNum, string noteHolding, string noteTarget) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("answered");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNum + "/matching/" + noteTarget, "question", "Level " + levelNum + " Matching Note " + noteTarget);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, false, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		TinCan.Extensions contextExtensions = new TinCan.Extensions();
		string noteHoldingURI = GBLXAPI.Instance.GetVocabURI("extension", "noteHolding");
		contextExtensions.Add(noteHoldingURI, noteHolding);

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList, null, null, contextExtensions);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendCorrectNoteMatched(uint levelNum, string noteHolding) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("answered");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNum + "/matching/" + noteHolding, "question", "Level " + levelNum + " Matching Note " + noteHolding);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendFreeExplorationNotePlaced(string noteName, string notePos) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("released");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration/note/" + noteName + "/" + notePos, "item", "Free Exploration Note " + noteName + " at " + notePos);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendFreeExplorationNotePlaced(string noteName, string notePos) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("released");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration/note/" + noteName + "/" + notePos, "item", "Free Exploration Note " + noteName + " at " + notePos);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendFreeExplorationNotePickedUp(string noteName, string notePos) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("interacted");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration/note/" + noteName + "/" + notePos, "item", "Free Exploration Note " + noteName + " at " + notePos);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendFreeExplorationQuitConfirmed() {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("interacted");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration/quitconfirmbutton", "menu", "Free Exploration Quit Dialog 'Yes' Button");

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendFreeExplorationQuitDenied() {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("interacted");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration/quitdenybutton", "menu", "Free Exploration Quit Dialog 'No' Button");

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendFreeExplorationQuitAttempted() {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("interacted");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration/quitbutton", "menu", "Free Exploration Quit Button");

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendStaffPlayed() {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("interacted");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration/playbutton", "menu", "Free Exploration Play Button");

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendStaffCleared() {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("interacted");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration/clearbutton", "menu", "Free Exploration Clear Button");

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendFreeExplorationFinished() {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("completed");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration", "level", "Free Exploration");

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}

	public static void SendFreeExplorationStarted() {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("started");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/free_exploration", "level", "Free Exploration");

		GBLXAPI.Instance.ResetDurationSlot((int)durationSlots.Level);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, null, statementContext);
	}

	public static void SendRightCircleClicked(uint levelNumber, int step) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("answered");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber + "/step/" + step, "question", "Level " + levelNumber + " Step " + step);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		// context extension for # of attempts
		// context extension for lives left

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendWrongCircleClicked(uint levelNumber, int step) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("answered");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber + "/step/" + step, "question", "Level " + levelNumber + " Step " + step);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, false, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		// context extension for # of attempts
		// context extension for lives left

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}

	public static void SendLevelFailed(uint levelNumber) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("attempted");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber, "level", "Level " + levelNumber);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, false, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		// context extension for # of attempts
		// context extension for lives left

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendLevelCompleted(uint levelNumber, int levelAttempts, int livesLeft) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("completed");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber, "level", "Level " + levelNumber);

		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		// context extension for # of attempts
		// context extension for lives left

		string attemptsURI = GBLXAPI.Instance.GetVocabURI("extension", "levelattempts");
		string livesleftURI = GBLXAPI.Instance.GetVocabURI("extension", "livesleft");

		TinCan.Extensions contextExtensions = new TinCan.Extensions();
		// TODO find a way to track level attempts
		// contextExtensions.Add(attemptsURI, levelAttempts.ToString());
		contextExtensions.Add(livesleftURI, livesLeft.ToString());

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList, null, null, contextExtensions);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}

	public static void SendLevelStarted(uint levelNumber) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("started");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber, "level", "Level " + levelNumber);

		GBLXAPI.Instance.ResetDurationSlot((int)durationSlots.Level);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		//List<Activity> groupingList = new List<Activity>();
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, null, statementContext);
	}

	public static void SendHintRequested(uint levelNumber) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("interacted");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber + "/hint", "non-player-character", "Level " + levelNumber + " Hint");

		// Get time since level started
		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.Level);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber, "level", "Level " + levelNumber));

		List<Activity> groupingList = new List<Activity>();
		groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList, groupingList);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	
	public static void ResetTutorialDialogDurationSlot() {
		GBLXAPI.Instance.ResetDurationSlot((int)durationSlots.TutorialDialog);
	}
	public static void SendTutorialDialogSeen(uint levelNumber, int tutorialIndex, bool audioFinished) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("experienced");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber + "/tutorial/" + tutorialIndex, "page", "Level " + levelNumber + " Tutorial Dialogue " + tutorialIndex);

		// Get duration and immediately reset for next tutorial dialog
		float duration = GBLXAPI.Instance.GetDurationSlot((int)durationSlots.TutorialDialog);
		Result statementResult = GBLXAPI.Instance.CreateResultStatement(true, true, duration);
		GBLXAPI.Instance.ResetDurationSlot((int)durationSlots.TutorialDialog);

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/level/" + levelNumber, "level", "Level " + levelNumber));

		List<Activity> groupingList = new List<Activity>();
		groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));
		//groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		string audioFinishedURI = GBLXAPI.Instance.GetVocabURI("extension", "cutsceneaudiofinished");

		TinCan.Extensions contextExtensions = new TinCan.Extensions();
		contextExtensions.Add(audioFinishedURI, audioFinished.ToString());

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList, groupingList, null, contextExtensions);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendCutsceneChanged(string sceneName, bool audioFinished) {
		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("experienced");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game/cutscenes/" + sceneName, "page", sceneName);
		Result statementResult = null;

		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/game", "serious-game", "Martian Music Invasion"));

		// List<Activity> groupingList = new List<Activity>();
		// groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement('http://cocotreestudios.com')) ???

		string audioFinishedURI = GBLXAPI.Instance.GetVocabURI("extension", "cutsceneaudiofinished");

		TinCan.Extensions contextExtensions = new TinCan.Extensions();
		contextExtensions.Add(audioFinishedURI, audioFinished.ToString());

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList, null, null, contextExtensions);

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}
	public static void SendTestStatementStarted(){

		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, "http://www.martianmusicinvasion.com/", "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("started");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/apps/GBLXAPITEST", "serious-game", "GBLXAPI TEST");
		Result statementResult = null;

		// context can be created right in the statement functions
		List<Activity> parentList = new List<Activity>();
		parentList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/apps/GBLXAPITEST", "serious-game", "GBLXAPI TEST"));

		List<Activity> groupingList = new List<Activity>();
		groupingList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("http://www.martianmusicinvasion.com/"));

		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList, groupingList);

		// QueueStatement(Agent statementActor, Verb statementVerb, Activity statementObject, Result statementResult, Context statementContext, StatementCallbackHandler sendCallback = null)
		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}

	public static void SendGameStarted(){

		Agent statementActor = GBLXAPI.Instance.CreateActorStatement(GBL_Interface.userUUID, GBL_Interface.companyURI, "Test User");
		Verb statementVerb = GBLXAPI.Instance.CreateVerbStatement("started");
		Activity statementObject = GBLXAPI.Instance.CreateObjectActivityStatement("http://martianmusicinvasion.com/gblxapitest", "serious-game", "GBLXAPI TEST");

		Context statementContext = null;
		Result statementResult = null;

		GBLXAPI.Instance.QueueStatement(statementActor, statementVerb, statementObject, statementResult, statementContext);
	}

	// // ------------------------------------------------------------------------
	// // Sample Context Generators
	// // ------------------------------------------------------------------------
    /*
    Since context generation can be many lines of code, it is often helpful to separate it out into helper functions. 
    These functions will be responsible for creating Context Activities, Context Extensions, and assigning them to a singular Context object.
     */

	public static Context CreateTestContext() {

		// CONTEXT ACTIVITIES

		// parent contains the activity just above this one in the hierarchy
		List<Activity> parentList = new List<Activity>();
		parentList.Add (GBLXAPI.Instance.CreateObjectActivityStatement(gameURI, "serious-game", gameName));

		// grouping contains all other related activities to this one
		List<Activity> groupingList = new List<Activity> ();
		groupingList.Add (GBLXAPI.Instance.CreateObjectActivityStatement (companyURI));

		// category is used in GBLxAPI to report on the subject area
		List<Activity> categoryList = new List<Activity> ();;
		categoryList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("https://gblxapi.org/socialstudies"));
		categoryList.Add(GBLXAPI.Instance.CreateObjectActivityStatement("https://gblxapi.org/math"));

		// CONTEXT EXTENSIONS

		// grade
		List<JToken> gradeList = new List<JToken>();
		gradeList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Grade", "Grade 4 level")); 

		// domain
		List<JToken> domainList = new List<JToken>();
		domainList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Domain", "History"));
		domainList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Domain", "Number and Operations in Base Ten"));

		// subdomain
		List<JToken> subdomainList = new List<JToken>();
		subdomainList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Subdomain", "Problem Solving"));

		// skill
		List<JToken> skillList = new List<JToken>();
		skillList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Skill","Patterns and Relationships"));
		skillList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Skill","Calculation and Computation"));
		
		// topic
		List<JToken> topicList = new List<JToken>();
		topicList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Topic","Arithmetic"));
		
		// focus
		List<JToken> focusList = new List<JToken>();
		focusList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Focus","Addition/Subtraction"));
		
		// action
		List<JToken> actionList = new List<JToken>();
		actionList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("Action","Solve Problems"));
		
		// c3/ccss
		List<JToken> c3List = new List<JToken>(); 
		c3List.Add(GBLXAPI.Instance.CreateContextExtensionStatement("C3 Framework", "d2.His.13.6-8."));

		List<JToken> ccList = new List<JToken>();
		ccList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("CC-MATH", "CCSS.Math.Content.4.NBT.B.4"));
		ccList.Add(GBLXAPI.Instance.CreateContextExtensionStatement("CC-MATH", "CCSS.Math.Content.5.NBT.A.1"));

		// creating TinCan.Extensions object to pack the lists into
		TinCan.Extensions contextExtensions = new TinCan.Extensions();
		// adding lists to extensions JObject
		GBLXAPI.Instance.PackExtension("domain", domainList, contextExtensions);
		GBLXAPI.Instance.PackExtension("subdomain", subdomainList, contextExtensions);
		GBLXAPI.Instance.PackExtension("topic", topicList, contextExtensions);
		GBLXAPI.Instance.PackExtension("focus", focusList, contextExtensions);
		GBLXAPI.Instance.PackExtension("action", actionList, contextExtensions);
		GBLXAPI.Instance.PackExtension("skill", skillList, contextExtensions);
		GBLXAPI.Instance.PackExtension("grade", gradeList, contextExtensions);
		GBLXAPI.Instance.PackExtension("cc", ccList, contextExtensions);
		GBLXAPI.Instance.PackExtension("c3", c3List, contextExtensions);

		// Folding all of the above into our Context object to be used in the statement
		Context statementContext = GBLXAPI.Instance.CreateContextActivityStatement(parentList, groupingList, categoryList, contextExtensions);
		return statementContext;
	}
}