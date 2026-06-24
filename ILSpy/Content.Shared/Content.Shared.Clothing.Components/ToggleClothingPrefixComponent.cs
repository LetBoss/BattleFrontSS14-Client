using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ToggleClothingPrefixComponent : Component, ISerializationGenerated<ToggleClothingPrefixComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string? PrefixOn = "on";

	[DataField(null, false, 1, false, false, null)]
	public string? PrefixOff;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ToggleClothingPrefixComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ToggleClothingPrefixComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ToggleClothingPrefixComponent>(this, ref target, hookCtx, false, context))
		{
			string PrefixOnTemp = null;
			if (!serialization.TryCustomCopy<string>(PrefixOn, ref PrefixOnTemp, hookCtx, false, context))
			{
				PrefixOnTemp = PrefixOn;
			}
			target.PrefixOn = PrefixOnTemp;
			string PrefixOffTemp = null;
			if (!serialization.TryCustomCopy<string>(PrefixOff, ref PrefixOffTemp, hookCtx, false, context))
			{
				PrefixOffTemp = PrefixOff;
			}
			target.PrefixOff = PrefixOffTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ToggleClothingPrefixComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToggleClothingPrefixComponent cast = (ToggleClothingPrefixComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToggleClothingPrefixComponent cast = (ToggleClothingPrefixComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToggleClothingPrefixComponent def = (ToggleClothingPrefixComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ToggleClothingPrefixComponent Instantiate()
	{
		return new ToggleClothingPrefixComponent();
	}
}
