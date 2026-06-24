using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Repairable;

[RegisterComponent]
[Access(new Type[] { typeof(RMCRepairableSystem) })]
public sealed class NailgunRepairableComponent : Component, ISerializationGenerated<NailgunRepairableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float RepairMetal;

	[DataField(null, false, 1, false, false, null)]
	public float RepairPlasteel;

	[DataField(null, false, 1, false, false, null)]
	public float RepairWood;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NailgunRepairableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NailgunRepairableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NailgunRepairableComponent>(this, ref target, hookCtx, false, context))
		{
			float RepairMetalTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RepairMetal, ref RepairMetalTemp, hookCtx, false, context))
			{
				RepairMetalTemp = RepairMetal;
			}
			target.RepairMetal = RepairMetalTemp;
			float RepairPlasteelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RepairPlasteel, ref RepairPlasteelTemp, hookCtx, false, context))
			{
				RepairPlasteelTemp = RepairPlasteel;
			}
			target.RepairPlasteel = RepairPlasteelTemp;
			float RepairWoodTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RepairWood, ref RepairWoodTemp, hookCtx, false, context))
			{
				RepairWoodTemp = RepairWood;
			}
			target.RepairWood = RepairWoodTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NailgunRepairableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NailgunRepairableComponent cast = (NailgunRepairableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NailgunRepairableComponent cast = (NailgunRepairableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NailgunRepairableComponent def = (NailgunRepairableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NailgunRepairableComponent Instantiate()
	{
		return new NailgunRepairableComponent();
	}
}
