// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Components.GenericPartInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Construction.Components;

[DataDefinition]
[Serializable]
public struct GenericPartInfo : ISerializationGenerated<GenericPartInfo>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public int Amount;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId DefaultPrototype;
  [DataField(null, false, 1, false, false, null)]
  public LocId? ExamineName;

  public GenericPartInfo()
  {
    this.Amount = 0;
    this.DefaultPrototype = new EntProtoId();
    this.ExamineName = new LocId?();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GenericPartInfo target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<GenericPartInfo>(this, ref target, hookCtx, false, context))
      return;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref num, hookCtx, false, context))
      num = this.Amount;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.DefaultPrototype, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.DefaultPrototype, hookCtx, context, false);
    LocId? nullable = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.ExamineName, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<LocId?>(this.ExamineName, hookCtx, context, false);
    target = target with
    {
      Amount = num,
      DefaultPrototype = entProtoId,
      ExamineName = nullable
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GenericPartInfo target,
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
    GenericPartInfo target1 = (GenericPartInfo) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public GenericPartInfo Instantiate() => new GenericPartInfo();
}
