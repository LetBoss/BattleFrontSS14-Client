// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Stasis.CMStasisBagComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.Stasis;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMStasisBagSystem)})]
public sealed class CMStasisBagComponent : 
  Component,
  ISerializationGenerated<CMStasisBagComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MetabolismMultiplier = 1000;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StasisMaxTime = TimeSpan.FromMinutes(15L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StasisLeft = TimeSpan.FromMinutes(15L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId UsedBag = (EntProtoId) "RMCStasisBagUsed";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMStasisBagComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMStasisBagComponent) target1;
    if (serialization.TryCustomCopy<CMStasisBagComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MetabolismMultiplier, ref target2, hookCtx, false, context))
      target2 = this.MetabolismMultiplier;
    target.MetabolismMultiplier = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StasisMaxTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.StasisMaxTime, hookCtx, context);
    target.StasisMaxTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StasisLeft, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.StasisLeft, hookCtx, context);
    target.StasisLeft = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.UsedBag, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.UsedBag, hookCtx, context);
    target.UsedBag = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMStasisBagComponent target,
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
    CMStasisBagComponent target1 = (CMStasisBagComponent) target;
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
    CMStasisBagComponent target1 = (CMStasisBagComponent) target;
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
    CMStasisBagComponent target1 = (CMStasisBagComponent) target;
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
  virtual CMStasisBagComponent Component.Instantiate() => new CMStasisBagComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMStasisBagComponent_AutoState : IComponentState
  {
    public int MetabolismMultiplier;
    public TimeSpan StasisMaxTime;
    public TimeSpan StasisLeft;
    public EntProtoId UsedBag;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMStasisBagComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMStasisBagComponent, ComponentGetState>(new ComponentEventRefHandler<CMStasisBagComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMStasisBagComponent, ComponentHandleState>(new ComponentEventRefHandler<CMStasisBagComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMStasisBagComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMStasisBagComponent.CMStasisBagComponent_AutoState()
      {
        MetabolismMultiplier = component.MetabolismMultiplier,
        StasisMaxTime = component.StasisMaxTime,
        StasisLeft = component.StasisLeft,
        UsedBag = component.UsedBag
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMStasisBagComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMStasisBagComponent.CMStasisBagComponent_AutoState current))
        return;
      component.MetabolismMultiplier = current.MetabolismMultiplier;
      component.StasisMaxTime = current.StasisMaxTime;
      component.StasisLeft = current.StasisLeft;
      component.UsedBag = current.UsedBag;
    }
  }
}
