// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Fob.CivFobComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Fob;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class CivFobComponent : 
  Component,
  ISerializationGenerated<CivFobComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? SideId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float OrderRange = 15f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BuildZoneRange = 15f;
  [DataField(null, false, 1, false, false, null)]
  public float TickInterval = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float CollectRadius = 5f;
  [DataField(null, false, 1, false, false, null)]
  public float CollectInterval = 15f;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> InsertBlacklist = new List<EntProtoId>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan NextTick;
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan NextCollect;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivFobComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivFobComponent) target1;
    if (serialization.TryCustomCopy<CivFobComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SideId, ref target2, hookCtx, false, context))
      target2 = this.SideId;
    target.SideId = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OrderRange, ref target3, hookCtx, false, context))
      target3 = this.OrderRange;
    target.OrderRange = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BuildZoneRange, ref target4, hookCtx, false, context))
      target4 = this.BuildZoneRange;
    target.BuildZoneRange = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TickInterval, ref target5, hookCtx, false, context))
      target5 = this.TickInterval;
    target.TickInterval = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CollectRadius, ref target6, hookCtx, false, context))
      target6 = this.CollectRadius;
    target.CollectRadius = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CollectInterval, ref target7, hookCtx, false, context))
      target7 = this.CollectInterval;
    target.CollectInterval = target7;
    List<EntProtoId> target8 = (List<EntProtoId>) null;
    if (this.InsertBlacklist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.InsertBlacklist, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<EntProtoId>>(this.InsertBlacklist, hookCtx, context);
    target.InsertBlacklist = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivFobComponent target,
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
    CivFobComponent target1 = (CivFobComponent) target;
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
    CivFobComponent target1 = (CivFobComponent) target;
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
    CivFobComponent target1 = (CivFobComponent) target;
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
  virtual CivFobComponent Component.Instantiate() => new CivFobComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivFobComponent_AutoState : IComponentState
  {
    public string? SideId;
    public float OrderRange;
    public float BuildZoneRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivFobComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivFobComponent, ComponentGetState>(new ComponentEventRefHandler<CivFobComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivFobComponent, ComponentHandleState>(new ComponentEventRefHandler<CivFobComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, CivFobComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivFobComponent.CivFobComponent_AutoState()
      {
        SideId = component.SideId,
        OrderRange = component.OrderRange,
        BuildZoneRange = component.BuildZoneRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivFobComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivFobComponent.CivFobComponent_AutoState current))
        return;
      component.SideId = current.SideId;
      component.OrderRange = current.OrderRange;
      component.BuildZoneRange = current.BuildZoneRange;
    }
  }
}
