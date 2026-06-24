// Decompiled with JetBrains decompiler
// Type: Content.Shared.Armor.SharedArmorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Armor;

public abstract class SharedArmorSystem : EntitySystem
{
  [Dependency]
  private ExamineSystemShared _examine;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ArmorComponent, InventoryRelayedEvent<CoefficientQueryEvent>>(new EntityEventRefHandler<ArmorComponent, InventoryRelayedEvent<CoefficientQueryEvent>>((object) this, __methodptr(OnCoefficientQuery)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ArmorComponent, InventoryRelayedEvent<DamageModifyEvent>>(new ComponentEventHandler<ArmorComponent, InventoryRelayedEvent<DamageModifyEvent>>((object) this, __methodptr(OnDamageModify)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ArmorComponent, BorgModuleRelayedEvent<DamageModifyEvent>>(new ComponentEventRefHandler<ArmorComponent, BorgModuleRelayedEvent<DamageModifyEvent>>((object) this, __methodptr(OnBorgDamageModify)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ArmorComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<ArmorComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnArmorVerbExamine)), (Type[]) null, (Type[]) null);
  }

  private void OnCoefficientQuery(
    Entity<ArmorComponent> ent,
    ref InventoryRelayedEvent<CoefficientQueryEvent> args)
  {
    foreach (KeyValuePair<string, float> coefficient in ent.Comp.Modifiers.Coefficients)
    {
      float num;
      args.Args.DamageModifiers.Coefficients[coefficient.Key] = args.Args.DamageModifiers.Coefficients.TryGetValue(coefficient.Key, out num) ? num * coefficient.Value : coefficient.Value;
    }
  }

  private void OnDamageModify(
    EntityUid uid,
    ArmorComponent component,
    InventoryRelayedEvent<DamageModifyEvent> args)
  {
    args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, component.Modifiers);
  }

  private void OnBorgDamageModify(
    EntityUid uid,
    ArmorComponent component,
    ref BorgModuleRelayedEvent<DamageModifyEvent> args)
  {
    args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, component.Modifiers);
  }

  private void OnArmorVerbExamine(
    EntityUid uid,
    ArmorComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess || !component.ShowArmorOnExamine)
      return;
    FormattedMessage armorExamine = this.GetArmorExamine(component.Modifiers);
    ArmorExamineEvent armorExamineEvent = new ArmorExamineEvent(armorExamine);
    this.RaiseLocalEvent<ArmorExamineEvent>(uid, ref armorExamineEvent, false);
    this._examine.AddDetailedExamineVerb(args, (Component) component, armorExamine, this.Loc.GetString("armor-examinable-verb-text"), "/Textures/Interface/VerbIcons/dot.svg.192dpi.png", this.Loc.GetString("armor-examinable-verb-message"));
  }

  private FormattedMessage GetArmorExamine(DamageModifierSet armorModifiers)
  {
    FormattedMessage armorExamine = new FormattedMessage();
    armorExamine.AddMarkupOrThrow(this.Loc.GetString("armor-examine"));
    foreach (KeyValuePair<string, float> coefficient in armorModifiers.Coefficients)
    {
      armorExamine.PushNewline();
      string str = this.Loc.GetString("armor-damage-type-" + coefficient.Key.ToLower());
      armorExamine.AddMarkupOrThrow(this.Loc.GetString("armor-coefficient-value", ("type", (object) str), ("value", (object) MathF.Round((float) ((1.0 - (double) coefficient.Value) * 100.0), 1))));
    }
    foreach (KeyValuePair<string, float> keyValuePair in armorModifiers.FlatReduction)
    {
      armorExamine.PushNewline();
      string str = this.Loc.GetString("armor-damage-type-" + keyValuePair.Key.ToLower());
      armorExamine.AddMarkupOrThrow(this.Loc.GetString("armor-reduction-value", ("type", (object) str), ("value", (object) keyValuePair.Value)));
    }
    return armorExamine;
  }
}
