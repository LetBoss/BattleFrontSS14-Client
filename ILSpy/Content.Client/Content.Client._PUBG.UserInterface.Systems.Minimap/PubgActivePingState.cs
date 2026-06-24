using System;
using System.Numerics;
using Content.Shared._PUBG.Party;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public readonly record struct PubgActivePingState(NetEntity Source, MapId MapId, Vector2 Position, PubgPartyPingKind Kind, string? ItemPrototypeId, DateTime ExpiresAtUtc);
