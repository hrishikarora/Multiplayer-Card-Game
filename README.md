# Requirements
- Unity: 6000.2.6f2
- Target platform: Android
- Internet connection for Photon

# How To Run
1. Open the Unity project in the Unity editor.
2. Open the LobbyScene.
3. Press Play in the editor.
4. Start a second instance (another editor or an Android build) and open LobbyScene.
5. On both clients, press the quick match button.
6. When both are in the same room, the game scene will load and the match will start.

# Networking
- The game uses Photon for networking.
- 1v1 matchmaking is done through a quick-join style lobby that joins or creates a room.
- Players are assigned IDs P1 and P2 using room custom properties.
- Game state changes (game start, turn start, end turn, reveal cards, end game) are sent as JSON messages with an `action` field.
# Reconnection
- The game supports automatic reconnection while the app is still running and the room is alive. However, if a player fully closes the app, they currently cannot rejoin the same match; this could be extended in the future by rejoining the room and restoring the state from a saved GameSnapshot.
- The game automatically considers it a forfeit if a user is unable to rejoin within a certain time window, and the other player wins.

# Cards and JSON
- Card data is a JSON file under:
  `Resources/Cards/card.json`

# Event-Driven Architecture
The game uses events for main gameplay actions:
- GameStart
- TurnStart
- PlayerEndedTurn
- RevealCards
- GameEnd

The networking layer listens for JSON messages, converts them into these events, and the game systems react to the events (UI, scoring, hand management, turn management).

# Known Limits and Notes
- If a player closes the app, they currently cannot reconnect to the same match. This could be added by letting them rejoin the room and restoring the game state from a GameSnapshot sent by the master client.
