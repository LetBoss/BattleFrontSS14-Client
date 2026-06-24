// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.DamageOnAttackedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Damage.Components;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Shared.Damage.Systems;

public sealed class DamageOnAttackedSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private DamageableSystem _damageableSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageOnAttackedComponent, AttackedEvent>(new EntityEventRefHandler<DamageOnAttackedComponent, AttackedEvent>((object) this, __methodptr(OnAttacked)), (Type[]) null, (Type[]) null);
  }

  private void OnAttacked(Entity<DamageOnAttackedComponent> entity, ref AttackedEvent args)
  {
    if (!entity.Comp.IsDamageActive)
      return;
    DamageSpecifier damageSpecifier1 = entity.Comp.Damage;
    if (!entity.Comp.IgnoreResistances)
    {
      Entity<DamageOnAttackedProtectionComponent> target;
      this._inventorySystem.TryGetInventoryEntity<DamageOnAttackedProtectionComponent>(Entity<InventoryComponent>.op_Implicit(args.User), out target);
      HandsComponent handsComponent;
      EntityUid? nullable;
      DamageOnAttackedProtectionComponent protectionComponent1;
      if (target.Comp == null && this.TryComp<HandsComponent>(args.User, ref handsComponent) && this._handsSystem.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((args.User, handsComponent)), out nullable) && this.TryComp<DamageOnAttackedProtectionComponent>(nullable, ref protectionComponent1) && protectionComponent1.Slots == SlotFlags.NONE)
        target = Entity<DamageOnAttackedProtectionComponent>.op_Implicit((nullable.Value, protectionComponent1));
      DamageOnAttackedProtectionComponent protectionComponent2;
      if (target.Comp == null && this.TryComp<DamageOnAttackedProtectionComponent>(args.User, ref protectionComponent2))
        target = Entity<DamageOnAttackedProtectionComponent>.op_Implicit((args.User, protectionComponent2));
      if (target.Comp != null)
        damageSpecifier1 = DamageSpecifier.ApplyModifierSet(damageSpecifier1, target.Comp.DamageProtection);
    }
    DamageSpecifier damageSpecifier2 = this._damageableSystem.TryChangeDamage(new EntityUid?(args.User), damageSpecifier1, entity.Comp.IgnoreResistances, origin: new EntityUid?(Entity<DamageOnAttackedComponent>.op_Implicit(entity)));
    if (damageSpecifier2 == null || !damageSpecifier2.AnyPositive())
      return;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(54, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" injured themselves by attacking ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<DamageOnAttackedComponent>.op_Implicit(entity)), (MetaDataComponent) null), "target", "ToPrettyString(entity)");
    logStringHandler.AppendLiteral(" and received ");
    logStringHandler.AppendFormatted<FixedPoint2>(damageSpecifier2.GetTotal(), "damage", "totalDamage.GetTotal()");
    logStringHandler.AppendLiteral(" damage");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Damaged, ref local);
    this._audioSystem.PlayPredicted(entity.Comp.InteractSound, Entity<DamageOnAttackedComponent>.op_Implicit(entity), new EntityUid?(args.User), new AudioParams?());
    if (!entity.Comp.PopupText.HasValue)
      return;
    SharedPopupSystem popupSystem = this._popupSystem;
    ILocalizationManager loc = this.Loc;
    LocId? popupText = entity.Comp.PopupText;
    string str = popupText.HasValue ? LocId.op_Implicit(popupText.GetValueOrDefault()) : (string) null;
    string message = loc.GetString(str);
    EntityUid user = args.User;
    EntityUid? recipient = new EntityUid?(args.User);
    popupSystem.PopupClient(message, user, recipient);
  }

  public void SetIsDamageActiveTo(Entity<DamageOnAttackedComponent> entity, bool mode)
  {
    if (entity.Comp.IsDamageActive == mode)
      return;
    entity.Comp.IsDamageActive = mode;
    this.Dirty<DamageOnAttackedComponent>(entity, (MetaDataComponent) null);
  }
}
