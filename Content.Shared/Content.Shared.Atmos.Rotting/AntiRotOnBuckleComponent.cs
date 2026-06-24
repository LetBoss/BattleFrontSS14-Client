using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Rotting;

[RegisterComponent]
[NetworkedComponent]
public sealed class AntiRotOnBuckleComponent : Component, ISerializationGenerated<AntiRotOnBuckleComponent>, ISerializationGenerated
{
	[DataField("requiresPower", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool RequiresPower = true;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Enabled = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AntiRotOnBuckleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AntiRotOnBuckleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AntiRotOnBuckleComponent>(this, ref target, hookCtx, false, context))
		{
			bool RequiresPowerTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequiresPower, ref RequiresPowerTemp, hookCtx, false, context))
			{
				RequiresPowerTemp = RequiresPower;
			}
			target.RequiresPower = RequiresPowerTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AntiRotOnBuckleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AntiRotOnBuckleComponent cast = (AntiRotOnBuckleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AntiRotOnBuckleComponent cast = (AntiRotOnBuckleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AntiRotOnBuckleComponent def = (AntiRotOnBuckleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AntiRotOnBuckleComponent Instantiate()
	{
		return new AntiRotOnBuckleComponent();
	}
}
