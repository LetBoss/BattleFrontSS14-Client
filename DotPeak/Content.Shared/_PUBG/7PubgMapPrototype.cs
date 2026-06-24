// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.ChunkedMapSettings
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class ChunkedMapSettings : 
  ISerializationGenerated<ChunkedMapSettings>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Enabled;
  [DataField("light", false, 1, false, false, null)]
  public bool LightEnabled;
  [DataField("lightColor", false, 1, false, false, null)]
  public Color LightColor = Color.Black;
  [DataField(null, false, 1, false, false, null)]
  public int ChunkWidth;
  [DataField(null, false, 1, false, false, null)]
  public int ChunkHeight;
  [DataField(null, false, 1, false, false, null)]
  public List<ResPath> ChunkPaths = new List<ResPath>();
  [DataField(null, false, 1, false, false, null)]
  public ResPath? ChunkFolder;
  [DataField(null, false, 1, false, false, null)]
  public float ChunkLoadIntervalSeconds = 0.6f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChunkedMapSettings target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ChunkedMapSettings>(this, ref target, hookCtx, false, context))
      return;
    bool target1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target1, hookCtx, false, context))
      target1 = this.Enabled;
    target.Enabled = target1;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.LightEnabled, ref target2, hookCtx, false, context))
      target2 = this.LightEnabled;
    target.LightEnabled = target2;
    Color target3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.LightColor, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Color>(this.LightColor, hookCtx, context);
    target.LightColor = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.ChunkWidth, ref target4, hookCtx, false, context))
      target4 = this.ChunkWidth;
    target.ChunkWidth = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.ChunkHeight, ref target5, hookCtx, false, context))
      target5 = this.ChunkHeight;
    target.ChunkHeight = target5;
    List<ResPath> target6 = (List<ResPath>) null;
    if (this.ChunkPaths == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ResPath>>(this.ChunkPaths, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<ResPath>>(this.ChunkPaths, hookCtx, context);
    target.ChunkPaths = target6;
    ResPath? target7 = new ResPath?();
    if (!serialization.TryCustomCopy<ResPath?>(this.ChunkFolder, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ResPath?>(this.ChunkFolder, hookCtx, context);
    target.ChunkFolder = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ChunkLoadIntervalSeconds, ref target8, hookCtx, false, context))
      target8 = this.ChunkLoadIntervalSeconds;
    target.ChunkLoadIntervalSeconds = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChunkedMapSettings target,
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
    ChunkedMapSettings target1 = (ChunkedMapSettings) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ChunkedMapSettings Instantiate() => new ChunkedMapSettings();
}
