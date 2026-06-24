// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Doom.LightDoomedComponent
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
namespace Content.Shared._RMC14.Xenonids.Doom;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class LightDoomedComponent : 
  Component,
  ISerializationGenerated<LightDoomedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? EndsAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool WasEnabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DoomActivated;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LightDoomedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LightDoomedComponent) target1;
    if (serialization.TryCustomCopy<LightDoomedComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EndsAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.EndsAt, hookCtx, context);
    target.EndsAt = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.WasEnabled, ref target4, hookCtx, false, context))
      target4 = this.WasEnabled;
    target.WasEnabled = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoomActivated, ref target5, hookCtx, false, context))
      target5 = this.DoomActivated;
    target.DoomActivated = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LightDoomedComponent target,
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
    LightDoomedComponent target1 = (LightDoomedComponent) target;
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
    LightDoomedComponent target1 = (LightDoomedComponent) target;
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
    LightDoomedComponent target1 = (LightDoomedComponent) target;
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
  virtual LightDoomedComponent Component.Instantiate() => new LightDoomedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class LightDoomedComponent_AutoState : IComponentState
  {
    public TimeSpan Duration;
    public TimeSpan? EndsAt;
    public bool WasEnabled;
    public bool DoomActivated;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LightDoomedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<LightDoomedComponent, ComponentGetState>(new ComponentEventRefHandler<LightDoomedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<LightDoomedComponent, ComponentHandleState>(new ComponentEventRefHandler<LightDoomedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      LightDoomedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new LightDoomedComponent.LightDoomedComponent_AutoState()
      {
        Duration = component.Duration,
        EndsAt = component.EndsAt,
        WasEnabled = component.WasEnabled,
        DoomActivated = component.DoomActivated
      };
    }

    private void OnHandleState(
      EntityUid uid,
      LightDoomedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is LightDoomedComponent.LightDoomedComponent_AutoState current))
        return;
      component.Duration = current.Duration;
      component.EndsAt = current.EndsAt;
      component.WasEnabled = current.WasEnabled;
      component.DoomActivated = current.DoomActivated;
    }
  }
}
