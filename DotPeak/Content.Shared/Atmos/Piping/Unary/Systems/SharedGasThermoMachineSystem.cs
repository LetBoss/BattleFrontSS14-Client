// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Unary.Systems.SharedGasThermoMachineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Atmos.Piping.Unary.Systems;

public abstract class SharedGasThermoMachineSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedPowerReceiverSystem _receiver;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasThermoMachineComponent, ExaminedEvent>(new ComponentEventHandler<GasThermoMachineComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasThermoMachineComponent, GasThermomachineToggleMessage>(new ComponentEventHandler<GasThermoMachineComponent, GasThermomachineToggleMessage>((object) this, __methodptr(OnToggleMessage)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasThermoMachineComponent, GasThermomachineChangeTemperatureMessage>(new ComponentEventHandler<GasThermoMachineComponent, GasThermomachineChangeTemperatureMessage>((object) this, __methodptr(OnChangeTemperature)), (Type[]) null, (Type[]) null);
  }

  private void OnExamined(
    EntityUid uid,
    GasThermoMachineComponent thermoMachine,
    ExaminedEvent args)
  {
    string markup;
    if (!this.Loc.TryGetString("gas-thermomachine-system-examined", ref markup, new (string, object)[3]
    {
      ("machineName", !this.IsHeater(thermoMachine) ? (object) "freezer" : (object) "heater"),
      ("tempColor", !this.IsHeater(thermoMachine) ? (object) "deepskyblue" : (object) "red"),
      ("temp", (object) Math.Round((double) thermoMachine.TargetTemperature, 2))
    }))
      return;
    args.PushMarkup(markup);
  }

  public bool IsHeater(GasThermoMachineComponent comp) => (double) comp.Cp >= 0.0;

  private void OnToggleMessage(
    EntityUid uid,
    GasThermoMachineComponent thermoMachine,
    GasThermomachineToggleMessage args)
  {
    bool flag = this._receiver.TogglePower(uid, user: new EntityUid?(((BaseBoundUserInterfaceEvent) args).Actor));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(9, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" turned ");
    logStringHandler.AppendFormatted(flag ? "On" : "Off");
    logStringHandler.AppendLiteral(" ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.AtmosPowerChanged, ref local);
    this.DirtyUI(uid, thermoMachine);
  }

  private void OnChangeTemperature(
    EntityUid uid,
    GasThermoMachineComponent thermoMachine,
    GasThermomachineChangeTemperatureMessage args)
  {
    thermoMachine.TargetTemperature = !this.IsHeater(thermoMachine) ? MathF.Max(args.Temperature, thermoMachine.MinTemperature) : MathF.Min(args.Temperature, thermoMachine.MaxTemperature);
    thermoMachine.TargetTemperature = MathF.Max(thermoMachine.TargetTemperature, 2.7f);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(24, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set temperature on ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<float>(thermoMachine.TargetTemperature, "thermoMachine.TargetTemperature");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.AtmosTemperatureChanged, ref local);
    this.Dirty(uid, (IComponent) thermoMachine, (MetaDataComponent) null);
    this.DirtyUI(uid, thermoMachine);
  }

  protected virtual void DirtyUI(
    EntityUid uid,
    GasThermoMachineComponent? thermoMachine,
    UserInterfaceComponent? ui = null)
  {
  }
}
