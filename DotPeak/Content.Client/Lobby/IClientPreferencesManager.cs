// Decompiled with JetBrains decompiler
// Type: Content.Client.Lobby.IClientPreferencesManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Construction.Prototypes;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Lobby;

public interface IClientPreferencesManager
{
  event Action OnServerDataLoaded;

  bool ServerDataLoaded => this.Settings != null;

  GameSettings? Settings { get; }

  PlayerPreferences? Preferences { get; }

  void Initialize();

  void SelectCharacter(ICharacterProfile profile);

  void SelectCharacter(int slot);

  void UpdateCharacter(ICharacterProfile profile, int slot);

  void CreateCharacter(ICharacterProfile profile);

  void DeleteCharacter(ICharacterProfile profile);

  void DeleteCharacter(int slot);

  void UpdateConstructionFavorites(List<ProtoId<ConstructionPrototype>> favorites);
}
