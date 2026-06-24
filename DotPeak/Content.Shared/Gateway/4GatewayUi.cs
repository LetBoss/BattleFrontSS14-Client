// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gateway.GatewayDestinationData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Gateway;

[NetSerializable]
[Serializable]
public record struct GatewayDestinationData
{
  public NetEntity Entity;
  public FormattedMessage Name;
  public bool Portal;
  public bool Locked;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<NetEntity>.Default.GetHashCode(this.Entity) * -1521134295 + EqualityComparer<FormattedMessage>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Portal)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Locked);
  }

  [CompilerGenerated]
  public readonly bool Equals(GatewayDestinationData other)
  {
    return EqualityComparer<NetEntity>.Default.Equals(this.Entity, other.Entity) && EqualityComparer<FormattedMessage>.Default.Equals(this.Name, other.Name) && EqualityComparer<bool>.Default.Equals(this.Portal, other.Portal) && EqualityComparer<bool>.Default.Equals(this.Locked, other.Locked);
  }
}
