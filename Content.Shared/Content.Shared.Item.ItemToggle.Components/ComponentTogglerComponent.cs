using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Item.ItemToggle.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ComponentTogglerSystem) })]
public sealed class ComponentTogglerComponent : Component, ISerializationGenerated<ComponentTogglerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ComponentRegistry Components = new ComponentRegistry();

	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry? RemoveComponents;

	[DataField(null, false, 1, false, false, null)]
	public bool Parent;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Target;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ComponentTogglerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ComponentTogglerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ComponentTogglerComponent>(this, ref target, hookCtx, false, context))
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
			ComponentRegistry RemoveComponentsTemp = null;
			if (!serialization.TryCustomCopy<ComponentRegistry>(RemoveComponents, ref RemoveComponentsTemp, hookCtx, false, context))
			{
				RemoveComponentsTemp = serialization.CreateCopy<ComponentRegistry>(RemoveComponents, hookCtx, context, false);
			}
			target.RemoveComponents = RemoveComponentsTemp;
			bool ParentTemp = false;
			if (!serialization.TryCustomCopy<bool>(Parent, ref ParentTemp, hookCtx, false, context))
			{
				ParentTemp = Parent;
			}
			target.Parent = ParentTemp;
			EntityUid? TargetTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Target, ref TargetTemp, hookCtx, false, context))
			{
				TargetTemp = serialization.CreateCopy<EntityUid?>(Target, hookCtx, context, false);
			}
			target.Target = TargetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ComponentTogglerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ComponentTogglerComponent cast = (ComponentTogglerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ComponentTogglerComponent cast = (ComponentTogglerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ComponentTogglerComponent def = (ComponentTogglerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ComponentTogglerComponent Instantiate()
	{
		return new ComponentTogglerComponent();
	}
}
