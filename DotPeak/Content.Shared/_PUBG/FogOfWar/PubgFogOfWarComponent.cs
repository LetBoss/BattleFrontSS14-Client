// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.FogOfWar.PubgFogOfWarComponent
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
namespace Content.Shared._PUBG.FogOfWar;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgFogOfWarComponent : 
  Component,
  ISerializationGenerated<PubgFogOfWarComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 20f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FovDegrees = 150f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ConeOpacity = 0.85f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Angle CurrentAngle;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Angle? DesiredViewAngle;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgFogOfWarComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgFogOfWarComponent) target1;
    if (serialization.TryCustomCopy<PubgFogOfWarComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FovDegrees, ref target4, hookCtx, false, context))
      target4 = this.FovDegrees;
    target.FovDegrees = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ConeOpacity, ref target5, hookCtx, false, context))
      target5 = this.ConeOpacity;
    target.ConeOpacity = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgFogOfWarComponent target,
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
    PubgFogOfWarComponent target1 = (PubgFogOfWarComponent) target;
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
    PubgFogOfWarComponent target1 = (PubgFogOfWarComponent) target;
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
    PubgFogOfWarComponent target1 = (PubgFogOfWarComponent) target;
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
  virtual PubgFogOfWarComponent Component.Instantiate() => new PubgFogOfWarComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgFogOfWarComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float Range;
    public float FovDegrees;
    public float ConeOpacity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgFogOfWarComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgFogOfWarComponent, ComponentGetState>(new ComponentEventRefHandler<PubgFogOfWarComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgFogOfWarComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgFogOfWarComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgFogOfWarComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgFogOfWarComponent.PubgFogOfWarComponent_AutoState()
      {
        Enabled = component.Enabled,
        Range = component.Range,
        FovDegrees = component.FovDegrees,
        ConeOpacity = component.ConeOpacity
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgFogOfWarComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgFogOfWarComponent.PubgFogOfWarComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Range = current.Range;
      component.FovDegrees = current.FovDegrees;
      component.ConeOpacity = current.ConeOpacity;
    }
  }
}
