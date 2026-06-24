using System;
using Content.Shared.Administration;
using Robust.Shared.Network;

namespace Content.Client.UserInterface.Systems.Bwoink;

public interface IAHelpUIHandler : IDisposable
{
	bool IsAdmin { get; }

	bool IsOpen { get; }

	Action<NetUserId, string, bool, bool>? SendMessageAction { get; set; }

	event Action OnClose;

	event Action OnOpen;

	event Action<NetUserId, string>? InputTextChanged;

	void Receive(SharedBwoinkSystem.BwoinkTextMessage message);

	void Close();

	void Open(NetUserId netUserId, bool relayActive);

	void ToggleWindow();

	void DiscordRelayChanged(bool active);

	void PeopleTypingUpdated(BwoinkPlayerTypingUpdated args);
}
