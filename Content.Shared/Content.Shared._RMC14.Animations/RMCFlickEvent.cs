using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Animations;

[Serializable]
[NetSerializable]
public sealed class RMCFlickEvent(NetEntity entity, Rsi animationState, Rsi defaultState, string? layer) : EntityEventArgs
{
	public readonly NetEntity Entity = entity;

	public readonly Rsi AnimationState = animationState;

	public readonly Rsi DefaultState = defaultState;

	public readonly string? Layer = layer;
}
