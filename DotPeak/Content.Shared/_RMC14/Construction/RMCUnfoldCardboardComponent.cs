// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.RMCUnfoldCardboardComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Construction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCUnfoldCardboardComponent : 
  Component,
  ISerializationGenerated<RMCUnfoldCardboardComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId VerbText = (LocId) "rmc-unfold-cardboard-component-verb";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId FailedNotEmptyText = (LocId) "rmc-unfold-cardboard-component-failed-not-empty";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntitySpawnEntry> Spawns = new List<EntitySpawnEntry>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCUnfoldCardboardComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCUnfoldCardboardComponent) target1;
    if (serialization.TryCustomCopy<RMCUnfoldCardboardComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.VerbText, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.VerbText, hookCtx, context);
    target.VerbText = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FailedNotEmptyText, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.FailedNotEmptyText, hookCtx, context);
    target.FailedNotEmptyText = target3;
    List<EntitySpawnEntry> target4 = (List<EntitySpawnEntry>) null;
    if (this.Spawns == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.Spawns, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.Spawns, hookCtx, context);
    target.Spawns = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCUnfoldCardboardComponent target,
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
    RMCUnfoldCardboardComponent target1 = (RMCUnfoldCardboardComponent) target;
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
    RMCUnfoldCardboardComponent target1 = (RMCUnfoldCardboardComponent) target;
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
    RMCUnfoldCardboardComponent target1 = (RMCUnfoldCardboardComponent) target;
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
  virtual RMCUnfoldCardboardComponent Component.Instantiate() => new RMCUnfoldCardboardComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCUnfoldCardboardComponent_AutoState : IComponentState
  {
    public LocId VerbText;
    public LocId FailedNotEmptyText;
    public List<EntitySpawnEntry> Spawns;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCUnfoldCardboardComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCUnfoldCardboardComponent, ComponentGetState>(new ComponentEventRefHandler<RMCUnfoldCardboardComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCUnfoldCardboardComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCUnfoldCardboardComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCUnfoldCardboardComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCUnfoldCardboardComponent.RMCUnfoldCardboardComponent_AutoState()
      {
        VerbText = component.VerbText,
        FailedNotEmptyText = component.FailedNotEmptyText,
        Spawns = component.Spawns
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCUnfoldCardboardComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCUnfoldCardboardComponent.RMCUnfoldCardboardComponent_AutoState current))
        return;
      component.VerbText = current.VerbText;
      component.FailedNotEmptyText = current.FailedNotEmptyText;
      component.Spawns = current.Spawns == null ? (List<EntitySpawnEntry>) null : new List<EntitySpawnEntry>((IEnumerable<EntitySpawnEntry>) current.Spawns);
    }
  }
}
