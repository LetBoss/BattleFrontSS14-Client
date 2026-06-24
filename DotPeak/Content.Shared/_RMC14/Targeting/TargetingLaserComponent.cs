// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Targeting.TargetingLaserComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Targeting;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class TargetingLaserComponent : 
  Component,
  ISerializationGenerated<TargetingLaserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowLaser = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color LaserColor = Color.Red;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color CurrentLaserColor = Color.Red;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LaserAlpha = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool GradualAlpha = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TargetingLaserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TargetingLaserComponent) target1;
    if (serialization.TryCustomCopy<TargetingLaserComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowLaser, ref target2, hookCtx, false, context))
      target2 = this.ShowLaser;
    target.ShowLaser = target2;
    Color target3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.LaserColor, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Color>(this.LaserColor, hookCtx, context);
    target.LaserColor = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.CurrentLaserColor, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.CurrentLaserColor, hookCtx, context);
    target.CurrentLaserColor = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LaserAlpha, ref target5, hookCtx, false, context))
      target5 = this.LaserAlpha;
    target.LaserAlpha = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.GradualAlpha, ref target6, hookCtx, false, context))
      target6 = this.GradualAlpha;
    target.GradualAlpha = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TargetingLaserComponent target,
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
    TargetingLaserComponent target1 = (TargetingLaserComponent) target;
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
    TargetingLaserComponent target1 = (TargetingLaserComponent) target;
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
    TargetingLaserComponent target1 = (TargetingLaserComponent) target;
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
  virtual TargetingLaserComponent Component.Instantiate() => new TargetingLaserComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TargetingLaserComponent_AutoState : IComponentState
  {
    public bool ShowLaser;
    public Color LaserColor;
    public Color CurrentLaserColor;
    public float LaserAlpha;
    public bool GradualAlpha;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TargetingLaserComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TargetingLaserComponent, ComponentGetState>(new ComponentEventRefHandler<TargetingLaserComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TargetingLaserComponent, ComponentHandleState>(new ComponentEventRefHandler<TargetingLaserComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TargetingLaserComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TargetingLaserComponent.TargetingLaserComponent_AutoState()
      {
        ShowLaser = component.ShowLaser,
        LaserColor = component.LaserColor,
        CurrentLaserColor = component.CurrentLaserColor,
        LaserAlpha = component.LaserAlpha,
        GradualAlpha = component.GradualAlpha
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TargetingLaserComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TargetingLaserComponent.TargetingLaserComponent_AutoState current))
        return;
      component.ShowLaser = current.ShowLaser;
      component.LaserColor = current.LaserColor;
      component.CurrentLaserColor = current.CurrentLaserColor;
      component.LaserAlpha = current.LaserAlpha;
      component.GradualAlpha = current.GradualAlpha;
    }
  }
}
