// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Minimap.PubgActivePingState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Party;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public readonly record struct PubgActivePingState(
  NetEntity Source,
  MapId MapId,
  Vector2 Position,
  PubgPartyPingKind Kind,
  string? ItemPrototypeId,
  DateTime ExpiresAtUtc)
;
