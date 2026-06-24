// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Definition.DataDefinition
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Definition;

public abstract class DataDefinition
{
  internal ImmutableArray<FieldDefinition> BaseFieldDefinitions { get; init; }

  internal bool IsRecord { get; init; }

  public abstract bool TryGetDuplicates([NotNullWhen(true)] out string[] duplicates);
}
