// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetUserData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Immutable;
using System.Text;

#nullable enable
namespace Robust.Shared.Network;

public sealed record NetUserData
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public NetUserId UserId { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string UserName { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string? PatronTier { get; init; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public DateTime? CreatedTime { get; init; }

  public ImmutableArray<byte> HWId { get; init; }

  public ImmutableArray<ImmutableArray<byte>> ModernHWIds { get; init; }

  public float Trust { get; init; }

  public NetUserData(NetUserId userId, string userName)
  {
    this.UserId = userId;
    this.UserName = userName;
  }

  public sealed override string ToString()
  {
    StringBuilder builder = new StringBuilder();
    builder.Append(nameof (NetUserData));
    builder.Append(" { ");
    // ISSUE: reference to a compiler-generated method
    if ((this with { HWId = new ImmutableArray<byte>() }).PrintMembers(builder))
      builder.Append(' ');
    builder.Append('}');
    return builder.ToString();
  }
}
