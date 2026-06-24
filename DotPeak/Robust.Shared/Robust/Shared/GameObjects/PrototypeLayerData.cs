// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.PrototypeLayerData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class PrototypeLayerData : 
  ISerializationGenerated<PrototypeLayerData>,
  ISerializationGenerated
{
  [DataField("shader", false, 1, false, false, null)]
  public string? Shader;
  [DataField("texture", false, 1, false, false, null)]
  public string? TexturePath;
  [DataField("sprite", false, 1, false, false, null)]
  public string? RsiPath;
  [DataField("state", false, 1, false, false, null)]
  public string? State;
  [DataField("scale", false, 1, false, false, null)]
  public Vector2? Scale;
  [DataField("rotation", false, 1, false, false, null)]
  public Angle? Rotation;
  [DataField("offset", false, 1, false, false, null)]
  public Vector2? Offset;
  [DataField("visible", false, 1, false, false, null)]
  public bool? Visible;
  [DataField("color", false, 1, false, false, null)]
  public Robust.Shared.Maths.Color? Color;
  [DataField("map", false, 1, false, false, null)]
  public HashSet<string>? MapKeys;
  [DataField("renderingStrategy", false, 1, false, false, null)]
  public LayerRenderingStrategy? RenderingStrategy;
  [DataField(null, false, 1, false, false, null)]
  public PrototypeCopyToShaderParameters? CopyToShaderParameters;
  [DataField(null, false, 1, false, false, null)]
  public bool Cycle;
  [DataField(null, false, 1, false, false, null)]
  public bool Loop = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PrototypeLayerData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PrototypeLayerData>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Shader, ref target1, hookCtx, false, context))
      target1 = this.Shader;
    target.Shader = target1;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.TexturePath, ref target2, hookCtx, false, context))
      target2 = this.TexturePath;
    target.TexturePath = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.RsiPath, ref target3, hookCtx, false, context))
      target3 = this.RsiPath;
    target.RsiPath = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.State, ref target4, hookCtx, false, context))
      target4 = this.State;
    target.State = target4;
    Vector2? target5 = new Vector2?();
    if (!serialization.TryCustomCopy<Vector2?>(this.Scale, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2?>(this.Scale, hookCtx, context);
    target.Scale = target5;
    Angle? target6 = new Angle?();
    if (!serialization.TryCustomCopy<Angle?>(this.Rotation, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Angle?>(this.Rotation, hookCtx, context);
    target.Rotation = target6;
    Vector2? target7 = new Vector2?();
    if (!serialization.TryCustomCopy<Vector2?>(this.Offset, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Vector2?>(this.Offset, hookCtx, context);
    target.Offset = target7;
    bool? target8 = new bool?();
    if (!serialization.TryCustomCopy<bool?>(this.Visible, ref target8, hookCtx, false, context))
      target8 = this.Visible;
    target.Visible = target8;
    Robust.Shared.Maths.Color? target9 = new Robust.Shared.Maths.Color?();
    if (!serialization.TryCustomCopy<Robust.Shared.Maths.Color?>(this.Color, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Robust.Shared.Maths.Color?>(this.Color, hookCtx, context);
    target.Color = target9;
    HashSet<string> target10 = (HashSet<string>) null;
    if (!serialization.TryCustomCopy<HashSet<string>>(this.MapKeys, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<HashSet<string>>(this.MapKeys, hookCtx, context);
    target.MapKeys = target10;
    LayerRenderingStrategy? target11 = new LayerRenderingStrategy?();
    if (!serialization.TryCustomCopy<LayerRenderingStrategy?>(this.RenderingStrategy, ref target11, hookCtx, false, context))
      target11 = this.RenderingStrategy;
    target.RenderingStrategy = target11;
    PrototypeCopyToShaderParameters target12 = (PrototypeCopyToShaderParameters) null;
    if (!serialization.TryCustomCopy<PrototypeCopyToShaderParameters>(this.CopyToShaderParameters, ref target12, hookCtx, false, context))
    {
      if (this.CopyToShaderParameters == null)
        target12 = (PrototypeCopyToShaderParameters) null;
      else
        serialization.CopyTo<PrototypeCopyToShaderParameters>(this.CopyToShaderParameters, ref target12, hookCtx, context);
    }
    target.CopyToShaderParameters = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.Cycle, ref target13, hookCtx, false, context))
      target13 = this.Cycle;
    target.Cycle = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.Loop, ref target14, hookCtx, false, context))
      target14 = this.Loop;
    target.Loop = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PrototypeLayerData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PrototypeLayerData target1 = (PrototypeLayerData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PrototypeLayerData Instantiate() => new PrototypeLayerData();
}
