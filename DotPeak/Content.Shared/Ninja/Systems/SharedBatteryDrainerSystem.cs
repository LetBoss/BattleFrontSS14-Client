// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.SharedBatteryDrainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Ninja.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public abstract class SharedBatteryDrainerSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<BatteryDrainerComponent, DoAfterAttemptEvent<DrainDoAfterEvent>>(new EntityEventRefHandler<BatteryDrainerComponent, DoAfterAttemptEvent<DrainDoAfterEvent>>(this.OnDoAfterAttempt));
    this.SubscribeLocalEvent<BatteryDrainerComponent, DrainDoAfterEvent>(new EntityEventRefHandler<BatteryDrainerComponent, DrainDoAfterEvent>(this.OnDoAfter));
  }

  protected virtual void OnDoAfterAttempt(
    Entity<BatteryDrainerComponent> ent,
    ref DoAfterAttemptEvent<DrainDoAfterEvent> args)
  {
    if (ent.Comp.BatteryUid.HasValue)
      return;
    args.Cancel();
  }

  private void OnDoAfter(Entity<BatteryDrainerComponent> ent, ref DrainDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Repeat = this.TryDrainPower(ent, valueOrDefault);
  }

  protected virtual bool TryDrainPower(Entity<BatteryDrainerComponent> ent, EntityUid target)
  {
    return true;
  }

  public void SetBattery(Entity<BatteryDrainerComponent?> ent, EntityUid? battery)
  {
    if (!this.Resolve<BatteryDrainerComponent>((EntityUid) ent, ref ent.Comp))
      return;
    EntityUid? batteryUid = ent.Comp.BatteryUid;
    EntityUid? nullable = battery;
    if ((batteryUid.HasValue == nullable.HasValue ? (batteryUid.HasValue ? (batteryUid.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    ent.Comp.BatteryUid = battery;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }
}
