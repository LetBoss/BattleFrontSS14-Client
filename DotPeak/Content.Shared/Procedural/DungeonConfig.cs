// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonConfig
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural;

[Virtual]
[DataDefinition]
public class DungeonConfig : ISerializationGenerated<DungeonConfig>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<IDunGenLayer> Layers = new List<IDunGenLayer>();
  [DataField(null, false, 1, false, false, null)]
  public bool ReserveTiles;
  [DataField(null, false, 1, false, false, null)]
  public int MinCount = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MaxCount = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MinOffset;
  [DataField(null, false, 1, false, false, null)]
  public int MaxOffset;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref DungeonConfig target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DungeonConfig>(this, ref target, hookCtx, false, context))
      return;
    List<IDunGenLayer> target1 = (List<IDunGenLayer>) null;
    if (this.Layers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<IDunGenLayer>>(this.Layers, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<List<IDunGenLayer>>(this.Layers, hookCtx, context);
    target.Layers = target1;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ReserveTiles, ref target2, hookCtx, false, context))
      target2 = this.ReserveTiles;
    target.ReserveTiles = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinCount, ref target3, hookCtx, false, context))
      target3 = this.MinCount;
    target.MinCount = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxCount, ref target4, hookCtx, false, context))
      target4 = this.MaxCount;
    target.MaxCount = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinOffset, ref target5, hookCtx, false, context))
      target5 = this.MinOffset;
    target.MinOffset = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxOffset, ref target6, hookCtx, false, context))
      target6 = this.MaxOffset;
    target.MaxOffset = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref DungeonConfig target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DungeonConfig target1 = (DungeonConfig) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual DungeonConfig Instantiate() => new DungeonConfig();
}
