// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Devour.DevouredComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Devour;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoDevourSystem)})]
public sealed class DevouredComponent : 
  Component,
  ISerializationGenerated<DevouredComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan WarnAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Warned;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan RegurgitateAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimeBetweenStruggles = TimeSpan.FromSeconds(1.4);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextDevouredAttackTimeAllowed;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DevouredComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DevouredComponent) target1;
    if (serialization.TryCustomCopy<DevouredComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WarnAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.WarnAt, hookCtx, context);
    target.WarnAt = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Warned, ref target3, hookCtx, false, context))
      target3 = this.Warned;
    target.Warned = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RegurgitateAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.RegurgitateAt, hookCtx, context);
    target.RegurgitateAt = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeBetweenStruggles, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.TimeBetweenStruggles, hookCtx, context);
    target.TimeBetweenStruggles = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDevouredAttackTimeAllowed, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextDevouredAttackTimeAllowed, hookCtx, context);
    target.NextDevouredAttackTimeAllowed = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DevouredComponent target,
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
    DevouredComponent target1 = (DevouredComponent) target;
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
    DevouredComponent target1 = (DevouredComponent) target;
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
    DevouredComponent target1 = (DevouredComponent) target;
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
  virtual DevouredComponent Component.Instantiate() => new DevouredComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DevouredComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DevouredComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DevouredComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DevouredComponent component,
      ref EntityUnpausedEvent args)
    {
      component.WarnAt += args.PausedTime;
      component.RegurgitateAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DevouredComponent_AutoState : IComponentState
  {
    public TimeSpan WarnAt;
    public bool Warned;
    public TimeSpan RegurgitateAt;
    public TimeSpan TimeBetweenStruggles;
    public TimeSpan NextDevouredAttackTimeAllowed;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DevouredComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DevouredComponent, ComponentGetState>(new ComponentEventRefHandler<DevouredComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DevouredComponent, ComponentHandleState>(new ComponentEventRefHandler<DevouredComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    DevouredComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new DevouredComponent.DevouredComponent_AutoState()
      {
        WarnAt = component.WarnAt,
        Warned = component.Warned,
        RegurgitateAt = component.RegurgitateAt,
        TimeBetweenStruggles = component.TimeBetweenStruggles,
        NextDevouredAttackTimeAllowed = component.NextDevouredAttackTimeAllowed
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DevouredComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DevouredComponent.DevouredComponent_AutoState current))
        return;
      component.WarnAt = current.WarnAt;
      component.Warned = current.Warned;
      component.RegurgitateAt = current.RegurgitateAt;
      component.TimeBetweenStruggles = current.TimeBetweenStruggles;
      component.NextDevouredAttackTimeAllowed = current.NextDevouredAttackTimeAllowed;
    }
  }
}
