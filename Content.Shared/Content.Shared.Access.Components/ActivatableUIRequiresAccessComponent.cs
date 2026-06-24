using System;
using Content.Shared.Access.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Access.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ActivatableUIRequiresAccessSystem) })]
public sealed class ActivatableUIRequiresAccessComponent : Component, ISerializationGenerated<ActivatableUIRequiresAccessComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public LocId? PopupMessage = LocId.op_Implicit("lock-comp-has-user-access-fail");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActivatableUIRequiresAccessComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActivatableUIRequiresAccessComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ActivatableUIRequiresAccessComponent>(this, ref target, hookCtx, false, context))
		{
			LocId? PopupMessageTemp = null;
			if (!serialization.TryCustomCopy<LocId?>(PopupMessage, ref PopupMessageTemp, hookCtx, false, context))
			{
				PopupMessageTemp = serialization.CreateCopy<LocId?>(PopupMessage, hookCtx, context, false);
			}
			target.PopupMessage = PopupMessageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActivatableUIRequiresAccessComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresAccessComponent cast = (ActivatableUIRequiresAccessComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresAccessComponent cast = (ActivatableUIRequiresAccessComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresAccessComponent def = (ActivatableUIRequiresAccessComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActivatableUIRequiresAccessComponent Instantiate()
	{
		return new ActivatableUIRequiresAccessComponent();
	}
}
