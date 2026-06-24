// Decompiled with JetBrains decompiler
// Type: Content.Shared.Electrocution.SharedElectrocutionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Electrocution;

public abstract class SharedElectrocutionSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<InsulatedComponent, ElectrocutionAttemptEvent>(new ComponentEventHandler<InsulatedComponent, ElectrocutionAttemptEvent>(this.OnInsulatedElectrocutionAttempt));
    this.SubscribeLocalEvent<InsulatedComponent, InventoryRelayedEvent<ElectrocutionAttemptEvent>>((ComponentEventHandler<InsulatedComponent, InventoryRelayedEvent<ElectrocutionAttemptEvent>>) ((e, c, ev) => this.OnInsulatedElectrocutionAttempt(e, c, ev.Args)));
  }

  public void SetInsulatedSiemensCoefficient(
    EntityUid uid,
    float siemensCoefficient,
    InsulatedComponent? insulated = null)
  {
    if (!this.Resolve<InsulatedComponent>(uid, ref insulated))
      return;
    insulated.Coefficient = siemensCoefficient;
    this.Dirty(uid, (IComponent) insulated);
  }

  public void SetElectrified(Entity<ElectrifiedComponent> ent, bool value)
  {
    if (ent.Comp.Enabled == value)
      return;
    ent.Comp.Enabled = value;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
    this._appearance.SetData(ent.Owner, (Enum) ElectrifiedVisuals.IsElectrified, (object) value);
  }

  public void SetElectrifiedWireCut(Entity<ElectrifiedComponent> ent, bool value)
  {
    if (ent.Comp.IsWireCut == value)
      return;
    ent.Comp.IsWireCut = value;
    this.Dirty<ElectrifiedComponent>(ent);
  }

  public virtual bool TryDoElectrocution(
    EntityUid uid,
    EntityUid? sourceUid,
    int shockDamage,
    TimeSpan time,
    bool refresh,
    float siemensCoefficient = 1f,
    StatusEffectsComponent? statusEffects = null,
    bool ignoreInsulation = false)
  {
    return false;
  }

  private void OnInsulatedElectrocutionAttempt(
    EntityUid uid,
    InsulatedComponent insulated,
    ElectrocutionAttemptEvent args)
  {
    args.SiemensCoefficient *= insulated.Coefficient;
  }
}
