using System;
using Content.Shared.Dataset;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Traits.Assorted;

[RegisterComponent]
[NetworkedComponent]
public sealed class PainNumbnessComponent : Component, ISerializationGenerated<PainNumbnessComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<LocalizedDatasetPrototype> ForceSayNumbDataset = ProtoId<LocalizedDatasetPrototype>.op_Implicit("ForceSayNumbDataset");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PainNumbnessComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PainNumbnessComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PainNumbnessComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<LocalizedDatasetPrototype> ForceSayNumbDatasetTemp = default(ProtoId<LocalizedDatasetPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<LocalizedDatasetPrototype>>(ForceSayNumbDataset, ref ForceSayNumbDatasetTemp, hookCtx, false, context))
			{
				ForceSayNumbDatasetTemp = serialization.CreateCopy<ProtoId<LocalizedDatasetPrototype>>(ForceSayNumbDataset, hookCtx, context, false);
			}
			target.ForceSayNumbDataset = ForceSayNumbDatasetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PainNumbnessComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PainNumbnessComponent cast = (PainNumbnessComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PainNumbnessComponent cast = (PainNumbnessComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PainNumbnessComponent def = (PainNumbnessComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PainNumbnessComponent Instantiate()
	{
		return new PainNumbnessComponent();
	}
}
