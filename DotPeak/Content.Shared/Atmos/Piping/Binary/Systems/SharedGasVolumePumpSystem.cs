// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Binary.Systems.SharedGasVolumePumpSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Visuals;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Atmos.Piping.Binary.Systems;

public abstract class SharedGasVolumePumpSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedPowerReceiverSystem _receiver;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasVolumePumpComponent, ComponentInit>(new EntityEventRefHandler<GasVolumePumpComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasVolumePumpComponent, PowerChangedEvent>(new EntityEventRefHandler<GasVolumePumpComponent, PowerChangedEvent>((object) this, __methodptr(OnPowerChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasVolumePumpComponent, ExaminedEvent>(new ComponentEventHandler<GasVolumePumpComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasVolumePumpComponent, GasVolumePumpToggleStatusMessage>(new ComponentEventHandler<GasVolumePumpComponent, GasVolumePumpToggleStatusMessage>((object) this, __methodptr(OnToggleStatusMessage)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasVolumePumpComponent, GasVolumePumpChangeTransferRateMessage>(new ComponentEventHandler<GasVolumePumpComponent, GasVolumePumpChangeTransferRateMessage>((object) this, __methodptr(OnTransferRateChangeMessage)), (Type[]) null, (Type[]) null);
  }

  private void OnInit(Entity<GasVolumePumpComponent> ent, ref ComponentInit args)
  {
    this.UpdateAppearance(ent.Owner, ent.Comp);
  }

  private void OnPowerChanged(Entity<GasVolumePumpComponent> ent, ref PowerChangedEvent args)
  {
    this.UpdateAppearance(ent.Owner, ent.Comp);
  }

  protected virtual void UpdateUi(Entity<GasVolumePumpComponent> entity)
  {
  }

  private void OnToggleStatusMessage(
    EntityUid uid,
    GasVolumePumpComponent pump,
    GasVolumePumpToggleStatusMessage args)
  {
    pump.Enabled = args.Enabled;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(22, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set the power on ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "device", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<bool>(args.Enabled, "args.Enabled");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.AtmosPowerChanged, LogImpact.Medium, ref local);
    this.Dirty(uid, (IComponent) pump, (MetaDataComponent) null);
    this.UpdateUi(Entity<GasVolumePumpComponent>.op_Implicit((uid, pump)));
    this.UpdateAppearance(uid, pump);
  }

  private void OnTransferRateChangeMessage(
    EntityUid uid,
    GasVolumePumpComponent pump,
    GasVolumePumpChangeTransferRateMessage args)
  {
    pump.TransferRate = Math.Clamp(args.TransferRate, 0.0f, pump.MaxTransferRate);
    this.Dirty(uid, (IComponent) pump, (MetaDataComponent) null);
    this.UpdateUi(Entity<GasVolumePumpComponent>.op_Implicit((uid, pump)));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(30, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set the transfer rate on ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "device", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<float>(args.TransferRate, "args.TransferRate");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.AtmosVolumeChanged, LogImpact.Medium, ref local);
  }

  private void OnExamined(EntityUid uid, GasVolumePumpComponent pump, ExaminedEvent args)
  {
    string markup;
    if (!this.Transform(uid).Anchored || !this.Loc.TryGetString("gas-volume-pump-system-examined", ref markup, ("statusColor", (object) "lightblue"), ("rate", (object) pump.TransferRate)))
      return;
    args.PushMarkup(markup);
  }

  protected void UpdateAppearance(
    EntityUid uid,
    GasVolumePumpComponent? pump = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<GasVolumePumpComponent, AppearanceComponent>(uid, ref pump, ref appearance, false))
      return;
    if ((!pump.Enabled ? 0 : (this._receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)) ? 1 : 0)) == 0)
      this._appearance.SetData(uid, (Enum) GasVolumePumpVisuals.State, (object) GasVolumePumpState.Off, appearance);
    else if (pump.Blocked)
      this._appearance.SetData(uid, (Enum) GasVolumePumpVisuals.State, (object) GasVolumePumpState.Blocked, appearance);
    else
      this._appearance.SetData(uid, (Enum) GasVolumePumpVisuals.State, (object) GasVolumePumpState.On, appearance);
  }
}
