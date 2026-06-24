// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.Cartridges.NanoTaskCartridgeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.CartridgeLoader.Cartridges;

[RegisterComponent]
[AutoGenerateComponentPause]
public sealed class NanoTaskCartridgeComponent : 
  Component,
  ISerializationGenerated<NanoTaskCartridgeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<NanoTaskItemAndId> Tasks = new List<NanoTaskItemAndId>();
  [DataField(null, false, 1, false, false, null)]
  public int Counter = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan NextPrintAllowedAfter = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan PrintDelay = TimeSpan.FromSeconds(5L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NanoTaskCartridgeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (NanoTaskCartridgeComponent) component;
    if (serialization.TryCustomCopy<NanoTaskCartridgeComponent>(this, ref target, hookCtx, false, context))
      return;
    List<NanoTaskItemAndId> nanoTaskItemAndIdList = (List<NanoTaskItemAndId>) null;
    if (this.Tasks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<NanoTaskItemAndId>>(this.Tasks, ref nanoTaskItemAndIdList, hookCtx, true, context))
      nanoTaskItemAndIdList = serialization.CreateCopy<List<NanoTaskItemAndId>>(this.Tasks, hookCtx, context, false);
    target.Tasks = nanoTaskItemAndIdList;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Counter, ref num, hookCtx, false, context))
      num = this.Counter;
    target.Counter = num;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPrintAllowedAfter, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextPrintAllowedAfter, hookCtx, context, false);
    target.NextPrintAllowedAfter = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PrintDelay, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.PrintDelay, hookCtx, context, false);
    target.PrintDelay = timeSpan2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NanoTaskCartridgeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NanoTaskCartridgeComponent target1 = (NanoTaskCartridgeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NanoTaskCartridgeComponent target1 = (NanoTaskCartridgeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NanoTaskCartridgeComponent target1 = (NanoTaskCartridgeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NanoTaskCartridgeComponent Component.Instantiate() => new NanoTaskCartridgeComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NanoTaskCartridgeComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<NanoTaskCartridgeComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<NanoTaskCartridgeComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      NanoTaskCartridgeComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextPrintAllowedAfter += args.PausedTime;
    }
  }
}
