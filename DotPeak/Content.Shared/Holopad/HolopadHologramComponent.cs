// Decompiled with JetBrains decompiler
// Type: Content.Shared.Holopad.HolopadHologramComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Holopad;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class HolopadHologramComponent : 
  Component,
  ISerializationGenerated<HolopadHologramComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string RsiPath = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string RsiState = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string ShaderName = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public Color Color1 = Color.White;
  [DataField(null, false, 1, false, false, null)]
  public Color Color2 = Color.White;
  [DataField(null, false, 1, false, false, null)]
  public float Alpha = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float Intensity = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float ScrollRate = 1f;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Offset;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public EntityUid? LinkedEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HolopadHologramComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HolopadHologramComponent) target1;
    if (serialization.TryCustomCopy<HolopadHologramComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.RsiPath == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RsiPath, ref target2, hookCtx, false, context))
      target2 = this.RsiPath;
    target.RsiPath = target2;
    string target3 = (string) null;
    if (this.RsiState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RsiState, ref target3, hookCtx, false, context))
      target3 = this.RsiState;
    target.RsiState = target3;
    string target4 = (string) null;
    if (this.ShaderName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ShaderName, ref target4, hookCtx, false, context))
      target4 = this.ShaderName;
    target.ShaderName = target4;
    Color target5 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color1, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Color>(this.Color1, hookCtx, context);
    target.Color1 = target5;
    Color target6 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color2, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Color>(this.Color2, hookCtx, context);
    target.Color2 = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Alpha, ref target7, hookCtx, false, context))
      target7 = this.Alpha;
    target.Alpha = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Intensity, ref target8, hookCtx, false, context))
      target8 = this.Intensity;
    target.Intensity = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ScrollRate, ref target9, hookCtx, false, context))
      target9 = this.ScrollRate;
    target.ScrollRate = target9;
    Vector2 target10 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HolopadHologramComponent target,
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
    HolopadHologramComponent target1 = (HolopadHologramComponent) target;
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
    HolopadHologramComponent target1 = (HolopadHologramComponent) target;
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
    HolopadHologramComponent target1 = (HolopadHologramComponent) target;
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
  virtual HolopadHologramComponent Component.Instantiate() => new HolopadHologramComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HolopadHologramComponent_AutoState : IComponentState
  {
    public NetEntity? LinkedEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HolopadHologramComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HolopadHologramComponent, ComponentGetState>(new ComponentEventRefHandler<HolopadHologramComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HolopadHologramComponent, ComponentHandleState>(new ComponentEventRefHandler<HolopadHologramComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HolopadHologramComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HolopadHologramComponent.HolopadHologramComponent_AutoState()
      {
        LinkedEntity = this.GetNetEntity(component.LinkedEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HolopadHologramComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HolopadHologramComponent.HolopadHologramComponent_AutoState current))
        return;
      component.LinkedEntity = this.EnsureEntity<HolopadHologramComponent>(current.LinkedEntity, uid);
    }
  }
}
