// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.Weapons.ShellTypeInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle.Weapons;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class ShellTypeInfo : ISerializationGenerated<ShellTypeInfo>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId ProtoId;
  [DataField(null, false, 1, true, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public string Description = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public string SpriteState = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ShellTypeInfo target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ShellTypeInfo>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target1 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ProtoId, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId>(this.ProtoId, hookCtx, context);
    target.ProtoId = target1;
    string target2 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target2, hookCtx, false, context))
      target2 = this.Name;
    target.Name = target2;
    string target3 = (string) null;
    if (this.Description == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Description, ref target3, hookCtx, false, context))
      target3 = this.Description;
    target.Description = target3;
    string target4 = (string) null;
    if (this.SpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SpriteState, ref target4, hookCtx, false, context))
      target4 = this.SpriteState;
    target.SpriteState = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ShellTypeInfo target,
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
    ShellTypeInfo target1 = (ShellTypeInfo) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ShellTypeInfo Instantiate() => new ShellTypeInfo();
}
