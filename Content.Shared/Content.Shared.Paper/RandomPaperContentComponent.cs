using System;
using Content.Shared.Dataset;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Paper;

[RegisterComponent]
public sealed class RandomPaperContentComponent : Component, ISerializationGenerated<RandomPaperContentComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<LocalizedDatasetPrototype> Dataset;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomPaperContentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (RandomPaperContentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RandomPaperContentComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<LocalizedDatasetPrototype> DatasetTemp = default(ProtoId<LocalizedDatasetPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<LocalizedDatasetPrototype>>(Dataset, ref DatasetTemp, hookCtx, false, context))
			{
				DatasetTemp = serialization.CreateCopy<ProtoId<LocalizedDatasetPrototype>>(Dataset, hookCtx, context, false);
			}
			target.Dataset = DatasetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomPaperContentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomPaperContentComponent cast = (RandomPaperContentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomPaperContentComponent cast = (RandomPaperContentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomPaperContentComponent def = (RandomPaperContentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RandomPaperContentComponent Instantiate()
	{
		return new RandomPaperContentComponent();
	}
}
