using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Repairable;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(RMCRepairableSystem) })]
public sealed class NailgunComponent : Component, ISerializationGenerated<NailgunComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float NailingSpeed = 2f;

	[DataField(null, false, 1, false, false, null)]
	public int MaterialPerRepair = 1;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier RepairSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Weapons/nailgun_repair_long.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NailgunComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NailgunComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NailgunComponent>(this, ref target, hookCtx, false, context))
		{
			float NailingSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NailingSpeed, ref NailingSpeedTemp, hookCtx, false, context))
			{
				NailingSpeedTemp = NailingSpeed;
			}
			target.NailingSpeed = NailingSpeedTemp;
			int MaterialPerRepairTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaterialPerRepair, ref MaterialPerRepairTemp, hookCtx, false, context))
			{
				MaterialPerRepairTemp = MaterialPerRepair;
			}
			target.MaterialPerRepair = MaterialPerRepairTemp;
			SoundSpecifier RepairSoundTemp = null;
			if (RepairSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(RepairSound, ref RepairSoundTemp, hookCtx, true, context))
			{
				RepairSoundTemp = serialization.CreateCopy<SoundSpecifier>(RepairSound, hookCtx, context, false);
			}
			target.RepairSound = RepairSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NailgunComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NailgunComponent cast = (NailgunComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NailgunComponent cast = (NailgunComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NailgunComponent def = (NailgunComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NailgunComponent Instantiate()
	{
		return new NailgunComponent();
	}
}
