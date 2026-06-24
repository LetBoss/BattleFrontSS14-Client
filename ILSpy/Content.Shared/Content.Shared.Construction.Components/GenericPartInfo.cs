using System;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Components;

[Serializable]
[DataDefinition]
public struct GenericPartInfo : ISerializationGenerated<GenericPartInfo>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public int Amount = 0;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId DefaultPrototype = default(EntProtoId);

	[DataField(null, false, 1, false, false, null)]
	public LocId? ExamineName = null;

	public GenericPartInfo()
	{
	}//IL_000d: Unknown result type (might be due to invalid IL or missing references)


	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GenericPartInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<GenericPartInfo>(this, ref target, hookCtx, false, context))
		{
			int AmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			EntProtoId DefaultPrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(DefaultPrototype, ref DefaultPrototypeTemp, hookCtx, false, context))
			{
				DefaultPrototypeTemp = serialization.CreateCopy<EntProtoId>(DefaultPrototype, hookCtx, context, false);
			}
			LocId? ExamineNameTemp = null;
			if (!serialization.TryCustomCopy<LocId?>(ExamineName, ref ExamineNameTemp, hookCtx, false, context))
			{
				ExamineNameTemp = serialization.CreateCopy<LocId?>(ExamineName, hookCtx, context, false);
			}
			GenericPartInfo genericPartInfo = target;
			genericPartInfo.Amount = AmountTemp;
			genericPartInfo.DefaultPrototype = DefaultPrototypeTemp;
			genericPartInfo.ExamineName = ExamineNameTemp;
			target = genericPartInfo;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GenericPartInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericPartInfo cast = (GenericPartInfo)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public GenericPartInfo Instantiate()
	{
		return new GenericPartInfo();
	}
}
