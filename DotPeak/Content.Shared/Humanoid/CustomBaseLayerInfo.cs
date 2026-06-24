// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.CustomBaseLayerInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Humanoid;

[DataDefinition]
[NetSerializable]
[Serializable]
public readonly struct CustomBaseLayerInfo : 
  ISerializationGenerated<CustomBaseLayerInfo>,
  ISerializationGenerated
{
  public CustomBaseLayerInfo(string? id, Robust.Shared.Maths.Color? color = null)
  {
    this.Id = (ProtoId<HumanoidSpeciesSpriteLayer>?) id;
    this.Color = color;
  }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HumanoidSpeciesSpriteLayer>? Id { get; init; }

  [DataField(null, false, 1, false, false, null)]
  public Robust.Shared.Maths.Color? Color { get; init; }

  public CustomBaseLayerInfo()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CId\u003Ek__BackingField = new ProtoId<HumanoidSpeciesSpriteLayer>?();
    // ISSUE: reference to a compiler-generated field
    this.\u003CColor\u003Ek__BackingField = new Robust.Shared.Maths.Color?();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CustomBaseLayerInfo target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CustomBaseLayerInfo>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<HumanoidSpeciesSpriteLayer>? target1 = new ProtoId<HumanoidSpeciesSpriteLayer>?();
    if (!serialization.TryCustomCopy<ProtoId<HumanoidSpeciesSpriteLayer>?>(this.Id, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<HumanoidSpeciesSpriteLayer>?>(this.Id, hookCtx, context);
    Robust.Shared.Maths.Color? target2 = new Robust.Shared.Maths.Color?();
    if (!serialization.TryCustomCopy<Robust.Shared.Maths.Color?>(this.Color, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Robust.Shared.Maths.Color?>(this.Color, hookCtx, context);
    target = target with { Id = target1, Color = target2 };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CustomBaseLayerInfo target,
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
    CustomBaseLayerInfo target1 = (CustomBaseLayerInfo) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CustomBaseLayerInfo Instantiate() => new CustomBaseLayerInfo();
}
