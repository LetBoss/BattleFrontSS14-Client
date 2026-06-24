using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Interaction.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class UnremoveableComponent : Component, ISerializationGenerated<UnremoveableComponent>, ISerializationGenerated
{
	[DataField("deleteOnDrop", false, 1, false, false, null)]
	public bool DeleteOnDrop = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref UnremoveableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (UnremoveableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<UnremoveableComponent>(this, ref target, hookCtx, false, context))
		{
			bool DeleteOnDropTemp = false;
			if (!serialization.TryCustomCopy<bool>(DeleteOnDrop, ref DeleteOnDropTemp, hookCtx, false, context))
			{
				DeleteOnDropTemp = DeleteOnDrop;
			}
			target.DeleteOnDrop = DeleteOnDropTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref UnremoveableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UnremoveableComponent cast = (UnremoveableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UnremoveableComponent cast = (UnremoveableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UnremoveableComponent def = (UnremoveableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override UnremoveableComponent Instantiate()
	{
		return new UnremoveableComponent();
	}
}
