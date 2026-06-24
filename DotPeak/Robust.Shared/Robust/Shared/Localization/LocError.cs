// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LocError
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Linguini.Bundle.Errors;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Localization;

internal record LocError
{
  public readonly ResPath Path;
  public readonly FluentError Error;

  public LocError(ResPath path, FluentError fluentError)
  {
    this.Path = path;
    this.Error = fluentError;
  }

  public override string ToString() => $"[{this.Path.CanonPath}]: {this.Error}";

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Path = ");
    builder.Append(this.Path.ToString());
    builder.Append(", Error = ");
    builder.Append((object) this.Error);
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<ResPath>.Default.GetHashCode(this.Path)) * -1521134295 + EqualityComparer<FluentError>.Default.GetHashCode(this.Error);
  }

  [CompilerGenerated]
  public virtual bool Equals(LocError? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<ResPath>.Default.Equals(this.Path, other.Path) && EqualityComparer<FluentError>.Default.Equals(this.Error, other.Error);
  }

  [CompilerGenerated]
  protected LocError(LocError original)
  {
    this.Path = original.Path;
    this.Error = original.Error;
  }
}
