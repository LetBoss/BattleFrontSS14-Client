// Decompiled with JetBrains decompiler
// Type: Content.Client.SurveillanceCamera.SurveillanceCameraVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.SurveillanceCamera;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.SurveillanceCamera;

[RegisterComponent]
public sealed class SurveillanceCameraVisualsComponent : 
  Component,
  ISerializationGenerated<SurveillanceCameraVisualsComponent>,
  ISerializationGenerated
{
  [DataField("sprites", false, 1, false, false, null)]
  public Dictionary<SurveillanceCameraVisuals, string> CameraSprites = new Dictionary<SurveillanceCameraVisuals, string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SurveillanceCameraVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SurveillanceCameraVisualsComponent) component;
    if (serialization.TryCustomCopy<SurveillanceCameraVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<SurveillanceCameraVisuals, string> dictionary = (Dictionary<SurveillanceCameraVisuals, string>) null;
    if (this.CameraSprites == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<SurveillanceCameraVisuals, string>>(this.CameraSprites, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<SurveillanceCameraVisuals, string>>(this.CameraSprites, hookCtx, context, false);
    target.CameraSprites = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SurveillanceCameraVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SurveillanceCameraVisualsComponent target1 = (SurveillanceCameraVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SurveillanceCameraVisualsComponent target1 = (SurveillanceCameraVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SurveillanceCameraVisualsComponent target1 = (SurveillanceCameraVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SurveillanceCameraVisualsComponent Component.Instantiate()
  {
    return new SurveillanceCameraVisualsComponent();
  }
}
