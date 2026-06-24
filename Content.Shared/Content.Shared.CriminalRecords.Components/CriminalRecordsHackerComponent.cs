using System;
using Content.Shared.CriminalRecords.Systems;
using Content.Shared.Dataset;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CriminalRecords.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedCriminalRecordsHackerSystem) })]
public sealed class CriminalRecordsHackerComponent : Component, ISerializationGenerated<CriminalRecordsHackerComponent>, ISerializationGenerated
{
	public TimeSpan Delay = TimeSpan.FromSeconds(20L);

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<LocalizedDatasetPrototype> Reasons = ProtoId<LocalizedDatasetPrototype>.op_Implicit("CriminalRecordsWantedReasonPlaceholders");

	[DataField(null, false, 1, false, false, null)]
	public LocId Announcement = LocId.op_Implicit("ninja-criminal-records-hack-announcement");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CriminalRecordsHackerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CriminalRecordsHackerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CriminalRecordsHackerComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<LocalizedDatasetPrototype> ReasonsTemp = default(ProtoId<LocalizedDatasetPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<LocalizedDatasetPrototype>>(Reasons, ref ReasonsTemp, hookCtx, false, context))
			{
				ReasonsTemp = serialization.CreateCopy<ProtoId<LocalizedDatasetPrototype>>(Reasons, hookCtx, context, false);
			}
			target.Reasons = ReasonsTemp;
			LocId AnnouncementTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(Announcement, ref AnnouncementTemp, hookCtx, false, context))
			{
				AnnouncementTemp = serialization.CreateCopy<LocId>(Announcement, hookCtx, context, false);
			}
			target.Announcement = AnnouncementTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CriminalRecordsHackerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CriminalRecordsHackerComponent cast = (CriminalRecordsHackerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CriminalRecordsHackerComponent cast = (CriminalRecordsHackerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CriminalRecordsHackerComponent def = (CriminalRecordsHackerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CriminalRecordsHackerComponent Instantiate()
	{
		return new CriminalRecordsHackerComponent();
	}
}
