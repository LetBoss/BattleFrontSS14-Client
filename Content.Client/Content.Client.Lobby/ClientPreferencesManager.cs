using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Preferences;
using Robust.Client;
using Robust.Client.Player;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Lobby;

public sealed class ClientPreferencesManager : IClientPreferencesManager
{
	[Dependency]
	private IClientNetManager _netManager;

	[Dependency]
	private IBaseClient _baseClient;

	[Dependency]
	private IPlayerManager _playerManager;

	public GameSettings Settings { get; private set; }

	public PlayerPreferences Preferences { get; private set; }

	public event Action? OnServerDataLoaded;

	public void Initialize()
	{
		((INetManager)_netManager).RegisterNetMessage<MsgPreferencesAndSettings>((ProcessMessage<MsgPreferencesAndSettings>)HandlePreferencesAndSettings, (NetMessageAccept)3);
		((INetManager)_netManager).RegisterNetMessage<MsgUpdateCharacter>((ProcessMessage<MsgUpdateCharacter>)null, (NetMessageAccept)3);
		((INetManager)_netManager).RegisterNetMessage<MsgSelectCharacter>((ProcessMessage<MsgSelectCharacter>)null, (NetMessageAccept)3);
		((INetManager)_netManager).RegisterNetMessage<MsgDeleteCharacter>((ProcessMessage<MsgDeleteCharacter>)null, (NetMessageAccept)3);
		_baseClient.RunLevelChanged += BaseClientOnRunLevelChanged;
	}

	private void BaseClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)e.NewLevel == 1)
		{
			Settings = null;
			Preferences = null;
		}
	}

	public void SelectCharacter(ICharacterProfile profile)
	{
		SelectCharacter(Preferences.IndexOfCharacter(profile));
	}

	public void SelectCharacter(int slot)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Preferences = new PlayerPreferences(Preferences.Characters, slot, Preferences.AdminOOCColor, Preferences.ConstructionFavorites);
		MsgSelectCharacter msgSelectCharacter = new MsgSelectCharacter
		{
			SelectedCharacterIndex = slot
		};
		((INetManager)_netManager).ClientSendMessage((NetMessage)(object)msgSelectCharacter);
	}

	public void UpdateCharacter(ICharacterProfile profile, int slot)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		IDependencyCollection instance = IoCManager.Instance;
		profile.EnsureValid(((ISharedPlayerManager)_playerManager).LocalSession, instance);
		Dictionary<int, ICharacterProfile> characters = new Dictionary<int, ICharacterProfile>(Preferences.Characters) { [slot] = profile };
		Preferences = new PlayerPreferences(characters, Preferences.SelectedCharacterIndex, Preferences.AdminOOCColor, Preferences.ConstructionFavorites);
		MsgUpdateCharacter msgUpdateCharacter = new MsgUpdateCharacter
		{
			Profile = profile,
			Slot = slot
		};
		((INetManager)_netManager).ClientSendMessage((NetMessage)(object)msgUpdateCharacter);
	}

	public void CreateCharacter(ICharacterProfile profile)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<int, ICharacterProfile> dictionary = new Dictionary<int, ICharacterProfile>(Preferences.Characters);
		int? num = Extensions.FirstOrNull<int>(Enumerable.Range(0, Settings.MaxCharacterSlots).Except(dictionary.Keys));
		if (!num.HasValue)
		{
			throw new InvalidOperationException("Out of character slots!");
		}
		int value = num.Value;
		dictionary.Add(value, profile);
		Preferences = new PlayerPreferences(dictionary, Preferences.SelectedCharacterIndex, Preferences.AdminOOCColor, Preferences.ConstructionFavorites);
		UpdateCharacter(profile, value);
	}

	public void DeleteCharacter(ICharacterProfile profile)
	{
		DeleteCharacter(Preferences.IndexOfCharacter(profile));
	}

	public void DeleteCharacter(int slot)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<KeyValuePair<int, ICharacterProfile>> characters = Preferences.Characters.Where<KeyValuePair<int, ICharacterProfile>>((KeyValuePair<int, ICharacterProfile> p) => p.Key != slot);
		Preferences = new PlayerPreferences(characters, Preferences.SelectedCharacterIndex, Preferences.AdminOOCColor, Preferences.ConstructionFavorites);
		MsgDeleteCharacter msgDeleteCharacter = new MsgDeleteCharacter
		{
			Slot = slot
		};
		((INetManager)_netManager).ClientSendMessage((NetMessage)(object)msgDeleteCharacter);
	}

	public void UpdateConstructionFavorites(List<ProtoId<ConstructionPrototype>> favorites)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Preferences = new PlayerPreferences(Preferences.Characters, Preferences.SelectedCharacterIndex, Preferences.AdminOOCColor, favorites);
		MsgUpdateConstructionFavorites msgUpdateConstructionFavorites = new MsgUpdateConstructionFavorites
		{
			Favorites = favorites
		};
		((INetManager)_netManager).ClientSendMessage((NetMessage)(object)msgUpdateConstructionFavorites);
	}

	private void HandlePreferencesAndSettings(MsgPreferencesAndSettings message)
	{
		Preferences = message.Preferences;
		Settings = message.Settings;
		this.OnServerDataLoaded?.Invoke();
	}
}
