// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityUid
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[CopyByRef]
public readonly struct EntityUid(int id) : 
  IEquatable<EntityUid>,
  IComparable<EntityUid>,
  ISpanFormattable,
  IFormattable
{
  public readonly int Id = id;
  public static readonly EntityUid Invalid = new EntityUid(0);
  public static readonly EntityUid FirstUid = new EntityUid(1);

  public bool Valid => this.IsValid();

  public static EntityUid Parse(ReadOnlySpan<char> uid) => new EntityUid(int.Parse(uid));

  public static bool TryParse(ReadOnlySpan<char> uid, out EntityUid entityUid)
  {
    int result;
    if (!int.TryParse(uid, out result))
    {
      entityUid = new EntityUid();
      return false;
    }
    entityUid = new EntityUid(result);
    return true;
  }

  public bool IsValid() => this.Id > 0;

  public bool Equals(EntityUid other) => this.Id == other.Id;

  public override bool Equals(object? obj)
  {
    return obj != null && obj is EntityUid other && this.Equals(other);
  }

  public override int GetHashCode() => this.Id.GetHashCode() * 397;

  public static bool operator ==(EntityUid a, EntityUid b) => a.Id == b.Id;

  public static bool operator !=(EntityUid a, EntityUid b) => !(a == b);

  public static explicit operator int(EntityUid self) => self.Id;

  public override string ToString() => this.Id.ToString();

  public string ToString(string? format, IFormatProvider? formatProvider) => this.ToString();

  public bool TryFormat(
    Span<char> destination,
    out int charsWritten,
    ReadOnlySpan<char> format,
    IFormatProvider? provider)
  {
    return this.Id.TryFormat(destination, out charsWritten);
  }

  public int CompareTo(EntityUid other) => this.Id.CompareTo(other.Id);

  [Robust.Shared.ViewVariables.ViewVariables]
  private string Representation
  {
    get
    {
      return (string) IoCManager.Resolve<IEntityManager>().ToPrettyString((Entity<MetaDataComponent>) this);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  private string Name
  {
    get => this.MetaData?.EntityName ?? string.Empty;
    set
    {
      MetaDataComponent metaData = this.MetaData;
      if (metaData == null)
        return;
      IoCManager.Resolve<IEntityManager>().System<MetaDataSystem>().SetEntityName(this, value, metaData);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  private string Description
  {
    get => this.MetaData?.EntityDescription ?? string.Empty;
    set
    {
      MetaDataComponent metaData = this.MetaData;
      if (metaData == null)
        return;
      IoCManager.Resolve<IEntityManager>().System<MetaDataSystem>().SetEntityDescription(this, value, metaData);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private EntityPrototype? Prototype => this.MetaData?.EntityPrototype;

  [Robust.Shared.ViewVariables.ViewVariables]
  private GameTick LastModifiedTick
  {
    get
    {
      MetaDataComponent metaData = this.MetaData;
      return metaData == null ? GameTick.Zero : metaData.EntityLastModifiedTick;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private bool Paused
  {
    get
    {
      MetaDataComponent metaData = this.MetaData;
      return metaData != null && metaData.EntityPaused;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private EntityLifeStage LifeStage
  {
    get
    {
      MetaDataComponent metaData = this.MetaData;
      return metaData == null ? EntityLifeStage.Deleted : metaData.EntityLifeStage;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private MetaDataComponent? MetaData
  {
    get => IoCManager.Resolve<IEntityManager>().GetComponentOrNull<MetaDataComponent>(this);
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private TransformComponent? Transform
  {
    get => IoCManager.Resolve<IEntityManager>().GetComponentOrNull<TransformComponent>(this);
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private EntityUid _uid => this;

  [Robust.Shared.ViewVariables.ViewVariables]
  private NetEntity _netId => IoCManager.Resolve<IEntityManager>().GetNetEntity(this);
}
