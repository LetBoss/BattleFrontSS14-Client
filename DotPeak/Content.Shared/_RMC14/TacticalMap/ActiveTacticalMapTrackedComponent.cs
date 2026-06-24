// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.ActiveTacticalMapTrackedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedTacticalMapSystem)})]
public sealed class ActiveTacticalMapTrackedComponent : 
  Component,
  ISerializationGenerated<ActiveTacticalMapTrackedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Map;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier.Rsi? Icon;
  [DataField(null, false, 1, false, false, null)]
  public Color Color;
  [DataField(null, false, 1, false, false, null)]
  public bool Undefibbable;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier.Rsi? Background;
  [DataField(null, false, 1, false, false, null)]
  public bool HiveLeader;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActiveTacticalMapTrackedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActiveTacticalMapTrackedComponent) target1;
    if (serialization.TryCustomCopy<ActiveTacticalMapTrackedComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Map, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Map, hookCtx, context);
    target.Map = target2;
    SpriteSpecifier.Rsi target3 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.Icon, ref target3, hookCtx, false, context))
    {
      if (this.Icon == null)
        target3 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.Icon, ref target3, hookCtx, context);
    }
    target.Icon = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Undefibbable, ref target5, hookCtx, false, context))
      target5 = this.Undefibbable;
    target.Undefibbable = target5;
    SpriteSpecifier.Rsi target6 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.Background, ref target6, hookCtx, false, context))
    {
      if (this.Background == null)
        target6 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.Background, ref target6, hookCtx, context);
    }
    target.Background = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.HiveLeader, ref target7, hookCtx, false, context))
      target7 = this.HiveLeader;
    target.HiveLeader = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActiveTacticalMapTrackedComponent target,
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
    ActiveTacticalMapTrackedComponent target1 = (ActiveTacticalMapTrackedComponent) target;
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
    ActiveTacticalMapTrackedComponent target1 = (ActiveTacticalMapTrackedComponent) target;
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
    ActiveTacticalMapTrackedComponent target1 = (ActiveTacticalMapTrackedComponent) target;
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
  virtual ActiveTacticalMapTrackedComponent Component.Instantiate()
  {
    return new ActiveTacticalMapTrackedComponent();
  }
}
