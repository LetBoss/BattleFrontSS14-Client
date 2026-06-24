using System;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Components;

[Serializable]
[NetSerializable]
public sealed class InputMoverComponentState : ComponentState
{
	public MoveButtons HeldMoveButtons;

	public NetEntity? RelativeEntity;

	public Angle TargetRelativeRotation;

	public Angle RelativeRotation;

	public TimeSpan LerpTarget;

	public bool CanMove;
}
