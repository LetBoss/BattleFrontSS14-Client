using System;
using System.Collections.Generic;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Roles;

[RegisterComponent]
[NetworkedComponent]
public sealed class JobGroupComponent : Component, ISerializationGenerated<JobGroupComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public LocId Name;

	[DataField(null, false, 1, true, false, null)]
	public Color Color;

	[DataField(null, false, 1, true, false, null)]
	public HashSet<ProtoId<JobPrototype>> Jobs = new HashSet<ProtoId<JobPrototype>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref JobGroupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (JobGroupComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<JobGroupComponent>(this, ref target, hookCtx, false, context))
		{
			LocId NameTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = serialization.CreateCopy<LocId>(Name, hookCtx, context, false);
			}
			target.Name = NameTemp;
			Color ColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
			}
			target.Color = ColorTemp;
			HashSet<ProtoId<JobPrototype>> JobsTemp = null;
			if (Jobs == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<JobPrototype>>>(Jobs, ref JobsTemp, hookCtx, true, context))
			{
				JobsTemp = serialization.CreateCopy<HashSet<ProtoId<JobPrototype>>>(Jobs, hookCtx, context, false);
			}
			target.Jobs = JobsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref JobGroupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobGroupComponent cast = (JobGroupComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobGroupComponent cast = (JobGroupComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		JobGroupComponent def = (JobGroupComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override JobGroupComponent Instantiate()
	{
		return new JobGroupComponent();
	}
}
