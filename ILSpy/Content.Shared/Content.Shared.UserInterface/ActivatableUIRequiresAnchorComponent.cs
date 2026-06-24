using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.UserInterface;

[RegisterComponent]
[NetworkedComponent]
public sealed class ActivatableUIRequiresAnchorComponent : Component, ISerializationGenerated<ActivatableUIRequiresAnchorComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public LocId? Popup = LocId.op_Implicit("ui-needs-anchor");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActivatableUIRequiresAnchorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActivatableUIRequiresAnchorComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ActivatableUIRequiresAnchorComponent>(this, ref target, hookCtx, false, context))
		{
			LocId? PopupTemp = null;
			if (!serialization.TryCustomCopy<LocId?>(Popup, ref PopupTemp, hookCtx, false, context))
			{
				PopupTemp = serialization.CreateCopy<LocId?>(Popup, hookCtx, context, false);
			}
			target.Popup = PopupTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActivatableUIRequiresAnchorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresAnchorComponent cast = (ActivatableUIRequiresAnchorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresAnchorComponent cast = (ActivatableUIRequiresAnchorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresAnchorComponent def = (ActivatableUIRequiresAnchorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActivatableUIRequiresAnchorComponent Instantiate()
	{
		return new ActivatableUIRequiresAnchorComponent();
	}
}
