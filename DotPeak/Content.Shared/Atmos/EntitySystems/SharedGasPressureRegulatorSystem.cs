// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedGasPressureRegulatorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasPressureRegulatorSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  protected SharedUserInterfaceSystem UserInterfaceSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressureRegulatorComponent, ExaminedEvent>(new EntityEventRefHandler<GasPressureRegulatorComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasPressureRegulatorComponent, GasPressureRegulatorChangeThresholdMessage>(new EntityEventRefHandler<GasPressureRegulatorComponent, GasPressureRegulatorChangeThresholdMessage>((object) this, __methodptr(OnThresholdChangeMessage)), (Type[]) null, (Type[]) null);
  }

  private void OnExamined(Entity<GasPressureRegulatorComponent> ent, ref ExaminedEvent args)
  {
    if (!this.Transform(Entity<GasPressureRegulatorComponent>.op_Implicit(ent)).Anchored || !args.IsInDetailsRange)
      return;
    using (args.PushGroup("GasPressureRegulatorComponent"))
    {
      args.PushMarkup(this.Loc.GetString("gas-pressure-regulator-system-examined", ("statusColor", ent.Comp.Enabled ? (object) "green" : (object) "red"), ("open", (object) ent.Comp.Enabled)));
      args.PushMarkup(this.Loc.GetString("gas-pressure-regulator-examined-threshold-pressure", ("threshold", (object) $"{ent.Comp.Threshold:0.#}")));
      args.PushMarkup(this.Loc.GetString("gas-pressure-regulator-examined-flow-rate", ("flowRate", (object) $"{ent.Comp.FlowRate:0.#}")));
    }
  }

  private void OnThresholdChangeMessage(
    Entity<GasPressureRegulatorComponent> ent,
    ref GasPressureRegulatorChangeThresholdMessage args)
  {
    ent.Comp.Threshold = Math.Max(0.0f, args.ThresholdPressure);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(35, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set the pressure threshold on ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<GasPressureRegulatorComponent>.op_Implicit(ent)), (MetaDataComponent) null), "device", "ToPrettyString(ent)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<float>(ent.Comp.Threshold, "ent.Comp.Threshold");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.AtmosVolumeChanged, LogImpact.Medium, ref local);
    this.Dirty<GasPressureRegulatorComponent>(ent, (MetaDataComponent) null);
    this.UpdateUi(ent);
  }

  protected virtual void UpdateUi(Entity<GasPressureRegulatorComponent> ent)
  {
  }
}
