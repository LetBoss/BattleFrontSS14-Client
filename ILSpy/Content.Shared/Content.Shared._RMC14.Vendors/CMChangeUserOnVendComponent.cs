using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vendors;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedCMAutomatedVendorSystem) })]
public sealed class CMChangeUserOnVendComponent : Component, ISerializationGenerated<CMChangeUserOnVendComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry? AddComponents;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CMChangeUserOnVendComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CMChangeUserOnVendComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CMChangeUserOnVendComponent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry AddComponentsTemp = null;
			if (!serialization.TryCustomCopy<ComponentRegistry>(AddComponents, ref AddComponentsTemp, hookCtx, false, context))
			{
				AddComponentsTemp = serialization.CreateCopy<ComponentRegistry>(AddComponents, hookCtx, context, false);
			}
			target.AddComponents = AddComponentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CMChangeUserOnVendComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMChangeUserOnVendComponent cast = (CMChangeUserOnVendComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMChangeUserOnVendComponent cast = (CMChangeUserOnVendComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMChangeUserOnVendComponent def = (CMChangeUserOnVendComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CMChangeUserOnVendComponent Instantiate()
	{
		return new CMChangeUserOnVendComponent();
	}
}
