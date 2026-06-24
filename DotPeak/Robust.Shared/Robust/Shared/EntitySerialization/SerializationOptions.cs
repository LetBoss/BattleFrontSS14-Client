// Decompiled with JetBrains decompiler
// Type: Robust.Shared.EntitySerialization.SerializationOptions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.EntitySerialization;

public record struct SerializationOptions
{
  public static readonly SerializationOptions Default = new SerializationOptions();
  public MissingEntityBehaviour MissingEntityBehaviour;
  public EntityExceptionBehaviour EntityExceptionBehaviour;
  public bool ErrorOnOrphan;
  public LogLevel? LogAutoInclude;
  public bool ExpectPreInit;
  public FileCategory Category;

  public SerializationOptions()
  {
    this.ExpectPreInit = false;
    this.Category = FileCategory.Unknown;
    this.MissingEntityBehaviour = MissingEntityBehaviour.IncludeNullspace;
    this.EntityExceptionBehaviour = EntityExceptionBehaviour.Rethrow;
    this.ErrorOnOrphan = true;
    this.LogAutoInclude = new LogLevel?(LogLevel.Info);
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((((EqualityComparer<MissingEntityBehaviour>.Default.GetHashCode(this.MissingEntityBehaviour) * -1521134295 + EqualityComparer<EntityExceptionBehaviour>.Default.GetHashCode(this.EntityExceptionBehaviour)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.ErrorOnOrphan)) * -1521134295 + EqualityComparer<LogLevel?>.Default.GetHashCode(this.LogAutoInclude)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.ExpectPreInit)) * -1521134295 + EqualityComparer<FileCategory>.Default.GetHashCode(this.Category);
  }

  [CompilerGenerated]
  public readonly bool Equals(SerializationOptions other)
  {
    return EqualityComparer<MissingEntityBehaviour>.Default.Equals(this.MissingEntityBehaviour, other.MissingEntityBehaviour) && EqualityComparer<EntityExceptionBehaviour>.Default.Equals(this.EntityExceptionBehaviour, other.EntityExceptionBehaviour) && EqualityComparer<bool>.Default.Equals(this.ErrorOnOrphan, other.ErrorOnOrphan) && EqualityComparer<LogLevel?>.Default.Equals(this.LogAutoInclude, other.LogAutoInclude) && EqualityComparer<bool>.Default.Equals(this.ExpectPreInit, other.ExpectPreInit) && EqualityComparer<FileCategory>.Default.Equals(this.Category, other.Category);
  }
}
