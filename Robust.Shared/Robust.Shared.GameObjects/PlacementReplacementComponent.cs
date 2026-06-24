using System;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
public sealed class PlacementReplacementComponent : Component, ISerializationGenerated<PlacementReplacementComponent>, ISerializationGenerated
{
	[DataField("key", false, 1, false, false, null)]
	public string Key = "";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlacementReplacementComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (PlacementReplacementComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			string target3 = null;
			if (Key == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Key, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Key;
			}
			target.Key = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlacementReplacementComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlacementReplacementComponent target2 = (PlacementReplacementComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlacementReplacementComponent target2 = (PlacementReplacementComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlacementReplacementComponent target2 = (PlacementReplacementComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlacementReplacementComponent Instantiate()
	{
		return new PlacementReplacementComponent();
	}
}
