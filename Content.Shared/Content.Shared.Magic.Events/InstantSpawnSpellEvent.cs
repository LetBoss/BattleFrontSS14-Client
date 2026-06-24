using System;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Magic.Events;

public sealed class InstantSpawnSpellEvent : InstantActionEvent, ISerializationGenerated<InstantSpawnSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Prototype;

	[DataField(null, false, 1, false, false, null)]
	public bool PreventCollideWithCaster = true;

	[DataField(null, false, 1, false, false, null)]
	public MagicInstantSpawnData PosData = new TargetCasterPos();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InstantSpawnSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (InstantSpawnSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<InstantSpawnSpellEvent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId PrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
			bool PreventCollideWithCasterTemp = false;
			if (!serialization.TryCustomCopy<bool>(PreventCollideWithCaster, ref PreventCollideWithCasterTemp, hookCtx, false, context))
			{
				PreventCollideWithCasterTemp = PreventCollideWithCaster;
			}
			target.PreventCollideWithCaster = PreventCollideWithCasterTemp;
			MagicInstantSpawnData PosDataTemp = null;
			if (PosData == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<MagicInstantSpawnData>(PosData, ref PosDataTemp, hookCtx, true, context))
			{
				PosDataTemp = serialization.CreateCopy<MagicInstantSpawnData>(PosData, hookCtx, context, false);
			}
			target.PosData = PosDataTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InstantSpawnSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantSpawnSpellEvent cast = (InstantSpawnSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantSpawnSpellEvent cast = (InstantSpawnSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InstantSpawnSpellEvent Instantiate()
	{
		return new InstantSpawnSpellEvent();
	}
}
