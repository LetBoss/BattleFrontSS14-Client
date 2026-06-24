// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Supply.CivSupplyVendorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Supply;

[RegisterComponent]
public sealed class CivSupplyVendorComponent : 
  Component,
  ISerializationGenerated<CivSupplyVendorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> RestockBlacklist = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  public bool RestrictByTeam = true;
  [DataField(null, false, 1, false, false, null)]
  public CivSupplyVendorSide Side;
  [DataField(null, false, 1, false, false, null)]
  public string RequiredSideId = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string StockGroup = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public bool StartEmpty;
  [DataField(null, false, 1, false, false, null)]
  public bool RequireSquadLeader;
  [DataField(null, false, 1, false, false, null)]
  public bool RequireSquadLeaderDuringBriefing = true;
  [DataField(null, false, 1, false, false, null)]
  public string NotSquadLeaderMessage = "Только сквадной может запрашивать припасы.";
  [DataField(null, false, 1, false, false, null)]
  public string WrongTeamMessage = "Это терминал другой стороны.";
  [DataField(null, false, 1, false, false, null)]
  public float AutoCleanupRadius = 5f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AutoCleanupInterval = TimeSpan.FromSeconds(15L);
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan NextAutoCleanup;
  [DataField(null, false, 1, false, false, null)]
  public float VehicleLoadRadius = 6f;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? VehicleCargoWhitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivSupplyVendorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivSupplyVendorComponent) target1;
    if (serialization.TryCustomCopy<CivSupplyVendorComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntProtoId> target2 = (List<EntProtoId>) null;
    if (this.RestockBlacklist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.RestockBlacklist, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntProtoId>>(this.RestockBlacklist, hookCtx, context);
    target.RestockBlacklist = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.RestrictByTeam, ref target3, hookCtx, false, context))
      target3 = this.RestrictByTeam;
    target.RestrictByTeam = target3;
    CivSupplyVendorSide target4 = CivSupplyVendorSide.Attack;
    if (!serialization.TryCustomCopy<CivSupplyVendorSide>(this.Side, ref target4, hookCtx, false, context))
      target4 = this.Side;
    target.Side = target4;
    string target5 = (string) null;
    if (this.RequiredSideId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RequiredSideId, ref target5, hookCtx, false, context))
      target5 = this.RequiredSideId;
    target.RequiredSideId = target5;
    string target6 = (string) null;
    if (this.StockGroup == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StockGroup, ref target6, hookCtx, false, context))
      target6 = this.StockGroup;
    target.StockGroup = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.StartEmpty, ref target7, hookCtx, false, context))
      target7 = this.StartEmpty;
    target.StartEmpty = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireSquadLeader, ref target8, hookCtx, false, context))
      target8 = this.RequireSquadLeader;
    target.RequireSquadLeader = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireSquadLeaderDuringBriefing, ref target9, hookCtx, false, context))
      target9 = this.RequireSquadLeaderDuringBriefing;
    target.RequireSquadLeaderDuringBriefing = target9;
    string target10 = (string) null;
    if (this.NotSquadLeaderMessage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NotSquadLeaderMessage, ref target10, hookCtx, false, context))
      target10 = this.NotSquadLeaderMessage;
    target.NotSquadLeaderMessage = target10;
    string target11 = (string) null;
    if (this.WrongTeamMessage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.WrongTeamMessage, ref target11, hookCtx, false, context))
      target11 = this.WrongTeamMessage;
    target.WrongTeamMessage = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AutoCleanupRadius, ref target12, hookCtx, false, context))
      target12 = this.AutoCleanupRadius;
    target.AutoCleanupRadius = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AutoCleanupInterval, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.AutoCleanupInterval, hookCtx, context);
    target.AutoCleanupInterval = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VehicleLoadRadius, ref target14, hookCtx, false, context))
      target14 = this.VehicleLoadRadius;
    target.VehicleLoadRadius = target14;
    EntityWhitelist target15 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.VehicleCargoWhitelist, ref target15, hookCtx, false, context))
    {
      if (this.VehicleCargoWhitelist == null)
        target15 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.VehicleCargoWhitelist, ref target15, hookCtx, context);
    }
    target.VehicleCargoWhitelist = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivSupplyVendorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CivSupplyVendorComponent target1 = (CivSupplyVendorComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CivSupplyVendorComponent target1 = (CivSupplyVendorComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CivSupplyVendorComponent target1 = (CivSupplyVendorComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CivSupplyVendorComponent Component.Instantiate() => new CivSupplyVendorComponent();
}
