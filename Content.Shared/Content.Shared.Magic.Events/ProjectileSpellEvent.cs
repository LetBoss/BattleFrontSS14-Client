using System;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Magic.Events;

public sealed class ProjectileSpellEvent : WorldTargetActionEvent, ISerializationGenerated<ProjectileSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Prototype;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ProjectileSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		WorldTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ProjectileSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<ProjectileSpellEvent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId PrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ProjectileSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref WorldTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProjectileSpellEvent cast = (ProjectileSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProjectileSpellEvent cast = (ProjectileSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ProjectileSpellEvent Instantiate()
	{
		return new ProjectileSpellEvent();
	}
}
