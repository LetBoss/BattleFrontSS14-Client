// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Components.TeleportPoint
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Teleportation.Components;

[NetSerializable]
[DataDefinition]
[Serializable]
public record struct TeleportPoint : ISerializationGenerated<TeleportPoint>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Location;
  [DataField(null, false, 1, false, false, null)]
  public NetEntity TelePoint;

  public TeleportPoint(string Location, NetEntity TelePoint)
  {
    this.Location = Location;
    this.TelePoint = TelePoint;
  }

  public TeleportPoint()
  {
    this.Location = (string) null;
    this.TelePoint = new NetEntity();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TeleportPoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<TeleportPoint>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Location == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Location, ref target1, hookCtx, false, context))
      target1 = this.Location;
    NetEntity target2 = new NetEntity();
    if (!serialization.TryCustomCopy<NetEntity>(this.TelePoint, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetEntity>(this.TelePoint, hookCtx, context);
    target = target with
    {
      Location = target1,
      TelePoint = target2
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TeleportPoint target,
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
    TeleportPoint target1 = (TeleportPoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public TeleportPoint Instantiate() => new TeleportPoint();

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<string>.Default.GetHashCode(this.Location) * -1521134295 + EqualityComparer<NetEntity>.Default.GetHashCode(this.TelePoint);
  }

  [CompilerGenerated]
  public readonly bool Equals(TeleportPoint other)
  {
    return EqualityComparer<string>.Default.Equals(this.Location, other.Location) && EqualityComparer<NetEntity>.Default.Equals(this.TelePoint, other.TelePoint);
  }
}
