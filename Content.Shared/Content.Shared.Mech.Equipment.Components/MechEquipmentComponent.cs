using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Mech.Equipment.Components;

[RegisterComponent]
public sealed class MechEquipmentComponent : Component, ISerializationGenerated<MechEquipmentComponent>, ISerializationGenerated
{
	[DataField("installDuration", false, 1, false, false, null)]
	public float InstallDuration = 5f;

	[ViewVariables]
	public EntityUid? EquipmentOwner;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MechEquipmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MechEquipmentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MechEquipmentComponent>(this, ref target, hookCtx, false, context))
		{
			float InstallDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InstallDuration, ref InstallDurationTemp, hookCtx, false, context))
			{
				InstallDurationTemp = InstallDuration;
			}
			target.InstallDuration = InstallDurationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MechEquipmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechEquipmentComponent cast = (MechEquipmentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechEquipmentComponent cast = (MechEquipmentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechEquipmentComponent def = (MechEquipmentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MechEquipmentComponent Instantiate()
	{
		return new MechEquipmentComponent();
	}
}
