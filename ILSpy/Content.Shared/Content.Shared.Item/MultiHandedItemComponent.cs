using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Item;

[RegisterComponent]
[NetworkedComponent]
public sealed class MultiHandedItemComponent : Component, ISerializationGenerated<MultiHandedItemComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int HandsNeeded = 2;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MultiHandedItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MultiHandedItemComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MultiHandedItemComponent>(this, ref target, hookCtx, false, context))
		{
			int HandsNeededTemp = 0;
			if (!serialization.TryCustomCopy<int>(HandsNeeded, ref HandsNeededTemp, hookCtx, false, context))
			{
				HandsNeededTemp = HandsNeeded;
			}
			target.HandsNeeded = HandsNeededTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MultiHandedItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MultiHandedItemComponent cast = (MultiHandedItemComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MultiHandedItemComponent cast = (MultiHandedItemComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MultiHandedItemComponent def = (MultiHandedItemComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MultiHandedItemComponent Instantiate()
	{
		return new MultiHandedItemComponent();
	}
}
