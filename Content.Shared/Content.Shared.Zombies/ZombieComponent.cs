using System;
using System.Collections.Generic;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Zombies;

[RegisterComponent]
[NetworkedComponent]
public sealed class ZombieComponent : Component, ISerializationGenerated<ZombieComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float BaseZombieInfectionChance = 0.75f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MinZombieInfectionChance = 0.05f;

	public DamageSpecifier ResistanceEffectiveness = new DamageSpecifier
	{
		DamageDict = new Dictionary<string, FixedPoint2>
		{
			{ "Slash", 0.5 },
			{ "Piercing", 0.3 },
			{ "Blunt", 0.1 }
		}
	};

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ZombieMovementSpeedDebuff = 0.7f;

	[DataField("skinColor", false, 1, false, false, null)]
	public Color SkinColor = new Color(0.45f, 0.51f, 0.29f, 1f);

	[DataField("eyeColor", false, 1, false, false, null)]
	public Color EyeColor = new Color(0.96f, 0.13f, 0.24f, 1f);

	[DataField("baseLayerExternal", false, 1, false, false, null)]
	public string BaseLayerExternal = "MobHumanoidMarkingMatchSkin";

	[DataField("attackArc", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string AttackAnimation = "WeaponArcBite";

	[DataField("zombieRoleId", false, 1, false, false, typeof(PrototypeIdSerializer<AntagPrototype>))]
	public string ZombieRoleId = "Zombie";

	[DataField("beforeZombifiedCustomBaseLayers", false, 1, false, false, null)]
	public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> BeforeZombifiedCustomBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();

	[DataField("beforeZombifiedSkinColor", false, 1, false, false, null)]
	public Color BeforeZombifiedSkinColor;

	[DataField("beforeZombifiedEyeColor", false, 1, false, false, null)]
	public Color BeforeZombifiedEyeColor;

	[DataField("emoteId", false, 1, false, false, typeof(PrototypeIdSerializer<EmoteSoundsPrototype>))]
	public string? EmoteSoundsId = "Zombie";

	public EmoteSoundsPrototype? EmoteSounds;

	[DataField("nextTick", false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan NextTick;

	[DataField("passiveHealing", false, 1, false, false, null)]
	public DamageSpecifier PassiveHealing = new DamageSpecifier
	{
		DamageDict = new Dictionary<string, FixedPoint2>
		{
			{ "Blunt", -0.4 },
			{ "Slash", -0.2 },
			{ "Piercing", -0.2 },
			{ "Heat", -0.02 },
			{ "Shock", -0.02 }
		}
	};

	[DataField("passiveHealingCritMultiplier", false, 1, false, false, null)]
	public float PassiveHealingCritMultiplier = 2f;

	[DataField("healingOnBite", false, 1, false, false, null)]
	public DamageSpecifier HealingOnBite = new DamageSpecifier
	{
		DamageDict = new Dictionary<string, FixedPoint2>
		{
			{ "Blunt", -2 },
			{ "Slash", -2 },
			{ "Piercing", -2 }
		}
	};

	[DataField(null, false, 1, false, false, null)]
	public DamageSpecifier DamageOnBite = new DamageSpecifier
	{
		DamageDict = new Dictionary<string, FixedPoint2>
		{
			{ "Slash", 13 },
			{ "Piercing", 7 },
			{ "Structural", 10 }
		}
	};

	[DataField("greetSoundNotification", false, 1, false, false, null)]
	public SoundSpecifier GreetSoundNotification = (SoundSpecifier)new SoundPathSpecifier("/Audio/Ambience/Antag/zombie_start.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier BiteSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/bite.ogg", (AudioParams?)null);

	[DataField("beforeZombifiedBloodReagent", false, 1, false, false, null)]
	public string BeforeZombifiedBloodReagent = string.Empty;

	[DataField("newBloodReagent", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
	public string NewBloodReagent = "ZombieBlood";

	[DataField("zombieStatusIcon", false, 1, false, false, null)]
	public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = ProtoId<FactionIconPrototype>.op_Implicit("ZombieFaction");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ZombieComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ZombieComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ZombieComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		Color SkinColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(SkinColor, ref SkinColorTemp, hookCtx, false, context))
		{
			SkinColorTemp = serialization.CreateCopy<Color>(SkinColor, hookCtx, context, false);
		}
		target.SkinColor = SkinColorTemp;
		Color EyeColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(EyeColor, ref EyeColorTemp, hookCtx, false, context))
		{
			EyeColorTemp = serialization.CreateCopy<Color>(EyeColor, hookCtx, context, false);
		}
		target.EyeColor = EyeColorTemp;
		string BaseLayerExternalTemp = null;
		if (BaseLayerExternal == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(BaseLayerExternal, ref BaseLayerExternalTemp, hookCtx, false, context))
		{
			BaseLayerExternalTemp = BaseLayerExternal;
		}
		target.BaseLayerExternal = BaseLayerExternalTemp;
		string AttackAnimationTemp = null;
		if (AttackAnimation == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(AttackAnimation, ref AttackAnimationTemp, hookCtx, false, context))
		{
			AttackAnimationTemp = AttackAnimation;
		}
		target.AttackAnimation = AttackAnimationTemp;
		string ZombieRoleIdTemp = null;
		if (ZombieRoleId == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(ZombieRoleId, ref ZombieRoleIdTemp, hookCtx, false, context))
		{
			ZombieRoleIdTemp = ZombieRoleId;
		}
		target.ZombieRoleId = ZombieRoleIdTemp;
		Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> BeforeZombifiedCustomBaseLayersTemp = null;
		if (BeforeZombifiedCustomBaseLayers == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(BeforeZombifiedCustomBaseLayers, ref BeforeZombifiedCustomBaseLayersTemp, hookCtx, true, context))
		{
			BeforeZombifiedCustomBaseLayersTemp = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(BeforeZombifiedCustomBaseLayers, hookCtx, context, false);
		}
		target.BeforeZombifiedCustomBaseLayers = BeforeZombifiedCustomBaseLayersTemp;
		Color BeforeZombifiedSkinColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(BeforeZombifiedSkinColor, ref BeforeZombifiedSkinColorTemp, hookCtx, false, context))
		{
			BeforeZombifiedSkinColorTemp = serialization.CreateCopy<Color>(BeforeZombifiedSkinColor, hookCtx, context, false);
		}
		target.BeforeZombifiedSkinColor = BeforeZombifiedSkinColorTemp;
		Color BeforeZombifiedEyeColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(BeforeZombifiedEyeColor, ref BeforeZombifiedEyeColorTemp, hookCtx, false, context))
		{
			BeforeZombifiedEyeColorTemp = serialization.CreateCopy<Color>(BeforeZombifiedEyeColor, hookCtx, context, false);
		}
		target.BeforeZombifiedEyeColor = BeforeZombifiedEyeColorTemp;
		string EmoteSoundsIdTemp = null;
		if (!serialization.TryCustomCopy<string>(EmoteSoundsId, ref EmoteSoundsIdTemp, hookCtx, false, context))
		{
			EmoteSoundsIdTemp = EmoteSoundsId;
		}
		target.EmoteSoundsId = EmoteSoundsIdTemp;
		TimeSpan NextTickTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(NextTick, ref NextTickTemp, hookCtx, false, context))
		{
			NextTickTemp = serialization.CreateCopy<TimeSpan>(NextTick, hookCtx, context, false);
		}
		target.NextTick = NextTickTemp;
		ProtoId<FactionIconPrototype> StatusIconTemp = default(ProtoId<FactionIconPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<FactionIconPrototype>>(StatusIcon, ref StatusIconTemp, hookCtx, false, context))
		{
			StatusIconTemp = serialization.CreateCopy<ProtoId<FactionIconPrototype>>(StatusIcon, hookCtx, context, false);
		}
		target.StatusIcon = StatusIconTemp;
		DamageSpecifier PassiveHealingTemp = null;
		if (PassiveHealing == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(PassiveHealing, ref PassiveHealingTemp, hookCtx, false, context))
		{
			if (PassiveHealing == null)
			{
				PassiveHealingTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(PassiveHealing, ref PassiveHealingTemp, hookCtx, context, true);
			}
		}
		target.PassiveHealing = PassiveHealingTemp;
		float PassiveHealingCritMultiplierTemp = 0f;
		if (!serialization.TryCustomCopy<float>(PassiveHealingCritMultiplier, ref PassiveHealingCritMultiplierTemp, hookCtx, false, context))
		{
			PassiveHealingCritMultiplierTemp = PassiveHealingCritMultiplier;
		}
		target.PassiveHealingCritMultiplier = PassiveHealingCritMultiplierTemp;
		DamageSpecifier HealingOnBiteTemp = null;
		if (HealingOnBite == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(HealingOnBite, ref HealingOnBiteTemp, hookCtx, false, context))
		{
			if (HealingOnBite == null)
			{
				HealingOnBiteTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(HealingOnBite, ref HealingOnBiteTemp, hookCtx, context, true);
			}
		}
		target.HealingOnBite = HealingOnBiteTemp;
		DamageSpecifier DamageOnBiteTemp = null;
		if (DamageOnBite == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(DamageOnBite, ref DamageOnBiteTemp, hookCtx, false, context))
		{
			if (DamageOnBite == null)
			{
				DamageOnBiteTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(DamageOnBite, ref DamageOnBiteTemp, hookCtx, context, true);
			}
		}
		target.DamageOnBite = DamageOnBiteTemp;
		SoundSpecifier GreetSoundNotificationTemp = null;
		if (GreetSoundNotification == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(GreetSoundNotification, ref GreetSoundNotificationTemp, hookCtx, true, context))
		{
			GreetSoundNotificationTemp = serialization.CreateCopy<SoundSpecifier>(GreetSoundNotification, hookCtx, context, false);
		}
		target.GreetSoundNotification = GreetSoundNotificationTemp;
		SoundSpecifier BiteSoundTemp = null;
		if (BiteSound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(BiteSound, ref BiteSoundTemp, hookCtx, true, context))
		{
			BiteSoundTemp = serialization.CreateCopy<SoundSpecifier>(BiteSound, hookCtx, context, false);
		}
		target.BiteSound = BiteSoundTemp;
		string BeforeZombifiedBloodReagentTemp = null;
		if (BeforeZombifiedBloodReagent == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(BeforeZombifiedBloodReagent, ref BeforeZombifiedBloodReagentTemp, hookCtx, false, context))
		{
			BeforeZombifiedBloodReagentTemp = BeforeZombifiedBloodReagent;
		}
		target.BeforeZombifiedBloodReagent = BeforeZombifiedBloodReagentTemp;
		string NewBloodReagentTemp = null;
		if (NewBloodReagent == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(NewBloodReagent, ref NewBloodReagentTemp, hookCtx, false, context))
		{
			NewBloodReagentTemp = NewBloodReagent;
		}
		target.NewBloodReagent = NewBloodReagentTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ZombieComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ZombieComponent cast = (ZombieComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ZombieComponent cast = (ZombieComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ZombieComponent def = (ZombieComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ZombieComponent Instantiate()
	{
		return new ZombieComponent();
	}
}
