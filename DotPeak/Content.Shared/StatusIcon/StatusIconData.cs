// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusIcon.StatusIconData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.StatusIcon;

[Virtual]
[DataDefinition]
public class StatusIconData : 
  IComparable<StatusIconData>,
  ISerializationGenerated<StatusIconData>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public SpriteSpecifier Icon;
  [DataField(null, false, 1, false, false, null)]
  public int Priority = 10;
  [DataField(null, false, 1, false, false, null)]
  public bool VisibleToGhosts = true;
  [DataField(null, false, 1, false, false, null)]
  public bool HideInContainer = true;
  [DataField(null, false, 1, false, false, null)]
  public bool HideOnStealth = true;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? ShowTo;
  [DataField(null, false, 1, false, false, null)]
  public StatusIconLocationPreference LocationPreference;
  [DataField(null, false, 1, false, false, null)]
  public StatusIconLayer Layer;
  [DataField(null, false, 1, false, false, null)]
  public int Offset;
  [DataField(null, false, 1, false, false, null)]
  public bool IsShaded;

  public int CompareTo(StatusIconData? other)
  {
    return this.Priority.CompareTo(other != null ? other.Priority : int.MaxValue);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref StatusIconData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<StatusIconData>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier target1 = (SpriteSpecifier) null;
    if (this.Icon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Priority, ref target2, hookCtx, false, context))
      target2 = this.Priority;
    target.Priority = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.VisibleToGhosts, ref target3, hookCtx, false, context))
      target3 = this.VisibleToGhosts;
    target.VisibleToGhosts = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.HideInContainer, ref target4, hookCtx, false, context))
      target4 = this.HideInContainer;
    target.HideInContainer = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.HideOnStealth, ref target5, hookCtx, false, context))
      target5 = this.HideOnStealth;
    target.HideOnStealth = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.ShowTo, ref target6, hookCtx, false, context))
    {
      if (this.ShowTo == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.ShowTo, ref target6, hookCtx, context);
    }
    target.ShowTo = target6;
    StatusIconLocationPreference target7 = StatusIconLocationPreference.None;
    if (!serialization.TryCustomCopy<StatusIconLocationPreference>(this.LocationPreference, ref target7, hookCtx, false, context))
      target7 = this.LocationPreference;
    target.LocationPreference = target7;
    StatusIconLayer target8 = StatusIconLayer.Base;
    if (!serialization.TryCustomCopy<StatusIconLayer>(this.Layer, ref target8, hookCtx, false, context))
      target8 = this.Layer;
    target.Layer = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.Offset, ref target9, hookCtx, false, context))
      target9 = this.Offset;
    target.Offset = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsShaded, ref target10, hookCtx, false, context))
      target10 = this.IsShaded;
    target.IsShaded = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref StatusIconData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StatusIconData target1 = (StatusIconData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual StatusIconData Instantiate() => new StatusIconData();
}
