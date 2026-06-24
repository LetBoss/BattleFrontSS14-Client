// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.BorgModuleRelayedEvent`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Borgs;

[ByRefEvent]
public record struct BorgModuleRelayedEvent<TEvent>(TEvent Args)
{
  public readonly TEvent Args = Args;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<TEvent>.Default.GetHashCode(this.Args);
  }

  [CompilerGenerated]
  public readonly bool Equals(BorgModuleRelayedEvent<
  #nullable disable
  TEvent> other) => EqualityComparer<TEvent>.Default.Equals(this.Args, other.Args);

  [CompilerGenerated]
  public readonly void Deconstruct(out 
  #nullable enable
  TEvent Args) => Args = this.Args;
}
