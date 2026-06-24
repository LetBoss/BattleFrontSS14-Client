using System;
using Content.Shared.Anomaly.Effects;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Anomaly.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedInnerBodyAnomalySystem) })]
public sealed class InnerBodyAnomalyInjectorComponent : Component, ISerializationGenerated<InnerBodyAnomalyInjectorComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, true, false, null)]
	public ComponentRegistry InjectionComponents;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InnerBodyAnomalyInjectorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (InnerBodyAnomalyInjectorComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<InnerBodyAnomalyInjectorComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		target.Whitelist = WhitelistTemp;
		ComponentRegistry InjectionComponentsTemp = null;
		if (InjectionComponents == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ComponentRegistry>(InjectionComponents, ref InjectionComponentsTemp, hookCtx, false, context))
		{
			InjectionComponentsTemp = serialization.CreateCopy<ComponentRegistry>(InjectionComponents, hookCtx, context, false);
		}
		target.InjectionComponents = InjectionComponentsTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InnerBodyAnomalyInjectorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InnerBodyAnomalyInjectorComponent cast = (InnerBodyAnomalyInjectorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InnerBodyAnomalyInjectorComponent cast = (InnerBodyAnomalyInjectorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InnerBodyAnomalyInjectorComponent def = (InnerBodyAnomalyInjectorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InnerBodyAnomalyInjectorComponent Instantiate()
	{
		return new InnerBodyAnomalyInjectorComponent();
	}
}
