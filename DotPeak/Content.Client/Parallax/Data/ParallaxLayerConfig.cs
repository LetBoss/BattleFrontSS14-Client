// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Data.ParallaxLayerConfig
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Parallax.Data;

[DataDefinition]
public sealed class ParallaxLayerConfig : 
  ISerializationGenerated<ParallaxLayerConfig>,
  ISerializationGenerated
{
  [DataField("scrolling", false, 1, false, false, null)]
  public Vector2 Scrolling = Vector2.Zero;
  [DataField("shader", false, 1, false, false, null)]
  public string? Shader = "unshaded";

  [DataField("texture", false, 1, true, false, null)]
  public IParallaxTextureSource Texture { get; set; }

  [DataField("scale", false, 1, false, false, null)]
  public Vector2 Scale { get; set; } = Vector2.One;

  [DataField("tiled", false, 1, false, false, null)]
  public bool Tiled { get; set; } = true;

  [DataField("controlHomePosition", false, 1, false, false, null)]
  public Vector2 ControlHomePosition { get; set; }

  [DataField("worldHomePosition", false, 1, false, false, null)]
  public Vector2 WorldHomePosition { get; set; }

  [DataField("worldAdjustPosition", false, 1, false, false, null)]
  public Vector2 WorldAdjustPosition { get; set; }

  [DataField("slowness", false, 1, false, false, null)]
  public float Slowness { get; set; } = 0.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParallaxLayerConfig target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ParallaxLayerConfig>(this, ref target, hookCtx, false, context))
      return;
    IParallaxTextureSource parallaxTextureSource = (IParallaxTextureSource) null;
    if (this.Texture == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IParallaxTextureSource>(this.Texture, ref parallaxTextureSource, hookCtx, true, context))
      parallaxTextureSource = serialization.CreateCopy<IParallaxTextureSource>(this.Texture, hookCtx, context, false);
    target.Texture = parallaxTextureSource;
    Vector2 vector2_1 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Scale, ref vector2_1, hookCtx, false, context))
      vector2_1 = serialization.CreateCopy<Vector2>(this.Scale, hookCtx, context, false);
    target.Scale = vector2_1;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Tiled, ref flag, hookCtx, false, context))
      flag = this.Tiled;
    target.Tiled = flag;
    Vector2 vector2_2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.ControlHomePosition, ref vector2_2, hookCtx, false, context))
      vector2_2 = serialization.CreateCopy<Vector2>(this.ControlHomePosition, hookCtx, context, false);
    target.ControlHomePosition = vector2_2;
    Vector2 vector2_3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.WorldHomePosition, ref vector2_3, hookCtx, false, context))
      vector2_3 = serialization.CreateCopy<Vector2>(this.WorldHomePosition, hookCtx, context, false);
    target.WorldHomePosition = vector2_3;
    Vector2 vector2_4 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.WorldAdjustPosition, ref vector2_4, hookCtx, false, context))
      vector2_4 = serialization.CreateCopy<Vector2>(this.WorldAdjustPosition, hookCtx, context, false);
    target.WorldAdjustPosition = vector2_4;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Slowness, ref num, hookCtx, false, context))
      num = this.Slowness;
    target.Slowness = num;
    Vector2 vector2_5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Scrolling, ref vector2_5, hookCtx, false, context))
      vector2_5 = serialization.CreateCopy<Vector2>(this.Scrolling, hookCtx, context, false);
    target.Scrolling = vector2_5;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Shader, ref str, hookCtx, false, context))
      str = this.Shader;
    target.Shader = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParallaxLayerConfig target,
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
    ParallaxLayerConfig target1 = (ParallaxLayerConfig) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ParallaxLayerConfig Instantiate() => new ParallaxLayerConfig();
}
