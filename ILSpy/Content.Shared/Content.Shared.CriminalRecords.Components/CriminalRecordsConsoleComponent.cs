using System;
using Content.Shared.CriminalRecords.Systems;
using Content.Shared.Radio;
using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CriminalRecords.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedCriminalRecordsConsoleSystem) })]
public sealed class CriminalRecordsConsoleComponent : Component, ISerializationGenerated<CriminalRecordsConsoleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public uint? ActiveKey;

	[DataField(null, false, 1, false, false, null)]
	public StationRecordsFilter? Filter;

	[DataField(null, false, 1, false, false, null)]
	public SecurityStatus FilterStatus;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<RadioChannelPrototype> SecurityChannel = ProtoId<RadioChannelPrototype>.op_Implicit("Security");

	[DataField(null, false, 1, false, false, null)]
	public uint MaxStringLength = 256u;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CriminalRecordsConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CriminalRecordsConsoleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CriminalRecordsConsoleComponent>(this, ref target, hookCtx, false, context))
		{
			uint? ActiveKeyTemp = null;
			if (!serialization.TryCustomCopy<uint?>(ActiveKey, ref ActiveKeyTemp, hookCtx, false, context))
			{
				ActiveKeyTemp = ActiveKey;
			}
			target.ActiveKey = ActiveKeyTemp;
			StationRecordsFilter FilterTemp = null;
			if (!serialization.TryCustomCopy<StationRecordsFilter>(Filter, ref FilterTemp, hookCtx, false, context))
			{
				FilterTemp = serialization.CreateCopy<StationRecordsFilter>(Filter, hookCtx, context, false);
			}
			target.Filter = FilterTemp;
			SecurityStatus FilterStatusTemp = SecurityStatus.None;
			if (!serialization.TryCustomCopy<SecurityStatus>(FilterStatus, ref FilterStatusTemp, hookCtx, false, context))
			{
				FilterStatusTemp = FilterStatus;
			}
			target.FilterStatus = FilterStatusTemp;
			ProtoId<RadioChannelPrototype> SecurityChannelTemp = default(ProtoId<RadioChannelPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>>(SecurityChannel, ref SecurityChannelTemp, hookCtx, false, context))
			{
				SecurityChannelTemp = serialization.CreateCopy<ProtoId<RadioChannelPrototype>>(SecurityChannel, hookCtx, context, false);
			}
			target.SecurityChannel = SecurityChannelTemp;
			uint MaxStringLengthTemp = 0u;
			if (!serialization.TryCustomCopy<uint>(MaxStringLength, ref MaxStringLengthTemp, hookCtx, false, context))
			{
				MaxStringLengthTemp = MaxStringLength;
			}
			target.MaxStringLength = MaxStringLengthTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CriminalRecordsConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CriminalRecordsConsoleComponent cast = (CriminalRecordsConsoleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CriminalRecordsConsoleComponent cast = (CriminalRecordsConsoleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CriminalRecordsConsoleComponent def = (CriminalRecordsConsoleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CriminalRecordsConsoleComponent Instantiate()
	{
		return new CriminalRecordsConsoleComponent();
	}
}
