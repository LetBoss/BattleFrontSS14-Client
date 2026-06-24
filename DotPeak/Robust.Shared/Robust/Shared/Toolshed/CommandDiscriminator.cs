// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.CommandDiscriminator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed;

internal readonly record struct CommandDiscriminator(Type? PipedType, Type[]? TypeArguments)
{
  public bool Equals(CommandDiscriminator other)
  {
    if (other.PipedType != this.PipedType)
      return false;
    if (other.TypeArguments == null && this.TypeArguments == null)
      return true;
    return this.TypeArguments != null && this.TypeArguments.Length == other.TypeArguments.Length && ((ReadOnlySpan<Type>) this.TypeArguments).SequenceEqual<Type>((ReadOnlySpan<Type>) other.TypeArguments, (IEqualityComparer<Type>) null);
  }

  public override int GetHashCode()
  {
    Type pipedType = this.PipedType;
    int hashCode = (object) pipedType != null ? pipedType.GetHashCode() : 715827882;
    if (this.TypeArguments == null)
      return hashCode;
    foreach (Type typeArgument in this.TypeArguments)
    {
      hashCode += hashCode ^ typeArgument.GetHashCode();
      int.RotateLeft(hashCode, 3);
    }
    return hashCode;
  }
}
