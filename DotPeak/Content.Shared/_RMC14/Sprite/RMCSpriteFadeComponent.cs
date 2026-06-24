// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sprite.RMCSpriteFadeComponent
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Sprite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCSpriteFadeComponent : 
  Component,
  ISerializationGenerated<RMCSpriteFadeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TargetAlpha = 0.4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ChangeRate = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ReactToMouse = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<string> FadeLayers = new List<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCSpriteFadeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCSpriteFadeComponent) target1;
    if (serialization.TryCustomCopy<RMCSpriteFadeComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TargetAlpha, ref target2, hookCtx, false, context))
      target2 = this.TargetAlpha;
    target.TargetAlpha = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ChangeRate, ref target3, hookCtx, false, context))
      target3 = this.ChangeRate;
    target.ChangeRate = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ReactToMouse, ref target4, hookCtx, false, context))
      target4 = this.ReactToMouse;
    target.ReactToMouse = target4;
    List<string> target5 = (List<string>) null;
    if (this.FadeLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.FadeLayers, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<string>>(this.FadeLayers, hookCtx, context);
    target.FadeLayers = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCSpriteFadeComponent target,
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
    RMCSpriteFadeComponent target1 = (RMCSpriteFadeComponent) target;
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
    RMCSpriteFadeComponent target1 = (RMCSpriteFadeComponent) target;
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
    RMCSpriteFadeComponent target1 = (RMCSpriteFadeComponent) target;
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
  virtual RMCSpriteFadeComponent Component.Instantiate() => new RMCSpriteFadeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCSpriteFadeComponent_AutoState : IComponentState
  {
    public float TargetAlpha;
    public float ChangeRate;
    public bool ReactToMouse;
    public List<string> FadeLayers;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCSpriteFadeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCSpriteFadeComponent, ComponentGetState>(new ComponentEventRefHandler<RMCSpriteFadeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCSpriteFadeComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCSpriteFadeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCSpriteFadeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCSpriteFadeComponent.RMCSpriteFadeComponent_AutoState()
      {
        TargetAlpha = component.TargetAlpha,
        ChangeRate = component.ChangeRate,
        ReactToMouse = component.ReactToMouse,
        FadeLayers = component.FadeLayers
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCSpriteFadeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCSpriteFadeComponent.RMCSpriteFadeComponent_AutoState current))
        return;
      component.TargetAlpha = current.TargetAlpha;
      component.ChangeRate = current.ChangeRate;
      component.ReactToMouse = current.ReactToMouse;
      component.FadeLayers = current.FadeLayers == null ? (List<string>) null : new List<string>((IEnumerable<string>) current.FadeLayers);
    }
  }
}
