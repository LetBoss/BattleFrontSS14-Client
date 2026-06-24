using System;
using System.Collections.Generic;
using Content.Shared.Mobs;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Bots;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(MedibotSystem) })]
public sealed class EmaggableMedibotComponent : Component, ISerializationGenerated<EmaggableMedibotComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<MobState, MedibotTreatment> Replacements = new Dictionary<MobState, MedibotTreatment>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EmaggableMedibotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EmaggableMedibotComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EmaggableMedibotComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<MobState, MedibotTreatment> ReplacementsTemp = null;
			if (Replacements == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<MobState, MedibotTreatment>>(Replacements, ref ReplacementsTemp, hookCtx, true, context))
			{
				ReplacementsTemp = serialization.CreateCopy<Dictionary<MobState, MedibotTreatment>>(Replacements, hookCtx, context, false);
			}
			target.Replacements = ReplacementsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EmaggableMedibotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmaggableMedibotComponent cast = (EmaggableMedibotComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmaggableMedibotComponent cast = (EmaggableMedibotComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmaggableMedibotComponent def = (EmaggableMedibotComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EmaggableMedibotComponent Instantiate()
	{
		return new EmaggableMedibotComponent();
	}
}
