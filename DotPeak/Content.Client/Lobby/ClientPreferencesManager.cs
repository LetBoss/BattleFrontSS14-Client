// Decompiled with JetBrains decompiler
// Type: Content.Client.Lobby.ClientPreferencesManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Construction.Prototypes;
using Content.Shared.Preferences;
using Robust.Client;
using Robust.Client.Player;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Lobby;

public sealed class ClientPreferencesManager : IClientPreferencesManager
{
  [Dependency]
  private IClientNetManager _netManager;
  [Dependency]
  private IBaseClient _baseClient;
  [Dependency]
  private IPlayerManager _playerManager;

  public event Action? OnServerDataLoaded;

  public GameSettings Settings { get; private set; }

  public PlayerPreferences Preferences { get; private set; }

  public void Initialize()
  {
    // ISSUE: method pointer
    ((INetManager) this._netManager).RegisterNetMessage<MsgPreferencesAndSettings>(new ProcessMessage<MsgPreferencesAndSettings>((object) this, __methodptr(HandlePreferencesAndSettings)), (NetMessageAccept) 3);
    ((INetManager) this._netManager).RegisterNetMessage<MsgUpdateCharacter>((ProcessMessage<MsgUpdateCharacter>) null, (NetMessageAccept) 3);
    ((INetManager) this._netManager).RegisterNetMessage<MsgSelectCharacter>((ProcessMessage<MsgSelectCharacter>) null, (NetMessageAccept) 3);
    ((INetManager) this._netManager).RegisterNetMessage<MsgDeleteCharacter>((ProcessMessage<MsgDeleteCharacter>) null, (NetMessageAccept) 3);
    this._baseClient.RunLevelChanged += new EventHandler<RunLevelChangedEventArgs>(this.BaseClientOnRunLevelChanged);
  }

  private void BaseClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
  {
    if (e.NewLevel != 1)
      return;
    this.Settings = (GameSettings) null;
    this.Preferences = (PlayerPreferences) null;
  }

  public void SelectCharacter(ICharacterProfile profile)
  {
    this.SelectCharacter(this.Preferences.IndexOfCharacter(profile));
  }

  public void SelectCharacter(int slot)
  {
    this.Preferences = new PlayerPreferences((IEnumerable<KeyValuePair<int, ICharacterProfile>>) this.Preferences.Characters, slot, this.Preferences.AdminOOCColor, this.Preferences.ConstructionFavorites);
    ((INetManager) this._netManager).ClientSendMessage((NetMessage) new MsgSelectCharacter()
    {
      SelectedCharacterIndex = slot
    });
  }

  public void UpdateCharacter(ICharacterProfile profile, int slot)
  {
    IDependencyCollection instance = IoCManager.Instance;
    profile.EnsureValid(((ISharedPlayerManager) this._playerManager).LocalSession, instance);
    this.Preferences = new PlayerPreferences((IEnumerable<KeyValuePair<int, ICharacterProfile>>) new Dictionary<int, ICharacterProfile>((IEnumerable<KeyValuePair<int, ICharacterProfile>>) this.Preferences.Characters)
    {
      [slot] = profile
    }, this.Preferences.SelectedCharacterIndex, this.Preferences.AdminOOCColor, this.Preferences.ConstructionFavorites);
    ((INetManager) this._netManager).ClientSendMessage((NetMessage) new MsgUpdateCharacter()
    {
      Profile = profile,
      Slot = slot
    });
  }

  public void CreateCharacter(ICharacterProfile profile)
  {
    Dictionary<int, ICharacterProfile> characters = new Dictionary<int, ICharacterProfile>((IEnumerable<KeyValuePair<int, ICharacterProfile>>) this.Preferences.Characters);
    int num = (Extensions.FirstOrNull<int>(Enumerable.Range(0, this.Settings.MaxCharacterSlots).Except<int>((IEnumerable<int>) characters.Keys)) ?? throw new InvalidOperationException("Out of character slots!")).Value;
    characters.Add(num, profile);
    this.Preferences = new PlayerPreferences((IEnumerable<KeyValuePair<int, ICharacterProfile>>) characters, this.Preferences.SelectedCharacterIndex, this.Preferences.AdminOOCColor, this.Preferences.ConstructionFavorites);
    this.UpdateCharacter(profile, num);
  }

  public void DeleteCharacter(ICharacterProfile profile)
  {
    this.DeleteCharacter(this.Preferences.IndexOfCharacter(profile));
  }

  public void DeleteCharacter(int slot)
  {
    this.Preferences = new PlayerPreferences(this.Preferences.Characters.Where<KeyValuePair<int, ICharacterProfile>>((Func<KeyValuePair<int, ICharacterProfile>, bool>) (p => p.Key != slot)), this.Preferences.SelectedCharacterIndex, this.Preferences.AdminOOCColor, this.Preferences.ConstructionFavorites);
    ((INetManager) this._netManager).ClientSendMessage((NetMessage) new MsgDeleteCharacter()
    {
      Slot = slot
    });
  }

  public void UpdateConstructionFavorites(List<ProtoId<ConstructionPrototype>> favorites)
  {
    this.Preferences = new PlayerPreferences((IEnumerable<KeyValuePair<int, ICharacterProfile>>) this.Preferences.Characters, this.Preferences.SelectedCharacterIndex, this.Preferences.AdminOOCColor, favorites);
    ((INetManager) this._netManager).ClientSendMessage((NetMessage) new MsgUpdateConstructionFavorites()
    {
      Favorites = favorites
    });
  }

  private void HandlePreferencesAndSettings(MsgPreferencesAndSettings message)
  {
    this.Preferences = message.Preferences;
    this.Settings = message.Settings;
    Action serverDataLoaded = this.OnServerDataLoaded;
    if (serverDataLoaded == null)
      return;
    serverDataLoaded();
  }
}
