// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.ISharedPlayerManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Player;

[NotContentImplementable]
public interface ISharedPlayerManager
{
  ICommonSession[] Sessions { get; }

  ICommonSession[] NetworkedSessions { get; }

  IReadOnlyDictionary<NetUserId, ICommonSession> SessionsDict { get; }

  int PlayerCount { get; }

  int MaxPlayers { get; }

  void Initialize(int maxPlayers);

  void Startup();

  void Shutdown();

  void Dirty();

  [Robust.Shared.ViewVariables.ViewVariables]
  ICommonSession? LocalSession { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  NetUserId? LocalUser { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  EntityUid? LocalEntity { get; }

  event EventHandler<SessionStatusEventArgs>? PlayerStatusChanged;

  bool TryGetUserId(string userName, out NetUserId userId);

  bool TryGetSessionByEntity(EntityUid uid, [NotNullWhen(true)] out ICommonSession? session);

  bool TryGetSessionById([NotNullWhen(true)] NetUserId? user, [NotNullWhen(true)] out ICommonSession? session);

  bool TryGetSessionByUsername(string username, [NotNullWhen(true)] out ICommonSession? session);

  bool TryGetSessionByChannel(INetChannel channel, [NotNullWhen(true)] out ICommonSession? session);

  ICommonSession GetSessionByChannel(INetChannel channel) => this.GetSessionById(channel.UserId);

  ICommonSession GetSessionById(NetUserId user);

  bool ValidSessionId(NetUserId user)
  {
    return this.TryGetSessionById(new NetUserId?(user), out ICommonSession _);
  }

  SessionData GetPlayerData(NetUserId userId);

  bool TryGetPlayerData(NetUserId userId, [NotNullWhen(true)] out SessionData? data);

  bool TryGetPlayerDataByUsername(string userName, [NotNullWhen(true)] out SessionData? data);

  bool HasPlayerData(NetUserId userId);

  IEnumerable<SessionData> GetAllPlayerData();

  void GetPlayerStates(GameTick fromTick, List<SessionState> states);

  void UpdateState(ICommonSession commonSession);

  void RemoveSession(ICommonSession session, bool removeData = false);

  void RemoveSession(NetUserId user, bool removeData = false);

  ICommonSession CreateAndAddSession(INetChannel channel);

  ICommonSession CreateAndAddSession(NetUserId user, string name);

  bool SetAttachedEntity(
    [NotNullWhen(true)] ICommonSession? session,
    EntityUid? entity,
    out ICommonSession? kicked,
    bool force = false);

  bool SetAttachedEntity([NotNullWhen(true)] ICommonSession? session, EntityUid? entity, bool force = false)
  {
    return this.SetAttachedEntity(session, entity, out ICommonSession _, force);
  }

  void SetStatus(ICommonSession session, SessionStatus status);

  void SetPing(ICommonSession session, short ping);

  void SetName(ICommonSession session, string name);

  void JoinGame(ICommonSession session);
}
