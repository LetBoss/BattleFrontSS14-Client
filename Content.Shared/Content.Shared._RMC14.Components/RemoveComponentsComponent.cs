using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(RMCComponentsSystem) })]
public sealed class RemoveComponentsComponent : Component, ISerializationGenerated<RemoveComponentsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry Components = new ComponentRegistry();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RemoveComponentsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RemoveComponentsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RemoveComponentsComponent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry ComponentsTemp = null;
			if (Components == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ComponentRegistry>(Components, ref ComponentsTemp, hookCtx, false, context))
			{
				ComponentsTemp = serialization.CreateCopy<ComponentRegistry>(Components, hookCtx, context, false);
			}
			target.Components = ComponentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RemoveComponentsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RemoveComponentsComponent cast = (RemoveComponentsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RemoveComponentsComponent cast = (RemoveComponentsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RemoveComponentsComponent def = (RemoveComponentsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RemoveComponentsComponent Instantiate()
	{
		return new RemoveComponentsComponent();
	}
}
