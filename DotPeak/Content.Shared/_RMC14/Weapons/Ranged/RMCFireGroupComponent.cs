// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCFireGroupComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedFireGroupSystem)})]
public sealed class RMCFireGroupComponent : 
  Component,
  ISerializationGenerated<RMCFireGroupComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Group = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string UseDelayID = "RMCFireGroupDelay";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFireGroupComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCFireGroupComponent) target1;
    if (serialization.TryCustomCopy<RMCFireGroupComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Group == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Group, ref target2, hookCtx, false, context))
      target2 = this.Group;
    target.Group = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target3;
    string target4 = (string) null;
    if (this.UseDelayID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UseDelayID, ref target4, hookCtx, false, context))
      target4 = this.UseDelayID;
    target.UseDelayID = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFireGroupComponent target,
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
    RMCFireGroupComponent target1 = (RMCFireGroupComponent) target;
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
    RMCFireGroupComponent target1 = (RMCFireGroupComponent) target;
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
    RMCFireGroupComponent target1 = (RMCFireGroupComponent) target;
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
  virtual RMCFireGroupComponent Component.Instantiate() => new RMCFireGroupComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCFireGroupComponent_AutoState : IComponentState
  {
    public string Group;
    public TimeSpan Delay;
    public string UseDelayID;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCFireGroupComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCFireGroupComponent, ComponentGetState>(new ComponentEventRefHandler<RMCFireGroupComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCFireGroupComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCFireGroupComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCFireGroupComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCFireGroupComponent.RMCFireGroupComponent_AutoState()
      {
        Group = component.Group,
        Delay = component.Delay,
        UseDelayID = component.UseDelayID
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCFireGroupComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCFireGroupComponent.RMCFireGroupComponent_AutoState current))
        return;
      component.Group = current.Group;
      component.Delay = current.Delay;
      component.UseDelayID = current.UseDelayID;
    }
  }
}
