using System;
using Content.Shared.Random;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Mining.Components;

[RegisterComponent]
public sealed class OreVeinComponent : Component, ISerializationGenerated<OreVeinComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float OreChance = 0.1f;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<WeightedRandomOrePrototype>? OreRarityPrototypeId;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<OrePrototype>? CurrentOre;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OreVeinComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (OreVeinComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<OreVeinComponent>(this, ref target, hookCtx, false, context))
		{
			float OreChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(OreChance, ref OreChanceTemp, hookCtx, false, context))
			{
				OreChanceTemp = OreChance;
			}
			target.OreChance = OreChanceTemp;
			ProtoId<WeightedRandomOrePrototype>? OreRarityPrototypeIdTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<WeightedRandomOrePrototype>?>(OreRarityPrototypeId, ref OreRarityPrototypeIdTemp, hookCtx, false, context))
			{
				OreRarityPrototypeIdTemp = serialization.CreateCopy<ProtoId<WeightedRandomOrePrototype>?>(OreRarityPrototypeId, hookCtx, context, false);
			}
			target.OreRarityPrototypeId = OreRarityPrototypeIdTemp;
			ProtoId<OrePrototype>? CurrentOreTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<OrePrototype>?>(CurrentOre, ref CurrentOreTemp, hookCtx, false, context))
			{
				CurrentOreTemp = serialization.CreateCopy<ProtoId<OrePrototype>?>(CurrentOre, hookCtx, context, false);
			}
			target.CurrentOre = CurrentOreTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OreVeinComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OreVeinComponent cast = (OreVeinComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OreVeinComponent cast = (OreVeinComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OreVeinComponent def = (OreVeinComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OreVeinComponent Instantiate()
	{
		return new OreVeinComponent();
	}
}
