// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Parasite.XenoParasiteThrowerComponent
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Projectile.Parasite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoParasiteThrowerComponent : 
  Component,
  ISerializationGenerated<XenoParasiteThrowerComponent>,
  ISerializationGenerated
{
  public EntProtoId ParasitePrototype = (EntProtoId) "CMXenoParasite";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ReservedParasites;
  [DataField(null, false, 1, false, false, null)]
  public float ParasiteThrowDistance = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxParasites = 16 /*0x10*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CurParasites;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ThrownParasiteStunDuration = TimeSpan.FromSeconds(7.5);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ThrownParasiteCooldown = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public int NumPositions = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool[] VisiblePositions = new bool[4];

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoParasiteThrowerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoParasiteThrowerComponent) target1;
    if (serialization.TryCustomCopy<XenoParasiteThrowerComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.ReservedParasites, ref target2, hookCtx, false, context))
      target2 = this.ReservedParasites;
    target.ReservedParasites = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ParasiteThrowDistance, ref target3, hookCtx, false, context))
      target3 = this.ParasiteThrowDistance;
    target.ParasiteThrowDistance = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxParasites, ref target4, hookCtx, false, context))
      target4 = this.MaxParasites;
    target.MaxParasites = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurParasites, ref target5, hookCtx, false, context))
      target5 = this.CurParasites;
    target.CurParasites = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ThrownParasiteStunDuration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.ThrownParasiteStunDuration, hookCtx, context);
    target.ThrownParasiteStunDuration = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ThrownParasiteCooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.ThrownParasiteCooldown, hookCtx, context);
    target.ThrownParasiteCooldown = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.NumPositions, ref target8, hookCtx, false, context))
      target8 = this.NumPositions;
    target.NumPositions = target8;
    bool[] target9 = (bool[]) null;
    if (this.VisiblePositions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<bool[]>(this.VisiblePositions, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<bool[]>(this.VisiblePositions, hookCtx, context);
    target.VisiblePositions = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoParasiteThrowerComponent target,
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
    XenoParasiteThrowerComponent target1 = (XenoParasiteThrowerComponent) target;
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
    XenoParasiteThrowerComponent target1 = (XenoParasiteThrowerComponent) target;
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
    XenoParasiteThrowerComponent target1 = (XenoParasiteThrowerComponent) target;
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
  virtual XenoParasiteThrowerComponent Component.Instantiate()
  {
    return new XenoParasiteThrowerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoParasiteThrowerComponent_AutoState : IComponentState
  {
    public int ReservedParasites;
    public int MaxParasites;
    public int CurParasites;
    public bool[] VisiblePositions;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoParasiteThrowerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoParasiteThrowerComponent, ComponentGetState>(new ComponentEventRefHandler<XenoParasiteThrowerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoParasiteThrowerComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoParasiteThrowerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoParasiteThrowerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoParasiteThrowerComponent.XenoParasiteThrowerComponent_AutoState()
      {
        ReservedParasites = component.ReservedParasites,
        MaxParasites = component.MaxParasites,
        CurParasites = component.CurParasites,
        VisiblePositions = component.VisiblePositions
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoParasiteThrowerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoParasiteThrowerComponent.XenoParasiteThrowerComponent_AutoState current))
        return;
      component.ReservedParasites = current.ReservedParasites;
      component.MaxParasites = current.MaxParasites;
      component.CurParasites = current.CurParasites;
      component.VisiblePositions = current.VisiblePositions;
    }
  }
}
