using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Body.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Medical;

[RegisterComponent]
[NetworkedComponent]
public sealed class DefibrillatorComponent : Component, ISerializationGenerated<DefibrillatorComponent>, ISerializationGenerated
{
	[DataField("zapHeal", false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public DamageSpecifier ZapHeal;

	[DataField("zapDamage", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int ZapDamage = 5;

	[DataField("writheDuration", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan WritheDuration = TimeSpan.FromSeconds(3L);

	[DataField(null, false, 1, false, false, null)]
	public string DelayId = "defib-delay";

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan ZapDelay = TimeSpan.FromSeconds(5L);

	[DataField("doAfterDuration", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(3L);

	[DataField(null, false, 1, false, false, null)]
	public bool AllowDoAfterMovement = true;

	[DataField(null, false, 1, false, false, null)]
	public bool CanDefibCrit = true;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("zapSound", false, 1, false, false, null)]
	public SoundSpecifier? ZapSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Items/Defib/defib_zap.ogg", (AudioParams?)null);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("chargeSound", false, 1, false, false, null)]
	public SoundSpecifier? ChargeSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Items/Defib/defib_charge.ogg", (AudioParams?)null);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("failureSound", false, 1, false, false, null)]
	public SoundSpecifier? FailureSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Items/Defib/defib_failed.ogg", (AudioParams?)null);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("successSound", false, 1, false, false, null)]
	public SoundSpecifier? SuccessSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Items/Defib/defib_success.ogg", (AudioParams?)null);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("readySound", false, 1, false, false, null)]
	public SoundSpecifier? ReadySound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Items/Defib/defib_ready.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? ChargeSoundEntity;

	[DataField("rmcZapHeal", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public List<(ProtoId<DamageGroupPrototype> Group, int Amount)>? RMCZapDamage;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<SkillDefinitionComponent> Skill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillMedical");

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan SkillMultiplierDuration = TimeSpan.FromSeconds(3L);

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<MetabolismGroupPrototype> MetabolismId = ProtoId<MetabolismGroupPrototype>.op_Implicit("Medicine");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DefibrillatorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DefibrillatorComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<DefibrillatorComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier ZapHealTemp = null;
		if (ZapHeal == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(ZapHeal, ref ZapHealTemp, hookCtx, false, context))
		{
			if (ZapHeal == null)
			{
				ZapHealTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(ZapHeal, ref ZapHealTemp, hookCtx, context, true);
			}
		}
		target.ZapHeal = ZapHealTemp;
		int ZapDamageTemp = 0;
		if (!serialization.TryCustomCopy<int>(ZapDamage, ref ZapDamageTemp, hookCtx, false, context))
		{
			ZapDamageTemp = ZapDamage;
		}
		target.ZapDamage = ZapDamageTemp;
		TimeSpan WritheDurationTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(WritheDuration, ref WritheDurationTemp, hookCtx, false, context))
		{
			WritheDurationTemp = serialization.CreateCopy<TimeSpan>(WritheDuration, hookCtx, context, false);
		}
		target.WritheDuration = WritheDurationTemp;
		string DelayIdTemp = null;
		if (DelayId == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(DelayId, ref DelayIdTemp, hookCtx, false, context))
		{
			DelayIdTemp = DelayId;
		}
		target.DelayId = DelayIdTemp;
		TimeSpan ZapDelayTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(ZapDelay, ref ZapDelayTemp, hookCtx, false, context))
		{
			ZapDelayTemp = serialization.CreateCopy<TimeSpan>(ZapDelay, hookCtx, context, false);
		}
		target.ZapDelay = ZapDelayTemp;
		TimeSpan DoAfterDurationTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(DoAfterDuration, ref DoAfterDurationTemp, hookCtx, false, context))
		{
			DoAfterDurationTemp = serialization.CreateCopy<TimeSpan>(DoAfterDuration, hookCtx, context, false);
		}
		target.DoAfterDuration = DoAfterDurationTemp;
		bool AllowDoAfterMovementTemp = false;
		if (!serialization.TryCustomCopy<bool>(AllowDoAfterMovement, ref AllowDoAfterMovementTemp, hookCtx, false, context))
		{
			AllowDoAfterMovementTemp = AllowDoAfterMovement;
		}
		target.AllowDoAfterMovement = AllowDoAfterMovementTemp;
		bool CanDefibCritTemp = false;
		if (!serialization.TryCustomCopy<bool>(CanDefibCrit, ref CanDefibCritTemp, hookCtx, false, context))
		{
			CanDefibCritTemp = CanDefibCrit;
		}
		target.CanDefibCrit = CanDefibCritTemp;
		SoundSpecifier ZapSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(ZapSound, ref ZapSoundTemp, hookCtx, true, context))
		{
			ZapSoundTemp = serialization.CreateCopy<SoundSpecifier>(ZapSound, hookCtx, context, false);
		}
		target.ZapSound = ZapSoundTemp;
		SoundSpecifier ChargeSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(ChargeSound, ref ChargeSoundTemp, hookCtx, true, context))
		{
			ChargeSoundTemp = serialization.CreateCopy<SoundSpecifier>(ChargeSound, hookCtx, context, false);
		}
		target.ChargeSound = ChargeSoundTemp;
		SoundSpecifier FailureSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(FailureSound, ref FailureSoundTemp, hookCtx, true, context))
		{
			FailureSoundTemp = serialization.CreateCopy<SoundSpecifier>(FailureSound, hookCtx, context, false);
		}
		target.FailureSound = FailureSoundTemp;
		SoundSpecifier SuccessSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(SuccessSound, ref SuccessSoundTemp, hookCtx, true, context))
		{
			SuccessSoundTemp = serialization.CreateCopy<SoundSpecifier>(SuccessSound, hookCtx, context, false);
		}
		target.SuccessSound = SuccessSoundTemp;
		SoundSpecifier ReadySoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(ReadySound, ref ReadySoundTemp, hookCtx, true, context))
		{
			ReadySoundTemp = serialization.CreateCopy<SoundSpecifier>(ReadySound, hookCtx, context, false);
		}
		target.ReadySound = ReadySoundTemp;
		EntityUid? ChargeSoundEntityTemp = null;
		if (!serialization.TryCustomCopy<EntityUid?>(ChargeSoundEntity, ref ChargeSoundEntityTemp, hookCtx, false, context))
		{
			ChargeSoundEntityTemp = serialization.CreateCopy<EntityUid?>(ChargeSoundEntity, hookCtx, context, false);
		}
		target.ChargeSoundEntity = ChargeSoundEntityTemp;
		List<(ProtoId<DamageGroupPrototype>, int)> RMCZapDamageTemp = null;
		if (!serialization.TryCustomCopy<List<(ProtoId<DamageGroupPrototype>, int)>>(RMCZapDamage, ref RMCZapDamageTemp, hookCtx, true, context))
		{
			RMCZapDamageTemp = serialization.CreateCopy<List<(ProtoId<DamageGroupPrototype>, int)>>(RMCZapDamage, hookCtx, context, false);
		}
		target.RMCZapDamage = RMCZapDamageTemp;
		EntProtoId<SkillDefinitionComponent> SkillTemp = default(EntProtoId<SkillDefinitionComponent>);
		if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(Skill, ref SkillTemp, hookCtx, false, context))
		{
			SkillTemp = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(Skill, hookCtx, context, false);
		}
		target.Skill = SkillTemp;
		TimeSpan SkillMultiplierDurationTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(SkillMultiplierDuration, ref SkillMultiplierDurationTemp, hookCtx, false, context))
		{
			SkillMultiplierDurationTemp = serialization.CreateCopy<TimeSpan>(SkillMultiplierDuration, hookCtx, context, false);
		}
		target.SkillMultiplierDuration = SkillMultiplierDurationTemp;
		ProtoId<MetabolismGroupPrototype> MetabolismIdTemp = default(ProtoId<MetabolismGroupPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<MetabolismGroupPrototype>>(MetabolismId, ref MetabolismIdTemp, hookCtx, false, context))
		{
			MetabolismIdTemp = serialization.CreateCopy<ProtoId<MetabolismGroupPrototype>>(MetabolismId, hookCtx, context, false);
		}
		target.MetabolismId = MetabolismIdTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DefibrillatorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DefibrillatorComponent cast = (DefibrillatorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DefibrillatorComponent cast = (DefibrillatorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DefibrillatorComponent def = (DefibrillatorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DefibrillatorComponent Instantiate()
	{
		return new DefibrillatorComponent();
	}
}
