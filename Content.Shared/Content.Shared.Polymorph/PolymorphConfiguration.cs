using System;
using Robust.Shared.Audio;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Polymorph;

[DataDefinition]
public sealed record PolymorphConfiguration : ISerializationGenerated<PolymorphConfiguration>, ISerializationGenerated
{
	[DataField(null, false, 1, true, true, null)]
	public EntProtoId Entity;

	[DataField(null, false, 1, false, true, null)]
	public EntProtoId? EffectProto;

	[DataField(null, false, 1, false, true, null)]
	public int Delay = 60;

	[DataField(null, false, 1, false, true, null)]
	public int? Duration;

	[DataField(null, false, 1, false, true, null)]
	public bool Forced;

	[DataField(null, false, 1, false, true, null)]
	public bool TransferDamage = true;

	[DataField(null, false, 1, false, true, null)]
	public bool TransferName;

	[DataField(null, false, 1, false, true, null)]
	public bool TransferHumanoidAppearance;

	[DataField(null, false, 1, false, true, null)]
	public PolymorphInventoryChange Inventory;

	[DataField(null, false, 1, false, true, null)]
	public bool RevertOnCrit = true;

	[DataField(null, false, 1, false, true, null)]
	public bool RevertOnDeath = true;

	[DataField(null, false, 1, false, true, null)]
	public bool RevertOnEat;

	[DataField(null, false, 1, false, true, null)]
	public bool AllowRepeatedMorphs;

	[DataField(null, false, 1, false, true, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan Cooldown = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? PolymorphSound;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? ExitPolymorphSound;

	[DataField(null, false, 1, false, false, null)]
	public LocId? PolymorphPopup = LocId.op_Implicit("polymorph-popup-generic");

	[DataField(null, false, 1, false, false, null)]
	public LocId? ExitPolymorphPopup = LocId.op_Implicit("polymorph-revert-popup-generic");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PolymorphConfiguration target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<PolymorphConfiguration>(this, ref target, hookCtx, false, context))
		{
			EntProtoId EntityTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Entity, ref EntityTemp, hookCtx, false, context))
			{
				EntityTemp = serialization.CreateCopy<EntProtoId>(Entity, hookCtx, context, false);
			}
			target.Entity = EntityTemp;
			EntProtoId? EffectProtoTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(EffectProto, ref EffectProtoTemp, hookCtx, false, context))
			{
				EffectProtoTemp = serialization.CreateCopy<EntProtoId?>(EffectProto, hookCtx, context, false);
			}
			target.EffectProto = EffectProtoTemp;
			int DelayTemp = 0;
			if (!serialization.TryCustomCopy<int>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = Delay;
			}
			target.Delay = DelayTemp;
			int? DurationTemp = null;
			if (!serialization.TryCustomCopy<int?>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = Duration;
			}
			target.Duration = DurationTemp;
			bool ForcedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Forced, ref ForcedTemp, hookCtx, false, context))
			{
				ForcedTemp = Forced;
			}
			target.Forced = ForcedTemp;
			bool TransferDamageTemp = false;
			if (!serialization.TryCustomCopy<bool>(TransferDamage, ref TransferDamageTemp, hookCtx, false, context))
			{
				TransferDamageTemp = TransferDamage;
			}
			target.TransferDamage = TransferDamageTemp;
			bool TransferNameTemp = false;
			if (!serialization.TryCustomCopy<bool>(TransferName, ref TransferNameTemp, hookCtx, false, context))
			{
				TransferNameTemp = TransferName;
			}
			target.TransferName = TransferNameTemp;
			bool TransferHumanoidAppearanceTemp = false;
			if (!serialization.TryCustomCopy<bool>(TransferHumanoidAppearance, ref TransferHumanoidAppearanceTemp, hookCtx, false, context))
			{
				TransferHumanoidAppearanceTemp = TransferHumanoidAppearance;
			}
			target.TransferHumanoidAppearance = TransferHumanoidAppearanceTemp;
			PolymorphInventoryChange InventoryTemp = PolymorphInventoryChange.None;
			if (!serialization.TryCustomCopy<PolymorphInventoryChange>(Inventory, ref InventoryTemp, hookCtx, false, context))
			{
				InventoryTemp = Inventory;
			}
			target.Inventory = InventoryTemp;
			bool RevertOnCritTemp = false;
			if (!serialization.TryCustomCopy<bool>(RevertOnCrit, ref RevertOnCritTemp, hookCtx, false, context))
			{
				RevertOnCritTemp = RevertOnCrit;
			}
			target.RevertOnCrit = RevertOnCritTemp;
			bool RevertOnDeathTemp = false;
			if (!serialization.TryCustomCopy<bool>(RevertOnDeath, ref RevertOnDeathTemp, hookCtx, false, context))
			{
				RevertOnDeathTemp = RevertOnDeath;
			}
			target.RevertOnDeath = RevertOnDeathTemp;
			bool RevertOnEatTemp = false;
			if (!serialization.TryCustomCopy<bool>(RevertOnEat, ref RevertOnEatTemp, hookCtx, false, context))
			{
				RevertOnEatTemp = RevertOnEat;
			}
			target.RevertOnEat = RevertOnEatTemp;
			bool AllowRepeatedMorphsTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllowRepeatedMorphs, ref AllowRepeatedMorphsTemp, hookCtx, false, context))
			{
				AllowRepeatedMorphsTemp = AllowRepeatedMorphs;
			}
			target.AllowRepeatedMorphs = AllowRepeatedMorphsTemp;
			TimeSpan CooldownTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Cooldown, ref CooldownTemp, hookCtx, false, context))
			{
				CooldownTemp = serialization.CreateCopy<TimeSpan>(Cooldown, hookCtx, context, false);
			}
			target.Cooldown = CooldownTemp;
			SoundSpecifier PolymorphSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(PolymorphSound, ref PolymorphSoundTemp, hookCtx, true, context))
			{
				PolymorphSoundTemp = serialization.CreateCopy<SoundSpecifier>(PolymorphSound, hookCtx, context, false);
			}
			target.PolymorphSound = PolymorphSoundTemp;
			SoundSpecifier ExitPolymorphSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(ExitPolymorphSound, ref ExitPolymorphSoundTemp, hookCtx, true, context))
			{
				ExitPolymorphSoundTemp = serialization.CreateCopy<SoundSpecifier>(ExitPolymorphSound, hookCtx, context, false);
			}
			target.ExitPolymorphSound = ExitPolymorphSoundTemp;
			LocId? PolymorphPopupTemp = null;
			if (!serialization.TryCustomCopy<LocId?>(PolymorphPopup, ref PolymorphPopupTemp, hookCtx, false, context))
			{
				PolymorphPopupTemp = serialization.CreateCopy<LocId?>(PolymorphPopup, hookCtx, context, false);
			}
			target.PolymorphPopup = PolymorphPopupTemp;
			LocId? ExitPolymorphPopupTemp = null;
			if (!serialization.TryCustomCopy<LocId?>(ExitPolymorphPopup, ref ExitPolymorphPopupTemp, hookCtx, false, context))
			{
				ExitPolymorphPopupTemp = serialization.CreateCopy<LocId?>(ExitPolymorphPopup, hookCtx, context, false);
			}
			target.ExitPolymorphPopup = ExitPolymorphPopupTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PolymorphConfiguration target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PolymorphConfiguration cast = (PolymorphConfiguration)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PolymorphConfiguration Instantiate()
	{
		return new PolymorphConfiguration();
	}
}
