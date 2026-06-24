// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Generator.SharedPowerSwitchableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Power.Generator;

public abstract class SharedPowerSwitchableSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<PowerSwitchableComponent, ExaminedEvent>(new ComponentEventHandler<PowerSwitchableComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(EntityUid uid, PowerSwitchableComponent comp, ExaminedEvent args)
  {
    string str = this.VoltageColor(this.GetVoltage(uid, comp));
    args.PushMarkup(this.Loc.GetString(comp.ExamineText, ("voltage", (object) str)));
  }

  public string VoltageColor(SwitchableVoltage voltage)
  {
    return this.Loc.GetString("power-switchable-voltage", (nameof (voltage), (object) this.VoltageString(voltage)));
  }

  public string VoltageString(SwitchableVoltage voltage) => voltage.ToString().ToUpper();

  public int NextIndex(EntityUid uid, PowerSwitchableComponent? comp = null)
  {
    return !this.Resolve<PowerSwitchableComponent>(uid, ref comp) ? 0 : (comp.ActiveIndex + 1) % comp.Cables.Count;
  }

  public SwitchableVoltage GetVoltage(EntityUid uid, PowerSwitchableComponent? comp = null)
  {
    return !this.Resolve<PowerSwitchableComponent>(uid, ref comp) ? SwitchableVoltage.HV : comp.Cables[comp.ActiveIndex].Voltage;
  }

  public SwitchableVoltage GetNextVoltage(EntityUid uid, PowerSwitchableComponent? comp = null)
  {
    return !this.Resolve<PowerSwitchableComponent>(uid, ref comp) ? SwitchableVoltage.HV : comp.Cables[this.NextIndex(uid, comp)].Voltage;
  }
}
