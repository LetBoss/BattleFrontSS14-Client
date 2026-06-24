// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.EntProtoId`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Prototypes;

[Serializable]
public readonly record struct EntProtoId<T>(string Id) : IEquatable<string>, IComparable<EntProtoId> where T : IComponent, new()
{
  public static implicit operator string(EntProtoId<T> protoId) => protoId.Id;

  public static implicit operator EntProtoId(EntProtoId<T> protoId) => new EntProtoId(protoId.Id);

  public static implicit operator EntProtoId<T>(string id) => new EntProtoId<T>(id);

  public static implicit operator EntProtoId<T>?(string? id)
  {
    return id != null ? new EntProtoId<T>?(new EntProtoId<T>(id)) : new EntProtoId<T>?();
  }

  public bool Equals(string? other) => this.Id == other;

  public int CompareTo(EntProtoId other)
  {
    return string.Compare(this.Id, other.Id, StringComparison.Ordinal);
  }

  public override string ToString() => this.Id ?? string.Empty;

  public T Get(IPrototypeManager? prototypes, IComponentFactory compFactory)
  {
    if (prototypes == null)
      prototypes = IoCManager.Resolve<IPrototypeManager>();
    EntityPrototype entityPrototype = prototypes.Index((EntProtoId) this);
    T component;
    if (!entityPrototype.TryGetComponent<T>(out component, compFactory))
      throw new ArgumentException($"{"EntityPrototype"} {entityPrototype.ID} has no {nameof (T)}");
    return component;
  }

  public bool TryGet([NotNullWhen(true)] out T? comp, IPrototypeManager? prototypes, IComponentFactory compFactory)
  {
    comp = default (T);
    if (prototypes == null)
      prototypes = IoCManager.Resolve<IPrototypeManager>();
    EntityPrototype prototype;
    return prototypes.TryIndex((EntProtoId) this, out prototype) && prototype.TryGetComponent<T>(out comp, compFactory);
  }
}
