// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Name.XenoRankNamesComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Name;

[RegisterComponent]
[NetworkedComponent]
public sealed class XenoRankNamesComponent : 
  Component,
  ISerializationGenerated<XenoRankNamesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<int, LocId> RankNames = new Dictionary<int, LocId>()
  {
    {
      0,
      (LocId) "rmc-xeno-young"
    },
    {
      2,
      (LocId) "rmc-xeno-mature"
    },
    {
      3,
      (LocId) "rmc-xeno-elder"
    },
    {
      4,
      (LocId) "rmc-xeno-ancient"
    },
    {
      5,
      (LocId) "rmc-xeno-prime"
    },
    {
      6,
      (LocId) "rmc-xeno-prime"
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoRankNamesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoRankNamesComponent) target1;
    if (serialization.TryCustomCopy<XenoRankNamesComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<int, LocId> target2 = (Dictionary<int, LocId>) null;
    if (this.RankNames == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, LocId>>(this.RankNames, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<int, LocId>>(this.RankNames, hookCtx, context);
    target.RankNames = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoRankNamesComponent target,
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
    XenoRankNamesComponent target1 = (XenoRankNamesComponent) target;
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
    XenoRankNamesComponent target1 = (XenoRankNamesComponent) target;
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
    XenoRankNamesComponent target1 = (XenoRankNamesComponent) target;
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
  virtual XenoRankNamesComponent Component.Instantiate() => new XenoRankNamesComponent();
}
