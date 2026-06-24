using System;
using System.Collections.Generic;
using Content.Shared.Access.Systems;
using Content.Shared.StationRecords;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Access.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(AccessReaderSystem) })]
public sealed class AccessReaderComponent : Component, ISerializationGenerated<AccessReaderComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Enabled = true;

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<AccessLevelPrototype>> DenyTags = new HashSet<ProtoId<AccessLevelPrototype>>();

	[DataField("access", false, 1, false, false, null)]
	public List<HashSet<ProtoId<AccessLevelPrototype>>> AccessLists = new List<HashSet<ProtoId<AccessLevelPrototype>>>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<StationRecordKey> AccessKeys = new HashSet<StationRecordKey>();

	[DataField(null, false, 1, false, false, null)]
	public string? ContainerAccessProvider;

	[DataField(null, false, 1, false, false, null)]
	public Queue<AccessRecord> AccessLog = new Queue<AccessRecord>();

	[DataField(null, false, 1, false, false, null)]
	public int AccessLogLimit = 20;

	[DataField(null, false, 1, false, false, null)]
	public bool LoggingDisabled;

	[DataField(null, false, 1, false, false, null)]
	public bool BreakOnAccessBreaker = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AccessReaderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AccessReaderComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AccessReaderComponent>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			HashSet<ProtoId<AccessLevelPrototype>> DenyTagsTemp = null;
			if (DenyTags == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(DenyTags, ref DenyTagsTemp, hookCtx, true, context))
			{
				DenyTagsTemp = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(DenyTags, hookCtx, context, false);
			}
			target.DenyTags = DenyTagsTemp;
			List<HashSet<ProtoId<AccessLevelPrototype>>> AccessListsTemp = null;
			if (AccessLists == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<HashSet<ProtoId<AccessLevelPrototype>>>>(AccessLists, ref AccessListsTemp, hookCtx, true, context))
			{
				AccessListsTemp = serialization.CreateCopy<List<HashSet<ProtoId<AccessLevelPrototype>>>>(AccessLists, hookCtx, context, false);
			}
			target.AccessLists = AccessListsTemp;
			HashSet<StationRecordKey> AccessKeysTemp = null;
			if (AccessKeys == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<StationRecordKey>>(AccessKeys, ref AccessKeysTemp, hookCtx, true, context))
			{
				AccessKeysTemp = serialization.CreateCopy<HashSet<StationRecordKey>>(AccessKeys, hookCtx, context, false);
			}
			target.AccessKeys = AccessKeysTemp;
			string ContainerAccessProviderTemp = null;
			if (!serialization.TryCustomCopy<string>(ContainerAccessProvider, ref ContainerAccessProviderTemp, hookCtx, false, context))
			{
				ContainerAccessProviderTemp = ContainerAccessProvider;
			}
			target.ContainerAccessProvider = ContainerAccessProviderTemp;
			Queue<AccessRecord> AccessLogTemp = null;
			if (AccessLog == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Queue<AccessRecord>>(AccessLog, ref AccessLogTemp, hookCtx, true, context))
			{
				AccessLogTemp = serialization.CreateCopy<Queue<AccessRecord>>(AccessLog, hookCtx, context, false);
			}
			target.AccessLog = AccessLogTemp;
			int AccessLogLimitTemp = 0;
			if (!serialization.TryCustomCopy<int>(AccessLogLimit, ref AccessLogLimitTemp, hookCtx, false, context))
			{
				AccessLogLimitTemp = AccessLogLimit;
			}
			target.AccessLogLimit = AccessLogLimitTemp;
			bool LoggingDisabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(LoggingDisabled, ref LoggingDisabledTemp, hookCtx, false, context))
			{
				LoggingDisabledTemp = LoggingDisabled;
			}
			target.LoggingDisabled = LoggingDisabledTemp;
			bool BreakOnAccessBreakerTemp = false;
			if (!serialization.TryCustomCopy<bool>(BreakOnAccessBreaker, ref BreakOnAccessBreakerTemp, hookCtx, false, context))
			{
				BreakOnAccessBreakerTemp = BreakOnAccessBreaker;
			}
			target.BreakOnAccessBreaker = BreakOnAccessBreakerTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AccessReaderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AccessReaderComponent cast = (AccessReaderComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AccessReaderComponent cast = (AccessReaderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AccessReaderComponent def = (AccessReaderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AccessReaderComponent Instantiate()
	{
		return new AccessReaderComponent();
	}
}
