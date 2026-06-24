// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Components.MapLightComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Map.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MapLightComponent : 
  Component,
  ISerializationGenerated<MapLightComponent>,
  ISerializationGenerated
{
  public static readonly Color DefaultColor = Color.FromSrgb(Color.Black);
  [DataField(null, false, 1, false, false, null)]
  public Color AmbientLightColor = Color.FromSrgb(Color.Black);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MapLightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MapLightComponent) target1;
    if (serialization.TryCustomCopy<MapLightComponent>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.AmbientLightColor, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.AmbientLightColor, hookCtx, context);
    target.AmbientLightColor = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MapLightComponent target,
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
    MapLightComponent target1 = (MapLightComponent) target;
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
    MapLightComponent target1 = (MapLightComponent) target;
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
    MapLightComponent target1 = (MapLightComponent) target;
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
  virtual MapLightComponent Component.Instantiate() => new MapLightComponent();
}
