using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Payload.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PayloadTriggerComponent : Component, ISerializationGenerated<PayloadTriggerComponent>, ISerializationGenerated
{
	public bool Active;

	[DataField("components", true, 1, false, true, null)]
	public ComponentRegistry? Components;

	[DataField("grantedComponents", false, 1, false, true, null)]
	public HashSet<Type> GrantedComponents = new HashSet<Type>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PayloadTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PayloadTriggerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PayloadTriggerComponent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry ComponentsTemp = null;
			if (!serialization.TryCustomCopy<ComponentRegistry>(Components, ref ComponentsTemp, hookCtx, false, context))
			{
				ComponentsTemp = serialization.CreateCopy<ComponentRegistry>(Components, hookCtx, context, false);
			}
			target.Components = ComponentsTemp;
			HashSet<Type> GrantedComponentsTemp = null;
			if (GrantedComponents == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<Type>>(GrantedComponents, ref GrantedComponentsTemp, hookCtx, true, context))
			{
				GrantedComponentsTemp = serialization.CreateCopy<HashSet<Type>>(GrantedComponents, hookCtx, context, false);
			}
			target.GrantedComponents = GrantedComponentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PayloadTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PayloadTriggerComponent cast = (PayloadTriggerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PayloadTriggerComponent cast = (PayloadTriggerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PayloadTriggerComponent def = (PayloadTriggerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PayloadTriggerComponent Instantiate()
	{
		return new PayloadTriggerComponent();
	}
}
