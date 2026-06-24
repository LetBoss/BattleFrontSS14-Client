// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Aid.XenoAidSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Aid;

public sealed class XenoAidSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private XenoEnergySystem _xenoEnergy;
  [Dependency]
  private XenoStrainSystem _xenoStrain;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoAidComponent, XenoAidActionEvent>(new EntityEventRefHandler<XenoAidComponent, XenoAidActionEvent>(this.OnXenoAidAction));
  }

  private void OnXenoAidAction(Entity<XenoAidComponent> xeno, ref XenoAidActionEvent args)
  {
    EntityUid target = args.Target;
    if (!this.HasComp<XenoComponent>(target))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-heal-sisters"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) target))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-not-same-hive"), target, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (xeno.Owner == target)
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-aid-self"), target, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (this._mobState.IsDead(target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-aid-on-fire"), target, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      switch (args.aidType)
      {
        case XenoAidMode.Healing:
          if (!this._interaction.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) target))
            break;
          if (!this._xeno.CanHeal(target))
          {
            this._popup.PopupClient(this.Loc.GetString("rmc-xeno-aid-on-fire"), target, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
            break;
          }
          if (!this._xenoEnergy.TryRemoveEnergyPopup((Entity<XenoEnergyComponent>) xeno.Owner, xeno.Comp.EnergyCost))
            break;
          args.Handled = true;
          FixedPoint2 heal = xeno.Comp.Heal;
          int? current = this.CompOrNull<XenoEnergyComponent>((EntityUid) xeno)?.Current;
          double valueOrDefault = (current.HasValue ? new double?((double) current.GetValueOrDefault() * 0.5) : new double?()).GetValueOrDefault();
          this._xenoEnergy.RemoveEnergy((Entity<XenoEnergyComponent>) xeno.Owner, (int) valueOrDefault);
          FixedPoint2 amount = !this._xenoStrain.AreSameStrain((Entity<XenoStrainComponent>) xeno.Owner, (Entity<XenoStrainComponent>) target) ? heal + (FixedPoint2) valueOrDefault : heal / 2f;
          DamageSpecifier damage1 = -this._rmcDamageable.DistributeTypesTotal((Entity<DamageableComponent>) target, amount);
          this._damageable.TryChangeDamage(new EntityUid?(target), damage1);
          DamageSpecifier damage2 = -this._rmcDamageable.DistributeTypesTotal((Entity<DamageableComponent>) xeno.Owner, xeno.Comp.Heal * 0.5 + (FixedPoint2) (valueOrDefault * 0.5));
          this._damageable.TryChangeDamage(new EntityUid?((EntityUid) xeno), damage2);
          this._popup.PopupClient(this.Loc.GetString("rmc-xeno-heal-self", ("target", (object) target)), target, new EntityUid?((EntityUid) xeno));
          this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-heal-target", ("target", (object) xeno)), target, target);
          string message1 = this.Loc.GetString("rmc-xeno-heal-others", ("user", (object) xeno), ("target", (object) target));
          Filter filter1 = Filter.Pvs(target).RemovePlayersByAttachedEntity((EntityUid) xeno, target);
          this._popup.PopupEntity(message1, target, filter1, true);
          EntProtoId? healEffect = xeno.Comp.HealEffect;
          if (healEffect.HasValue)
            this.SpawnAttachedTo((string) healEffect.GetValueOrDefault(), target.ToCoordinates(), rotation: new Angle());
          this.ActivateCooldown((EntityUid) xeno);
          break;
        case XenoAidMode.Ailments:
          if (!this._examine.InRangeUnOccluded(xeno.Owner, target, xeno.Comp.AilmentsRange) || !this._xenoEnergy.TryRemoveEnergyPopup((Entity<XenoEnergyComponent>) xeno.Owner, xeno.Comp.EnergyCost))
            break;
          foreach (ProtoId<StatusEffectPrototype> key in xeno.Comp.AilmentsRemove)
            this._statusEffects.TryRemoveStatusEffect(target, (string) key);
          this.EntityManager.RemoveComponents(target, xeno.Comp.ComponentsRemove);
          this._popup.PopupClient(this.Loc.GetString("rmc-xeno-heal-ailments-self", ("target", (object) target)), target, new EntityUid?((EntityUid) xeno));
          this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-heal-ailments-target", ("target", (object) target)), target, target);
          string message2 = this.Loc.GetString("rmc-xeno-heal-ailments-others", ("user", (object) xeno), ("target", (object) target));
          Filter filter2 = Filter.Pvs(target).RemovePlayersByAttachedEntity((EntityUid) xeno, target);
          this._popup.PopupEntity(message2, target, filter2, true);
          EntProtoId? ailmentsEffects = xeno.Comp.AilmentsEffects;
          if (ailmentsEffects.HasValue)
            this.SpawnAttachedTo((string) ailmentsEffects.GetValueOrDefault(), target.ToCoordinates(), rotation: new Angle());
          this._jitter.DoJitter(target, xeno.Comp.AilmentsJitterDuration, true, 80f, 8f, true);
          this.ActivateCooldown((EntityUid) xeno);
          break;
      }
    }
  }

  private void ActivateCooldown(EntityUid user)
  {
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoAidActionEvent>(user))
      this._actions.StartUseDelay(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)));
  }
}
