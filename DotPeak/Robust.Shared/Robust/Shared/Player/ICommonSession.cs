// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.ICommonSession
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Player;

[NotContentImplementable]
public interface ICommonSession
{
  SessionStatus Status { get; }

  EntityUid? AttachedEntity { get; }

  NetUserId UserId { get; }

  string Name { get; }

  short Ping { get; }

  INetChannel Channel { get; set; }

  LoginType AuthType { get; }

  HashSet<EntityUid> ViewSubscriptions { get; }

  DateTime ConnectedTime { get; set; }

  SessionState State { get; }

  SessionData Data { get; }

  bool ClientSide { get; set; }
}
