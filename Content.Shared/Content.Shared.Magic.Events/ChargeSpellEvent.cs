using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Magic.Events;

public sealed class ChargeSpellEvent : InstantActionEvent, ISerializationGenerated<ChargeSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public int Charge;

	[DataField(null, false, 1, false, false, null)]
	public string WandTag = "WizardWand";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChargeSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ChargeSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<ChargeSpellEvent>(this, ref target, hookCtx, false, context))
		{
			int ChargeTemp = 0;
			if (!serialization.TryCustomCopy<int>(Charge, ref ChargeTemp, hookCtx, false, context))
			{
				ChargeTemp = Charge;
			}
			target.Charge = ChargeTemp;
			string WandTagTemp = null;
			if (WandTag == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(WandTag, ref WandTagTemp, hookCtx, false, context))
			{
				WandTagTemp = WandTag;
			}
			target.WandTag = WandTagTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChargeSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChargeSpellEvent cast = (ChargeSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChargeSpellEvent cast = (ChargeSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ChargeSpellEvent Instantiate()
	{
		return new ChargeSpellEvent();
	}
}
