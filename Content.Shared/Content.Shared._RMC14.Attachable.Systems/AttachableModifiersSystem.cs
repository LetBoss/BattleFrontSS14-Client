using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Item;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Wieldable;
using Content.Shared._RMC14.Wieldable.Components;
using Content.Shared._RMC14.Wieldable.Events;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableModifiersSystem : EntitySystem
{
	[Dependency]
	private AttachableHolderSystem _attachableHolderSystem;

	[Dependency]
	private CMGunSystem _cmGunSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private ExamineSystemShared _examineSystem;

	[Dependency]
	private RMCSelectiveFireSystem _rmcSelectiveFireSystem;

	[Dependency]
	private RMCWieldableSystem _wieldableSystem;

	private const string modifierExamineColour = "yellow";

	private readonly Dictionary<string, FixedPoint2> _damage = new Dictionary<string, FixedPoint2>();

	[Dependency]
	private ItemSizeChangeSystem _itemSizeChangeSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableComponent, GetVerbsEvent<ExamineVerb>>((EntityEventRefHandler<AttachableComponent, GetVerbsEvent<ExamineVerb>>)OnAttachableGetExamineVerbs, (Type[])null, (Type[])null);
		InitializeMelee();
		InitializeRanged();
		InitializeSize();
		InitializeSpeed();
		InitializeWieldDelay();
	}

	private void OnAttachableGetExamineVerbs(Entity<AttachableComponent> attachable, ref GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanInteract || !args.CanAccess)
		{
			return;
		}
		AttachableGetExamineDataEvent ev = new AttachableGetExamineDataEvent(new Dictionary<byte, (AttachableModifierConditions?, List<string>)>());
		((EntitySystem)this).RaiseLocalEvent<AttachableGetExamineDataEvent>(attachable.Owner, ref ev, false);
		FormattedMessage message = new FormattedMessage();
		string text = default(string);
		foreach (byte key in ev.Data.Keys)
		{
			message.TryAddMarkup(GetExamineConditionText(attachable, ev.Data[key].conditions), ref text);
			message.PushNewline();
			foreach (string effectText in ev.Data[key].effectStrings)
			{
				message.TryAddMarkup("    " + effectText, ref text);
				message.PushNewline();
			}
		}
		if (!message.IsEmpty)
		{
			_examineSystem.AddDetailedExamineVerb(args, (Component)(object)attachable.Comp, message, base.Loc.GetString("rmc-attachable-examinable-verb-text"), "/Textures/Interface/VerbIcons/information.svg.192dpi.png", base.Loc.GetString("rmc-attachable-examinable-verb-message"));
		}
	}

	private string GetExamineConditionText(Entity<AttachableComponent> attachable, AttachableModifierConditions? conditions)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		string conditionText = base.Loc.GetString("rmc-attachable-examine-condition-always");
		if (!conditions.HasValue)
		{
			return conditionText;
		}
		AttachableModifierConditions cond = conditions.Value;
		bool conditionPlaced = false;
		conditionText = base.Loc.GetString("rmc-attachable-examine-condition-when") + " ";
		ExamineConditionAddEntry(cond.WieldedOnly, base.Loc.GetString("rmc-attachable-examine-condition-wielded"), ref conditionText, ref conditionPlaced);
		ExamineConditionAddEntry(cond.UnwieldedOnly, base.Loc.GetString("rmc-attachable-examine-condition-unwielded"), ref conditionText, ref conditionPlaced);
		ExamineConditionAddEntry(cond.ActiveOnly, base.Loc.GetString("rmc-attachable-examine-condition-active", (ValueTuple<string, object>)("attachable", attachable.Owner)), ref conditionText, ref conditionPlaced);
		ExamineConditionAddEntry(cond.InactiveOnly, base.Loc.GetString("rmc-attachable-examine-condition-inactive", (ValueTuple<string, object>)("attachable", attachable.Owner)), ref conditionText, ref conditionPlaced);
		if (cond.Whitelist != null)
		{
			EntityWhitelist whitelist = cond.Whitelist;
			if (whitelist.Registrations != null)
			{
				ExamineConditionAddEntry(cond.Whitelist != null, base.Loc.GetString("rmc-attachable-examine-condition-whitelist-comps", (ValueTuple<string, object>)("compNumber", whitelist.RequireAll ? "all" : "one"), (ValueTuple<string, object>)("comps", string.Join(", ", whitelist.Registrations))), ref conditionText, ref conditionPlaced);
			}
			if (whitelist.Sizes != null)
			{
				ExamineConditionAddEntry(cond.Whitelist != null, base.Loc.GetString("rmc-attachable-examine-condition-whitelist-sizes", (ValueTuple<string, object>)("sizes", string.Join(", ", whitelist.Sizes))), ref conditionText, ref conditionPlaced);
			}
			if (whitelist.Tags != null)
			{
				ExamineConditionAddEntry(cond.Whitelist != null, base.Loc.GetString("rmc-attachable-examine-condition-whitelist-tags", (ValueTuple<string, object>)("tagNumber", whitelist.RequireAll ? "all" : "one"), (ValueTuple<string, object>)("tags", string.Join(", ", whitelist.Tags))), ref conditionText, ref conditionPlaced);
			}
		}
		if (cond.Blacklist != null && cond.Blacklist.Tags != null)
		{
			EntityWhitelist blacklist = cond.Blacklist;
			if (blacklist.Registrations != null)
			{
				ExamineConditionAddEntry(cond.Blacklist != null, base.Loc.GetString("rmc-attachable-examine-condition-blacklist-comps", (ValueTuple<string, object>)("compNumber", blacklist.RequireAll ? "one" : "all"), (ValueTuple<string, object>)("comps", string.Join(", ", blacklist.Registrations))), ref conditionText, ref conditionPlaced);
			}
			if (blacklist.Sizes != null)
			{
				ExamineConditionAddEntry(cond.Blacklist != null, base.Loc.GetString("rmc-attachable-examine-condition-blacklist-sizes", (ValueTuple<string, object>)("sizes", string.Join(", ", blacklist.Sizes))), ref conditionText, ref conditionPlaced);
			}
			if (blacklist.Tags != null)
			{
				ExamineConditionAddEntry(cond.Blacklist != null, base.Loc.GetString("rmc-attachable-examine-condition-blacklist-tags", (ValueTuple<string, object>)("tagNumber", blacklist.RequireAll ? "one" : "all"), (ValueTuple<string, object>)("tags", string.Join(", ", blacklist.Tags))), ref conditionText, ref conditionPlaced);
			}
		}
		return conditionText + ":";
	}

	private void ExamineConditionAddEntry(bool condition, string text, ref string conditionText, ref bool conditionPlaced)
	{
		if (condition)
		{
			if (conditionPlaced)
			{
				conditionText += "; ";
			}
			conditionText += text;
			conditionPlaced = true;
		}
	}

	private byte GetExamineKey(AttachableModifierConditions? conditions)
	{
		byte key = 0;
		if (!conditions.HasValue)
		{
			return key;
		}
		key = (byte)((uint)key | (conditions.Value.WieldedOnly ? 1u : 0u));
		key = (byte)(key | (conditions.Value.UnwieldedOnly ? 2 : 0));
		key = (byte)(key | (conditions.Value.ActiveOnly ? 4 : 0));
		key = (byte)(key | (conditions.Value.InactiveOnly ? 8 : 0));
		key = (byte)(key | ((conditions.Value.Whitelist != null) ? 16 : 0));
		return (byte)(key | ((conditions.Value.Blacklist != null) ? 32 : 0));
	}

	private bool CanApplyModifiers(EntityUid attachableUid, AttachableModifierConditions? conditions)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		if (!conditions.HasValue)
		{
			return true;
		}
		_attachableHolderSystem.TryGetHolder(attachableUid, out var holderUid);
		if (holderUid.HasValue)
		{
			WieldableComponent wieldableComponent = default(WieldableComponent);
			((EntitySystem)this).TryComp<WieldableComponent>(holderUid, ref wieldableComponent);
			if (conditions.Value.UnwieldedOnly && wieldableComponent != null && wieldableComponent.Wielded)
			{
				return false;
			}
			if (conditions.Value.WieldedOnly && (wieldableComponent == null || !wieldableComponent.Wielded))
			{
				return false;
			}
		}
		AttachableToggleableComponent toggleableComponent = default(AttachableToggleableComponent);
		((EntitySystem)this).TryComp<AttachableToggleableComponent>(attachableUid, ref toggleableComponent);
		if (conditions.Value.InactiveOnly && toggleableComponent != null && toggleableComponent.Active)
		{
			return false;
		}
		if (conditions.Value.ActiveOnly && (toggleableComponent == null || !toggleableComponent.Active))
		{
			return false;
		}
		if (holderUid.HasValue)
		{
			if (conditions.Value.Whitelist != null && _whitelistSystem.IsWhitelistFail(conditions.Value.Whitelist, holderUid.Value))
			{
				return false;
			}
			if (conditions.Value.Blacklist != null && _whitelistSystem.IsWhitelistPass(conditions.Value.Blacklist, holderUid.Value))
			{
				return false;
			}
		}
		return true;
	}

	private void InitializeMelee()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponMeleeModsComponent, AttachableGetExamineDataEvent>((EntityEventRefHandler<AttachableWeaponMeleeModsComponent, AttachableGetExamineDataEvent>)OnMeleeModsGetExamineData, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponMeleeModsComponent, AttachableRelayedEvent<MeleeHitEvent>>((EntityEventRefHandler<AttachableWeaponMeleeModsComponent, AttachableRelayedEvent<MeleeHitEvent>>)OnMeleeModsHitEvent, (Type[])null, (Type[])null);
	}

	private void OnMeleeModsGetExamineData(Entity<AttachableWeaponMeleeModsComponent> attachable, ref AttachableGetExamineDataEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponMeleeModifierSet modSet in attachable.Comp.Modifiers)
		{
			byte key = GetExamineKey(modSet.Conditions);
			if (!args.Data.ContainsKey(key))
			{
				args.Data[key] = (modSet.Conditions, GetEffectStrings(modSet));
			}
			else
			{
				args.Data[key].effectStrings.AddRange(GetEffectStrings(modSet));
			}
		}
	}

	private List<string> GetEffectStrings(AttachableWeaponMeleeModifierSet modSet)
	{
		List<string> result = new List<string>();
		if (modSet.BonusDamage != null)
		{
			FixedPoint2 bonusDamage = modSet.BonusDamage.GetTotal();
			if (bonusDamage != 0)
			{
				result.Add(base.Loc.GetString("rmc-attachable-examine-melee-damage", new(string, object)[3]
				{
					("colour", "yellow"),
					("sign", (bonusDamage > 0) ? ((object)'+') : ""),
					("damage", bonusDamage)
				}));
			}
		}
		return result;
	}

	private void OnMeleeModsHitEvent(Entity<AttachableWeaponMeleeModsComponent> attachable, ref AttachableRelayedEvent<MeleeHitEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponMeleeModifierSet modSet in attachable.Comp.Modifiers)
		{
			ApplyModifierSet(attachable, modSet, ref args.Args);
		}
	}

	private void ApplyModifierSet(Entity<AttachableWeaponMeleeModsComponent> attachable, AttachableWeaponMeleeModifierSet modSet, ref MeleeHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!_attachableHolderSystem.TryGetHolder(Entity<AttachableWeaponMeleeModsComponent>.op_Implicit(attachable), out var _) || !CanApplyModifiers(attachable.Owner, modSet.Conditions))
		{
			return;
		}
		if (modSet.BonusDamage != null)
		{
			args.BonusDamage += modSet.BonusDamage;
		}
		if (!(args.BonusDamage.GetTotal() < FixedPoint2.Zero))
		{
			return;
		}
		_damage.Clear();
		string key;
		FixedPoint2 value;
		foreach (KeyValuePair<string, FixedPoint2> item in args.BonusDamage.DamageDict)
		{
			item.Deconstruct(out key, out value);
			string bonusId = key;
			FixedPoint2 bonusDmg = value;
			if (!(bonusDmg > FixedPoint2.Zero))
			{
				if (!args.BaseDamage.DamageDict.TryGetValue(bonusId, out var baseDamage))
				{
					_damage[bonusId] = -bonusDmg;
				}
				else if (-bonusDmg > baseDamage)
				{
					_damage[bonusId] = -bonusDmg - baseDamage;
				}
			}
		}
		foreach (KeyValuePair<string, FixedPoint2> item2 in _damage)
		{
			item2.Deconstruct(out key, out value);
			string bonusId2 = key;
			FixedPoint2 bonusDmg2 = value;
			args.BonusDamage.DamageDict[bonusId2] = -bonusDmg2;
		}
	}

	private void InitializeRanged()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableAlteredEvent>)OnRangedModsAltered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableGetExamineDataEvent>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableGetExamineDataEvent>)OnRangedModsGetExamineData, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetFireModesEvent>>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetFireModesEvent>>)OnRangedGetFireModes, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetFireModeValuesEvent>>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetFireModeValuesEvent>>)OnRangedModsGetFireModeValues, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetDamageFalloffEvent>>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetDamageFalloffEvent>>)OnRangedModsGetDamageFalloff, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetGunDamageModifierEvent>>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetGunDamageModifierEvent>>)OnRangedModsGetGunDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetWeaponAccuracyEvent>>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetWeaponAccuracyEvent>>)OnRangedModsGetWeaponAccuracy, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GunGetAmmoSpreadEvent>>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GunGetAmmoSpreadEvent>>)OnRangedModsGetScatterFlat, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GunRefreshModifiersEvent>>((EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GunRefreshModifiersEvent>>)OnRangedModsRefreshModifiers, (Type[])null, (Type[])null);
	}

	private void OnRangedModsGetExamineData(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableGetExamineDataEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponRangedModifierSet modSet in attachable.Comp.Modifiers)
		{
			byte key = GetExamineKey(modSet.Conditions);
			if (!args.Data.ContainsKey(key))
			{
				args.Data[key] = (modSet.Conditions, GetEffectStrings(modSet));
			}
			else
			{
				args.Data[key].effectStrings.AddRange(GetEffectStrings(modSet));
			}
		}
	}

	private List<string> GetEffectStrings(AttachableWeaponRangedModifierSet modSet)
	{
		List<string> result = new List<string>();
		if (modSet.AccuracyAddMult != 0)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-accuracy", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.AccuracyAddMult > 0) ? ((object)'+') : ""),
				("accuracy", modSet.AccuracyAddMult)
			}));
		}
		if (modSet.ScatterFlat != 0.0)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-scatter", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.ScatterFlat > 0.0) ? ((object)'+') : ""),
				("scatter", modSet.ScatterFlat)
			}));
		}
		if (modSet.BurstScatterAddMult != 0.0)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-burst-scatter", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.BurstScatterAddMult > 0.0) ? ((object)'+') : ""),
				("burstScatterMult", modSet.BurstScatterAddMult)
			}));
		}
		if (modSet.ShotsPerBurstFlat != 0)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-shots-per-burst", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.ShotsPerBurstFlat > 0) ? ((object)'+') : ""),
				("shots", modSet.ShotsPerBurstFlat)
			}));
		}
		if (modSet.FireDelayFlat != 0f)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-fire-delay", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.FireDelayFlat > 0f) ? ((object)'+') : ""),
				("fireDelay", modSet.FireDelayFlat)
			}));
		}
		if (modSet.RecoilFlat != 0f)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-recoil", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.RecoilFlat > 0f) ? ((object)'+') : ""),
				("recoil", modSet.RecoilFlat)
			}));
		}
		if (modSet.DamageAddMult != 0)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-damage", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.DamageAddMult > 0) ? ((object)'+') : ""),
				("damage", modSet.DamageAddMult)
			}));
		}
		if (modSet.ProjectileSpeedFlat != 0f)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-projectile-speed", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.ProjectileSpeedFlat > 0f) ? ((object)'+') : ""),
				("projectileSpeed", modSet.ProjectileSpeedFlat)
			}));
		}
		if (modSet.DamageFalloffAddMult != 0)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-damage-falloff", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.DamageFalloffAddMult > 0) ? ((object)'+') : ""),
				("falloff", modSet.DamageFalloffAddMult)
			}));
		}
		if (modSet.RangeFlat != 0f)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-ranged-range", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.RangeFlat > 0f) ? ((object)'+') : ""),
				("falloff", modSet.RangeFlat)
			}));
		}
		return result;
	}

	private void OnRangedModsAltered(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		AttachableAlteredType alteration = args.Alteration;
		if (alteration != AttachableAlteredType.DetachedDeactivated && alteration != AttachableAlteredType.AppearanceChanged)
		{
			_cmGunSystem.RefreshGunDamageMultiplier(Entity<GunDamageModifierComponent>.op_Implicit(args.Holder));
			if (attachable.Comp.FireModeMods != null)
			{
				_rmcSelectiveFireSystem.RefreshFireModes(Entity<RMCSelectiveFireComponent>.op_Implicit(args.Holder), forceValueRefresh: true);
			}
			_rmcSelectiveFireSystem.RefreshModifiableFireModeValues(Entity<RMCSelectiveFireComponent>.op_Implicit(args.Holder));
		}
	}

	private void OnRangedModsRefreshModifiers(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableRelayedEvent<GunRefreshModifiersEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponRangedModifierSet modSet in attachable.Comp.Modifiers)
		{
			if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
			{
				args.Args.ShotsPerBurst = Math.Max(args.Args.ShotsPerBurst + modSet.ShotsPerBurstFlat, 1);
				args.Args.CameraRecoilScalar = Math.Max(args.Args.CameraRecoilScalar + modSet.RecoilFlat, 0f);
				ref GunRefreshModifiersEvent args2 = ref args.Args;
				Angle val = args.Args.MinAngle;
				args2.MinAngle = Angle.FromDegrees(Math.Max(((Angle)(ref val)).Degrees + modSet.ScatterFlat, 0.0));
				ref GunRefreshModifiersEvent args3 = ref args.Args;
				val = args.Args.MaxAngle;
				args3.MaxAngle = Angle.FromDegrees(Math.Max(((Angle)(ref val)).Degrees + modSet.ScatterFlat, Angle.op_Implicit(args.Args.MinAngle)));
				args.Args.ProjectileSpeed += modSet.ProjectileSpeedFlat;
				float fireDelayMod = ((args.Args.Gun.Comp.SelectedMode == SelectiveFire.Burst) ? (modSet.FireDelayFlat / 2f) : modSet.FireDelayFlat);
				float fireRate = 1f / (1f / args.Args.FireRate + fireDelayMod);
				if (!float.IsInfinity(fireRate))
				{
					args.Args.FireRate = fireRate;
				}
			}
		}
	}

	private void OnRangedGetFireModes(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableRelayedEvent<GetFireModesEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.FireModeMods == null)
		{
			return;
		}
		foreach (AttachableWeaponFireModesModifierSet modSet in attachable.Comp.FireModeMods)
		{
			if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
			{
				args.Args.Modes |= modSet.ExtraFireModes;
				args.Args.Set = modSet.SetFireMode;
			}
		}
	}

	private void OnRangedModsGetDamageFalloff(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableRelayedEvent<GetDamageFalloffEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponRangedModifierSet modSet in attachable.Comp.Modifiers)
		{
			if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
			{
				args.Args.FalloffMultiplier += modSet.DamageFalloffAddMult;
				args.Args.Range += modSet.RangeFlat;
			}
		}
	}

	private void OnRangedModsGetGunDamage(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableRelayedEvent<GetGunDamageModifierEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponRangedModifierSet modSet in attachable.Comp.Modifiers)
		{
			if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
			{
				args.Args.Multiplier += modSet.DamageAddMult;
			}
		}
	}

	private void OnRangedModsGetWeaponAccuracy(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableRelayedEvent<GetWeaponAccuracyEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponRangedModifierSet modSet in attachable.Comp.Modifiers)
		{
			if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
			{
				args.Args.AccuracyMultiplier += modSet.AccuracyAddMult;
				args.Args.Range += modSet.RangeFlat;
			}
		}
	}

	private void OnRangedModsGetFireModeValues(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableRelayedEvent<GetFireModeValuesEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponRangedModifierSet modSet in attachable.Comp.Modifiers)
		{
			if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
			{
				args.Args.BurstScatterMult += modSet.BurstScatterAddMult;
			}
		}
	}

	private void OnRangedModsGetScatterFlat(Entity<AttachableWeaponRangedModsComponent> attachable, ref AttachableRelayedEvent<GunGetAmmoSpreadEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWeaponRangedModifierSet modSet in attachable.Comp.Modifiers)
		{
			if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
			{
				ref GunGetAmmoSpreadEvent args2 = ref args.Args;
				args2.Spread += Angle.op_Implicit(Angle.op_Implicit(Angle.FromDegrees(modSet.ScatterFlat)) / 2.0);
				if (Angle.op_Implicit(args.Args.Spread) < 0.0)
				{
					args.Args.Spread = Angle.op_Implicit(0f);
				}
			}
		}
	}

	private void InitializeSize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableSizeModsComponent, AttachableGetExamineDataEvent>((EntityEventRefHandler<AttachableSizeModsComponent, AttachableGetExamineDataEvent>)OnSizeModsGetExamineData, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableSizeModsComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableSizeModsComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableSizeModsComponent, AttachableRelayedEvent<GetItemSizeModifiersEvent>>((EntityEventRefHandler<AttachableSizeModsComponent, AttachableRelayedEvent<GetItemSizeModifiersEvent>>)OnGetItemSizeModifiers, (Type[])null, (Type[])null);
	}

	private void OnSizeModsGetExamineData(Entity<AttachableSizeModsComponent> attachable, ref AttachableGetExamineDataEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableSizeModifierSet modSet in attachable.Comp.Modifiers)
		{
			byte key = GetExamineKey(modSet.Conditions);
			if (!args.Data.ContainsKey(key))
			{
				args.Data[key] = (modSet.Conditions, GetEffectStrings(modSet));
			}
			else
			{
				args.Data[key].effectStrings.AddRange(GetEffectStrings(modSet));
			}
		}
	}

	private List<string> GetEffectStrings(AttachableSizeModifierSet modSet)
	{
		List<string> result = new List<string>();
		if (modSet.Size != 0)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-size", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.Size > 0) ? ((object)'+') : ""),
				("size", modSet.Size)
			}));
		}
		return result;
	}

	private void OnAttachableAltered(Entity<AttachableSizeModsComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.Modifiers.Count != 0)
		{
			AttachableAlteredType alteration = args.Alteration;
			if (alteration != AttachableAlteredType.DetachedDeactivated && alteration != AttachableAlteredType.AppearanceChanged)
			{
				_itemSizeChangeSystem.RefreshItemSizeModifiers(Entity<ItemSizeChangeComponent>.op_Implicit(args.Holder));
			}
		}
	}

	private void OnGetItemSizeModifiers(Entity<AttachableSizeModsComponent> attachable, ref AttachableRelayedEvent<GetItemSizeModifiersEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableSizeModifierSet modSet in attachable.Comp.Modifiers)
		{
			if (!CanApplyModifiers(attachable.Owner, modSet.Conditions))
			{
				break;
			}
			args.Args.Size += modSet.Size;
		}
	}

	private void InitializeSpeed()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableSpeedModsComponent, AttachableGetExamineDataEvent>((EntityEventRefHandler<AttachableSpeedModsComponent, AttachableGetExamineDataEvent>)OnSpeedModsGetExamineData, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableSpeedModsComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableSpeedModsComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableSpeedModsComponent, AttachableRelayedEvent<GetWieldableSpeedModifiersEvent>>((EntityEventRefHandler<AttachableSpeedModsComponent, AttachableRelayedEvent<GetWieldableSpeedModifiersEvent>>)OnGetSpeedModifiers, (Type[])null, (Type[])null);
	}

	private void OnSpeedModsGetExamineData(Entity<AttachableSpeedModsComponent> attachable, ref AttachableGetExamineDataEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableSpeedModifierSet modSet in attachable.Comp.Modifiers)
		{
			byte key = GetExamineKey(modSet.Conditions);
			if (!args.Data.ContainsKey(key))
			{
				args.Data[key] = (modSet.Conditions, GetEffectStrings(modSet));
			}
			else
			{
				args.Data[key].effectStrings.AddRange(GetEffectStrings(modSet));
			}
		}
	}

	private List<string> GetEffectStrings(AttachableSpeedModifierSet modSet)
	{
		List<string> result = new List<string>();
		if (modSet.Walk != 0f)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-speed-walk", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.Walk > 0f) ? ((object)'+') : ""),
				("speed", modSet.Walk)
			}));
		}
		if (modSet.Sprint != 0f)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-speed-sprint", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.Sprint > 0f) ? ((object)'+') : ""),
				("speed", modSet.Sprint)
			}));
		}
		return result;
	}

	private void OnAttachableAltered(Entity<AttachableSpeedModsComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		AttachableAlteredType alteration = args.Alteration;
		if (alteration != AttachableAlteredType.DetachedDeactivated && alteration != AttachableAlteredType.AppearanceChanged)
		{
			_wieldableSystem.RefreshSpeedModifiers(Entity<WieldableSpeedModifiersComponent>.op_Implicit(args.Holder));
		}
	}

	private void OnGetSpeedModifiers(Entity<AttachableSpeedModsComponent> attachable, ref AttachableRelayedEvent<GetWieldableSpeedModifiersEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableSpeedModifierSet modSet in attachable.Comp.Modifiers)
		{
			ApplyModifierSet(attachable, modSet, ref args.Args);
		}
	}

	private void ApplyModifierSet(Entity<AttachableSpeedModsComponent> attachable, AttachableSpeedModifierSet modSet, ref GetWieldableSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
		{
			args.Walk += modSet.Walk;
			args.Sprint += modSet.Sprint;
		}
	}

	private void InitializeWieldDelay()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableWieldDelayModsComponent, AttachableGetExamineDataEvent>((EntityEventRefHandler<AttachableWieldDelayModsComponent, AttachableGetExamineDataEvent>)OnWieldDelayModsGetExamineData, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWieldDelayModsComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableWieldDelayModsComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableWieldDelayModsComponent, AttachableRelayedEvent<GetWieldDelayEvent>>((EntityEventRefHandler<AttachableWieldDelayModsComponent, AttachableRelayedEvent<GetWieldDelayEvent>>)OnGetWieldDelay, (Type[])null, (Type[])null);
	}

	private void OnWieldDelayModsGetExamineData(Entity<AttachableWieldDelayModsComponent> attachable, ref AttachableGetExamineDataEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWieldDelayModifierSet modSet in attachable.Comp.Modifiers)
		{
			byte key = GetExamineKey(modSet.Conditions);
			if (!args.Data.ContainsKey(key))
			{
				args.Data[key] = (modSet.Conditions, GetEffectStrings(modSet));
			}
			else
			{
				args.Data[key].effectStrings.AddRange(GetEffectStrings(modSet));
			}
		}
	}

	private List<string> GetEffectStrings(AttachableWieldDelayModifierSet modSet)
	{
		List<string> result = new List<string>();
		if (modSet.Delay != TimeSpan.Zero)
		{
			result.Add(base.Loc.GetString("rmc-attachable-examine-wield-delay", new(string, object)[3]
			{
				("colour", "yellow"),
				("sign", (modSet.Delay.TotalSeconds > 0.0) ? ((object)'+') : ""),
				("delay", modSet.Delay.TotalSeconds)
			}));
		}
		return result;
	}

	private void OnAttachableAltered(Entity<AttachableWieldDelayModsComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		switch (args.Alteration)
		{
		case AttachableAlteredType.Wielded:
		case AttachableAlteredType.Unwielded:
		case AttachableAlteredType.DetachedDeactivated:
		case AttachableAlteredType.AppearanceChanged:
			return;
		}
		_wieldableSystem.RefreshWieldDelay(Entity<WieldDelayComponent>.op_Implicit(args.Holder));
	}

	private void OnGetWieldDelay(Entity<AttachableWieldDelayModsComponent> attachable, ref AttachableRelayedEvent<GetWieldDelayEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		foreach (AttachableWieldDelayModifierSet modSet in attachable.Comp.Modifiers)
		{
			ApplyModifierSet(attachable, modSet, ref args.Args);
		}
	}

	private void ApplyModifierSet(Entity<AttachableWieldDelayModsComponent> attachable, AttachableWieldDelayModifierSet modSet, ref GetWieldDelayEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (CanApplyModifiers(attachable.Owner, modSet.Conditions))
		{
			args.Delay += modSet.Delay;
		}
	}
}
