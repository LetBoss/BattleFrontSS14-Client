using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Power.SMES;

[RegisterComponent]
public sealed class SmesComponent : Component, ISerializationGenerated<SmesComponent>, ISerializationGenerated
{
	[DataField("chargeOverlayPrefix", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ChargeOverlayPrefix = "smes-og";

	[DataField("inputOverlayPrefix", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string InputOverlayPrefix = "smes-oc";

	[DataField("outputOverlayPrefix", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string OutputOverlayPrefix = "smes-op";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SmesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (SmesComponent)(object)val;
		if (!serialization.TryCustomCopy<SmesComponent>(this, ref target, hookCtx, false, context))
		{
			string chargeOverlayPrefix = null;
			if (ChargeOverlayPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ChargeOverlayPrefix, ref chargeOverlayPrefix, hookCtx, false, context))
			{
				chargeOverlayPrefix = ChargeOverlayPrefix;
			}
			target.ChargeOverlayPrefix = chargeOverlayPrefix;
			string inputOverlayPrefix = null;
			if (InputOverlayPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(InputOverlayPrefix, ref inputOverlayPrefix, hookCtx, false, context))
			{
				inputOverlayPrefix = InputOverlayPrefix;
			}
			target.InputOverlayPrefix = inputOverlayPrefix;
			string outputOverlayPrefix = null;
			if (OutputOverlayPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(OutputOverlayPrefix, ref outputOverlayPrefix, hookCtx, false, context))
			{
				outputOverlayPrefix = OutputOverlayPrefix;
			}
			target.OutputOverlayPrefix = outputOverlayPrefix;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SmesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmesComponent target2 = (SmesComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmesComponent target2 = (SmesComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmesComponent target2 = (SmesComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SmesComponent Instantiate()
	{
		return new SmesComponent();
	}
}
