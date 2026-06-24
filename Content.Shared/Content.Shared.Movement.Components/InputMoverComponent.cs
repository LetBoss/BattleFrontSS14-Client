using System;
using System.Numerics;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class InputMoverComponent : Component, ISerializationGenerated<InputMoverComponent>, ISerializationGenerated
{
	public GameTick LastInputTick;

	public ushort LastInputSubTick;

	public Vector2 CurTickWalkMovement;

	public Vector2 CurTickSprintMovement;

	public MoveButtons HeldMoveButtons;

	public Vector2 WishDir;

	public EntityUid? RelativeEntity;

	[ViewVariables]
	public Angle TargetRelativeRotation = Angle.Zero;

	[ViewVariables]
	public Angle RelativeRotation;

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan LerpTarget;

	public const float LerpTime = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool CanMove = true;

	public bool HasDirectionalMovement => (HeldMoveButtons & MoveButtons.AnyDirection) != 0;

	public bool Sprinting => (HeldMoveButtons & MoveButtons.Walk) == 0;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InputMoverComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (InputMoverComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<InputMoverComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan LerpTargetTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(LerpTarget, ref LerpTargetTemp, hookCtx, false, context))
			{
				LerpTargetTemp = serialization.CreateCopy<TimeSpan>(LerpTarget, hookCtx, context, false);
			}
			target.LerpTarget = LerpTargetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InputMoverComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InputMoverComponent cast = (InputMoverComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InputMoverComponent cast = (InputMoverComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InputMoverComponent def = (InputMoverComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InputMoverComponent Instantiate()
	{
		return new InputMoverComponent();
	}
}
