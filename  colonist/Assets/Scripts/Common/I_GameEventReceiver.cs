using UnityEngine;
using System.Collections;

/// <summary>
/// Interface to define functions for the game object which can receive event in game.
/// </summary>
public interface I_GameEventReceiver  {

	void OnGameEvent(GameEvent _event);
	
}
