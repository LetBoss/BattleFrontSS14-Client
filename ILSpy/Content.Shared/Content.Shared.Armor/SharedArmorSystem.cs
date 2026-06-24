using System;
using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Shared.Armor;

public abstract class SharedArmorSystem : EntitySystem
{
	[Dependency]
	private ExamineSystemShared _examine;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ArmorComponent, InventoryRelayedEvent<CoefficientQueryEvent>>((EntityEventRefHandler<ArmorComponent, InventoryRelayedEvent<CoefficientQueryEvent>>)OnCoefficientQuery, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArmorComponent, InventoryRelayedEvent<DamageModifyEvent>>((ComponentEventHandler<ArmorComponent, InventoryRelayedEvent<DamageModifyEvent>>)OnDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArmorComponent, BorgModuleRelayedEvent<DamageModifyEvent>>((ComponentEventRefHandler<ArmorComponent, BorgModuleRelayedEvent<DamageModifyEvent>>)OnBorgDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArmorComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<ArmorComponent, GetVerbsEvent<ExamineVerb>>)OnArmorVerbExamine, (Type[])null, (Type[])null);
	}

	private void OnCoefficientQuery(Entity<ArmorComponent> ent, ref InventoryRelayedEvent<CoefficientQueryEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, float> armorCoefficient in ent.Comp.Modifiers.Coefficients)
		{
			args.Args.DamageModifiers.Coefficients[armorCoefficient.Key] = (args.Args.DamageModifiers.Coefficients.TryGetValue(armorCoefficient.Key, out var coefficient) ? (coefficient * armorCoefficient.Value) : armorCoefficient.Value);
		}
	}

	private void OnDamageModify(EntityUid uid, ArmorComponent component, InventoryRelayedEvent<DamageModifyEvent> args)
	{
		args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, component.Modifiers);
	}

	private void OnBorgDamageModify(EntityUid uid, ArmorComponent component, ref BorgModuleRelayedEvent<DamageModifyEvent> args)
	{
		args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, component.Modifiers);
	}

	private void OnArmorVerbExamine(EntityUid uid, ArmorComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanInteract && args.CanAccess && component.ShowArmorOnExamine)
		{
			FormattedMessage examineMarkup = GetArmorExamine(component.Modifiers);
			ArmorExamineEvent ev = new ArmorExamineEvent(examineMarkup);
			((EntitySystem)this).RaiseLocalEvent<ArmorExamineEvent>(uid, ref ev, false);
			_examine.AddDetailedExamineVerb(args, (Component)(object)component, examineMarkup, base.Loc.GetString("armor-examinable-verb-text"), "/Textures/Interface/VerbIcons/dot.svg.192dpi.png", base.Loc.GetString("armor-examinable-verb-message"));
		}
	}

	private FormattedMessage GetArmorExamine(DamageModifierSet armorModifiers)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		FormattedMessage msg = new FormattedMessage();
		msg.AddMarkupOrThrow(base.Loc.GetString("armor-examine"));
		foreach (KeyValuePair<string, float> coefficientArmor in armorModifiers.Coefficients)
		{
			msg.PushNewline();
			string armorType = base.Loc.GetString("armor-damage-type-" + coefficientArmor.Key.ToLower());
			msg.AddMarkupOrThrow(base.Loc.GetString("armor-coefficient-value", (ValueTuple<string, object>)("type", armorType), (ValueTuple<string, object>)("value", MathF.Round((1f - coefficientArmor.Value) * 100f, 1))));
		}
		foreach (KeyValuePair<string, float> flatArmor in armorModifiers.FlatReduction)
		{
			msg.PushNewline();
			string armorType2 = base.Loc.GetString("armor-damage-type-" + flatArmor.Key.ToLower());
			msg.AddMarkupOrThrow(base.Loc.GetString("armor-reduction-value", (ValueTuple<string, object>)("type", armorType2), (ValueTuple<string, object>)("value", flatArmor.Value)));
		}
		return msg;
	}
}
