// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Squads.SquadTeamComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Tracker.SquadLeader;
using Content.Shared.Access;
using Content.Shared.Radio;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Squads;

[RegisterComponent]
[NetworkedComponent]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SquadSystem)})]
[EntityCategory(new string[] {"Squads"})]
public sealed class SquadTeamComponent : 
  Component,
  ISerializationGenerated<SquadTeamComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool RoundStart;
  [DataField(null, false, 1, true, false, null)]
  public Color Color;
  [DataField(null, false, 1, false, false, null)]
  public Color? AccessibleColor;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<RadioChannelPrototype>? Radio;
  [DataField(null, false, 1, true, false, null)]
  public SpriteSpecifier Background;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier.Rsi? MinimapBackground;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AccessLevelPrototype>[] AccessLevels = Array.Empty<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<EntityUid> Members = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<JobPrototype>, int> Roles = new Dictionary<ProtoId<JobPrototype>, int>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<JobPrototype>, int> MaxRoles = new Dictionary<ProtoId<JobPrototype>, int>();
  [DataField(null, false, 1, false, false, null)]
  public bool CanSupplyDrop = true;
  [DataField(null, false, 1, false, false, null)]
  public List<SquadArmorLayers> BlacklistedSquadArmor = new List<SquadArmorLayers>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.Analyzers.Access(new Type[] {typeof (SquadLeaderTrackerSystem)})]
  public FireteamData Fireteams = new FireteamData();
  [DataField(null, false, 1, false, false, null)]
  public string Group = "UNMC";
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier.Rsi LeaderIcon = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/cm_job_icons.rsi"), "hudsquad_leader_a");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SquadTeamComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SquadTeamComponent) target1;
    if (serialization.TryCustomCopy<SquadTeamComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RoundStart, ref target2, hookCtx, false, context))
      target2 = this.RoundStart;
    target.RoundStart = target2;
    Color target3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target3;
    Color? target4 = new Color?();
    if (!serialization.TryCustomCopy<Color?>(this.AccessibleColor, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color?>(this.AccessibleColor, hookCtx, context);
    target.AccessibleColor = target4;
    ProtoId<RadioChannelPrototype>? target5 = new ProtoId<RadioChannelPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>?>(this.Radio, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<RadioChannelPrototype>?>(this.Radio, hookCtx, context);
    target.Radio = target5;
    SpriteSpecifier target6 = (SpriteSpecifier) null;
    if (this.Background == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Background, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SpriteSpecifier>(this.Background, hookCtx, context);
    target.Background = target6;
    SpriteSpecifier.Rsi target7 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.MinimapBackground, ref target7, hookCtx, false, context))
    {
      if (this.MinimapBackground == null)
        target7 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.MinimapBackground, ref target7, hookCtx, context);
    }
    target.MinimapBackground = target7;
    ProtoId<AccessLevelPrototype>[] target8 = (ProtoId<AccessLevelPrototype>[]) null;
    if (this.AccessLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<AccessLevelPrototype>[]>(this.AccessLevels, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<ProtoId<AccessLevelPrototype>[]>(this.AccessLevels, hookCtx, context);
    target.AccessLevels = target8;
    HashSet<EntityUid> target9 = (HashSet<EntityUid>) null;
    if (this.Members == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Members, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<HashSet<EntityUid>>(this.Members, hookCtx, context);
    target.Members = target9;
    Dictionary<ProtoId<JobPrototype>, int> target10 = (Dictionary<ProtoId<JobPrototype>, int>) null;
    if (this.Roles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, int>>(this.Roles, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, int>>(this.Roles, hookCtx, context);
    target.Roles = target10;
    Dictionary<ProtoId<JobPrototype>, int> target11 = (Dictionary<ProtoId<JobPrototype>, int>) null;
    if (this.MaxRoles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, int>>(this.MaxRoles, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, int>>(this.MaxRoles, hookCtx, context);
    target.MaxRoles = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanSupplyDrop, ref target12, hookCtx, false, context))
      target12 = this.CanSupplyDrop;
    target.CanSupplyDrop = target12;
    List<SquadArmorLayers> target13 = (List<SquadArmorLayers>) null;
    if (this.BlacklistedSquadArmor == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<SquadArmorLayers>>(this.BlacklistedSquadArmor, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<List<SquadArmorLayers>>(this.BlacklistedSquadArmor, hookCtx, context);
    target.BlacklistedSquadArmor = target13;
    FireteamData target14 = (FireteamData) null;
    if (this.Fireteams == (FireteamData) null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FireteamData>(this.Fireteams, ref target14, hookCtx, true, context))
    {
      if (this.Fireteams == (FireteamData) null)
        target14 = (FireteamData) null;
      else
        serialization.CopyTo<FireteamData>(this.Fireteams, ref target14, hookCtx, context, true);
    }
    target.Fireteams = target14;
    string target15 = (string) null;
    if (this.Group == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Group, ref target15, hookCtx, false, context))
      target15 = this.Group;
    target.Group = target15;
    SpriteSpecifier.Rsi target16 = (SpriteSpecifier.Rsi) null;
    if (this.LeaderIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.LeaderIcon, ref target16, hookCtx, false, context))
    {
      if (this.LeaderIcon == null)
        target16 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.LeaderIcon, ref target16, hookCtx, context, true);
    }
    target.LeaderIcon = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SquadTeamComponent target,
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
    SquadTeamComponent target1 = (SquadTeamComponent) target;
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
    SquadTeamComponent target1 = (SquadTeamComponent) target;
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
    SquadTeamComponent target1 = (SquadTeamComponent) target;
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
  virtual SquadTeamComponent Component.Instantiate() => new SquadTeamComponent();
}
