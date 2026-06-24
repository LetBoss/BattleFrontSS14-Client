using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Silicons.Laws.Components;

[RegisterComponent]
public sealed class StartIonStormedComponent : Component, ISerializationGenerated<StartIonStormedComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int IonStormAmount = 1;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StartIonStormedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StartIonStormedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StartIonStormedComponent>(this, ref target, hookCtx, false, context))
		{
			int IonStormAmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(IonStormAmount, ref IonStormAmountTemp, hookCtx, false, context))
			{
				IonStormAmountTemp = IonStormAmount;
			}
			target.IonStormAmount = IonStormAmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StartIonStormedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StartIonStormedComponent cast = (StartIonStormedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StartIonStormedComponent cast = (StartIonStormedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StartIonStormedComponent def = (StartIonStormedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StartIonStormedComponent Instantiate()
	{
		return new StartIonStormedComponent();
	}
}
