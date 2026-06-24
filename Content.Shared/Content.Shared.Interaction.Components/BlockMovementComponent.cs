using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Interaction.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class BlockMovementComponent : Component, ISerializationGenerated<BlockMovementComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool BlockInteraction = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BlockMovementComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BlockMovementComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<BlockMovementComponent>(this, ref target, hookCtx, false, context))
		{
			bool BlockInteractionTemp = false;
			if (!serialization.TryCustomCopy<bool>(BlockInteraction, ref BlockInteractionTemp, hookCtx, false, context))
			{
				BlockInteractionTemp = BlockInteraction;
			}
			target.BlockInteraction = BlockInteractionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BlockMovementComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlockMovementComponent cast = (BlockMovementComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlockMovementComponent cast = (BlockMovementComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlockMovementComponent def = (BlockMovementComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BlockMovementComponent Instantiate()
	{
		return new BlockMovementComponent();
	}
}
