// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedGasPressurePumpSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasPressurePumpSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedPowerReceiverSystem _receiver;
  [Dependency]
  protected SharedUserInterfaceSystem UserInterfaceSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressurePumpComponent, ComponentInit>(new EntityEventRefHandler<GasPressurePumpComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressurePumpComponent, PowerChangedEvent>(new EntityEventRefHandler<GasPressurePumpComponent, PowerChangedEvent>((object) this, __methodptr(OnPowerChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressurePumpComponent, GasPressurePumpChangeOutputPressureMessage>(new EntityEventRefHandler<GasPressurePumpComponent, GasPressurePumpChangeOutputPressureMessage>((object) this, __methodptr(OnOutputPressureChangeMessage)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressurePumpComponent, GasPressurePumpToggleStatusMessage>(new EntityEventRefHandler<GasPressurePumpComponent, GasPressurePumpToggleStatusMessage>((object) this, __methodptr(OnToggleStatusMessage)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressurePumpComponent, AtmosDeviceDisabledEvent>(new EntityEventRefHandler<GasPressurePumpComponent, AtmosDeviceDisabledEvent>((object) this, __methodptr(OnPumpLeaveAtmosphere)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressurePumpComponent, ExaminedEvent>(new EntityEventRefHandler<GasPressurePumpComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnExamined(Entity<GasPressurePumpComponent> ent, ref ExaminedEvent args)
  {
    string markup;
    if (!this.Transform(Entity<GasPressurePumpComponent>.op_Implicit(ent)).Anchored || !this.Loc.TryGetString("gas-pressure-pump-system-examined", ref markup, ("statusColor", (object) "lightblue"), ("pressure", (object) ent.Comp.TargetPressure)))
      return;
    args.PushMarkup(markup);
  }

  private void OnInit(Entity<GasPressurePumpComponent> ent, ref ComponentInit args)
  {
    this.UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent));
  }

  private void OnPowerChanged(Entity<GasPressurePumpComponent> ent, ref PowerChangedEvent args)
  {
    this.UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent));
  }

  private void UpdateAppearance(
    Entity<GasPressurePumpComponent, AppearanceComponent?> ent)
  {
    if (!this.Resolve<AppearanceComponent>(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent), ref ent.Comp2, false))
      return;
    bool flag = ent.Comp1.Enabled && this._receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ent.Owner));
    this._appearance.SetData(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent), (Enum) PumpVisuals.Enabled, (object) flag, ent.Comp2);
  }

  private void OnToggleStatusMessage(
    Entity<GasPressurePumpComponent> ent,
    ref GasPressurePumpToggleStatusMessage args)
  {
    ent.Comp.Enabled = args.Enabled;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(22, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set the power on ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<GasPressurePumpComponent>.op_Implicit(ent)), (MetaDataComponent) null), "device", "ToPrettyString(ent)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<bool>(args.Enabled, "args.Enabled");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.AtmosPowerChanged, LogImpact.Medium, ref local);
    this.Dirty<GasPressurePumpComponent>(ent, (MetaDataComponent) null);
    this.UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent));
    this.UpdateUi(ent);
  }

  private void OnOutputPressureChangeMessage(
    Entity<GasPressurePumpComponent> ent,
    ref GasPressurePumpChangeOutputPressureMessage args)
  {
    ent.Comp.TargetPressure = Math.Clamp(args.Pressure, 0.0f, 4500f);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(28, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set the pressure on ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<GasPressurePumpComponent>.op_Implicit(ent)), (MetaDataComponent) null), "device", "ToPrettyString(ent)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<float>(args.Pressure, "args.Pressure");
    logStringHandler.AppendLiteral("kPa");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.AtmosPressureChanged, LogImpact.Medium, ref local);
    this.Dirty<GasPressurePumpComponent>(ent, (MetaDataComponent) null);
    this.UpdateUi(ent);
  }

  private void OnPumpLeaveAtmosphere(
    Entity<GasPressurePumpComponent> ent,
    ref AtmosDeviceDisabledEvent args)
  {
    ent.Comp.Enabled = false;
    this.Dirty<GasPressurePumpComponent>(ent, (MetaDataComponent) null);
    this.UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent));
    this.UserInterfaceSystem.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) GasPressurePumpUiKey.Key);
  }

  protected virtual void UpdateUi(Entity<GasPressurePumpComponent> ent)
  {
  }
}
