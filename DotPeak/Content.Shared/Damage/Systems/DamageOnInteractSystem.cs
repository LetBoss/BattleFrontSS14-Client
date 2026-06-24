// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.DamageOnInteractSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Light;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage.Components;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Damage.Systems;

public sealed class DamageOnInteractSystem : EntitySystem
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
  private ThrowingSystem _throwingSystem;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private SharedStunSystem _stun;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageOnInteractComponent, InteractHandEvent>(new EntityEventRefHandler<DamageOnInteractComponent, InteractHandEvent>((object) this, __methodptr(OnHandInteract)), (Type[]) null, (Type[]) null);
  }

  private void OnHandInteract(Entity<DamageOnInteractComponent> entity, ref InteractHandEvent args)
  {
    if (this._gameTiming.CurTime < entity.Comp.NextInteraction)
    {
      args.Handled = true;
    }
    else
    {
      if (!entity.Comp.IsDamageActive)
        return;
      DamageSpecifier damageSpecifier1 = entity.Comp.Damage;
      if (!entity.Comp.IgnoreResistances)
      {
        Entity<DamageOnInteractProtectionComponent> target;
        this._inventorySystem.TryGetInventoryEntity<DamageOnInteractProtectionComponent>(Entity<InventoryComponent>.op_Implicit(args.User), out target);
        DamageOnInteractProtectionComponent protectionComponent;
        if (target.Comp == null && this.TryComp<DamageOnInteractProtectionComponent>(args.User, ref protectionComponent))
          target = Entity<DamageOnInteractProtectionComponent>.op_Implicit((args.User, protectionComponent));
        if (target.Comp != null)
        {
          damageSpecifier1 = DamageSpecifier.ApplyModifierSet(damageSpecifier1, target.Comp.DamageProtection);
        }
        else
        {
          LightBurnHandAttemptEvent handAttemptEvent = new LightBurnHandAttemptEvent(args.User, Entity<DamageOnInteractComponent>.op_Implicit(entity));
          this.RaiseLocalEvent<LightBurnHandAttemptEvent>(ref handAttemptEvent);
          if (handAttemptEvent.Cancelled)
            return;
        }
      }
      DamageSpecifier damageSpecifier2 = this._damageableSystem.TryChangeDamage(new EntityUid?(args.User), damageSpecifier1, origin: new EntityUid?(args.Target));
      if (damageSpecifier2 != null && damageSpecifier2.AnyPositive())
      {
        entity.Comp.LastInteraction = this._gameTiming.CurTime;
        entity.Comp.NextInteraction = this._gameTiming.CurTime + TimeSpan.FromSeconds((long) entity.Comp.InteractTimer);
        args.Handled = true;
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(61, 3);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
        logStringHandler.AppendLiteral(" injured their hand by interacting with ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Target)), "target", "ToPrettyString(args.Target)");
        logStringHandler.AppendLiteral(" and received ");
        logStringHandler.AppendFormatted<FixedPoint2>(damageSpecifier2.GetTotal(), "damage", "totalDamage.GetTotal()");
        logStringHandler.AppendLiteral(" damage");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Damaged, ref local);
        this._audioSystem.PlayPredicted(entity.Comp.InteractSound, args.Target, new EntityUid?(args.User), new AudioParams?());
        if (entity.Comp.PopupText.HasValue)
        {
          SharedPopupSystem popupSystem = this._popupSystem;
          ILocalizationManager loc = this.Loc;
          LocId? popupText = entity.Comp.PopupText;
          string str = popupText.HasValue ? LocId.op_Implicit(popupText.GetValueOrDefault()) : (string) null;
          string message = loc.GetString(str);
          EntityUid user = args.User;
          EntityUid? recipient = new EntityUid?(args.User);
          popupSystem.PopupClient(message, user, recipient);
        }
        if (RandomExtensions.Prob(this._random, entity.Comp.StunChance))
          this._stun.TryParalyze(args.User, TimeSpan.FromSeconds((double) entity.Comp.StunSeconds), true);
      }
      PullableComponent pullableComponent;
      if (!entity.Comp.Throw || !this.TryComp<PullableComponent>(Entity<DamageOnInteractComponent>.op_Implicit(entity), ref pullableComponent) || pullableComponent.BeingPulled)
        return;
      this._throwingSystem.TryThrow(Entity<DamageOnInteractComponent>.op_Implicit(entity), this._random.NextVector2(1f), (float) entity.Comp.ThrowSpeed);
    }
  }

  public void SetIsDamageActiveTo(Entity<DamageOnInteractComponent> entity, bool mode)
  {
    if (entity.Comp.IsDamageActive == mode)
      return;
    entity.Comp.IsDamageActive = mode;
    this.Dirty<DamageOnInteractComponent>(entity, (MetaDataComponent) null);
  }
}
