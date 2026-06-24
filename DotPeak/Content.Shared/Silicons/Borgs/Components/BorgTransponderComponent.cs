// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.Components.BorgTransponderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedBorgSystem)})]
public sealed class BorgTransponderComponent : 
  Component,
  ISerializationGenerated<BorgTransponderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public SpriteSpecifier? Sprite;
  [DataField(null, false, 1, true, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public LocId DisabledPopup = (LocId) "borg-transponder-disabled-popup";
  [DataField(null, false, 1, false, false, null)]
  public LocId DisablingPopup = (LocId) "borg-transponder-disabling-popup";
  [DataField(null, false, 1, false, false, null)]
  public LocId DestroyingPopup = (LocId) "borg-transponder-destroying-popup";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan BroadcastDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextBroadcast = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan? NextDisable;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DisableDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public bool FakeDisabling;
  [DataField(null, false, 1, false, false, null)]
  public bool FakeDisabled;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BorgTransponderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BorgTransponderComponent) target1;
    if (serialization.TryCustomCopy<BorgTransponderComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier target2 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Sprite, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SpriteSpecifier>(this.Sprite, hookCtx, context);
    target.Sprite = target2;
    string target3 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target3, hookCtx, false, context))
      target3 = this.Name;
    target.Name = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.DisabledPopup, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.DisabledPopup, hookCtx, context);
    target.DisabledPopup = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.DisablingPopup, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.DisablingPopup, hookCtx, context);
    target.DisablingPopup = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.DestroyingPopup, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.DestroyingPopup, hookCtx, context);
    target.DestroyingPopup = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BroadcastDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.BroadcastDelay, hookCtx, context);
    target.BroadcastDelay = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextBroadcast, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.NextBroadcast, hookCtx, context);
    target.NextBroadcast = target8;
    TimeSpan? target9 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextDisable, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan?>(this.NextDisable, hookCtx, context);
    target.NextDisable = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DisableDelay, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.DisableDelay, hookCtx, context);
    target.DisableDelay = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.FakeDisabling, ref target11, hookCtx, false, context))
      target11 = this.FakeDisabling;
    target.FakeDisabling = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.FakeDisabled, ref target12, hookCtx, false, context))
      target12 = this.FakeDisabled;
    target.FakeDisabled = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BorgTransponderComponent target,
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
    BorgTransponderComponent target1 = (BorgTransponderComponent) target;
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
    BorgTransponderComponent target1 = (BorgTransponderComponent) target;
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
    BorgTransponderComponent target1 = (BorgTransponderComponent) target;
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
  virtual BorgTransponderComponent Component.Instantiate() => new BorgTransponderComponent();
}
