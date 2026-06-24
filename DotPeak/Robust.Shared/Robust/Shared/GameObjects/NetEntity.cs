// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.NetEntity
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[CopyByRef]
[Serializable]
public readonly struct NetEntity(int id) : 
  IEquatable<NetEntity>,
  IComparable<NetEntity>,
  ISpanFormattable,
  IFormattable
{
  public readonly int Id = id;
  public const int ClientEntity = 1073741824 /*0x40000000*/;
  public static readonly NetEntity Invalid = new NetEntity(0);
  public static readonly NetEntity First = new NetEntity(1);

  public bool Valid => this.IsValid();

  public static NetEntity Parse(ReadOnlySpan<char> uid)
  {
    if (uid.Length == 0)
      throw new FormatException("An empty string is not a valid NetEntity");
    if (uid[0] != 'c')
      return new NetEntity(int.Parse(uid));
    if (uid.Length == 1)
      throw new FormatException("'c' is not a valid NetEntity");
    ref ReadOnlySpan<char> local = ref uid;
    return new NetEntity(int.Parse(local.Slice(1, local.Length - 1)) | 1073741824 /*0x40000000*/);
  }

  public static bool TryParse(ReadOnlySpan<char> uid, out NetEntity entity)
  {
    entity = NetEntity.Invalid;
    if (uid.Length == 0)
      return false;
    if (uid[0] != 'c')
    {
      int result;
      if (!int.TryParse(uid, out result))
        return false;
      entity = new NetEntity(result);
      return true;
    }
    if (uid.Length == 1)
      return false;
    ref ReadOnlySpan<char> local = ref uid;
    int result1;
    if (!int.TryParse(local.Slice(1, local.Length - 1), out result1))
      return false;
    entity = new NetEntity(result1 | 1073741824 /*0x40000000*/);
    return true;
  }

  public bool IsValid() => this.Id > 0;

  public bool Equals(NetEntity other) => this.Id == other.Id;

  public override bool Equals(object? obj)
  {
    return obj != null && obj is NetEntity other && this.Equals(other);
  }

  public override int GetHashCode() => this.Id;

  public static bool operator ==(NetEntity a, NetEntity b) => a.Id == b.Id;

  public static bool operator !=(NetEntity a, NetEntity b) => !(a == b);

  public static explicit operator int(NetEntity self) => self.Id;

  public override string ToString()
  {
    if (!this.IsClientSide())
      return this.Id.ToString();
    return $"c{this.Id & -1073741825 /*0xBFFFFFFF*/}";
  }

  public string ToString(string? format, IFormatProvider? formatProvider) => this.ToString();

  public bool TryFormat(
    Span<char> destination,
    out int charsWritten,
    ReadOnlySpan<char> format,
    IFormatProvider? provider)
  {
    if (!this.IsClientSide())
      return this.Id.TryFormat(destination, out charsWritten);
    Span<char> span1 = destination;
    Span<char> span2 = span1;
    ref int local1 = ref charsWritten;
    BufferInterpolatedStringHandler interpolatedStringHandler;
    // ISSUE: explicit constructor call
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).\u002Ector(1, 1, span1);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral("c");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<int>(this.Id & -1073741825 /*0xBFFFFFFF*/);
    ref BufferInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    return FormatHelpers.TryFormatInto(span2, ref local1, ref local2);
  }

  public int CompareTo(NetEntity other) => this.Id.CompareTo(other.Id);

  public bool IsClientSide() => (this.Id & 1073741824 /*0x40000000*/) == 1073741824 /*0x40000000*/;

  [Robust.Shared.ViewVariables.ViewVariables]
  private string Representation
  {
    get
    {
      IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
      return (string) entityManager.ToPrettyString((Entity<MetaDataComponent>) entityManager.GetEntity(this));
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
      IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
      entityManager.System<MetaDataSystem>().SetEntityName(entityManager.GetEntity(this), value, metaData);
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
      IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
      entityManager.System<MetaDataSystem>().SetEntityDescription(entityManager.GetEntity(this), value, metaData);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private Robust.Shared.Prototypes.EntityPrototype? Prototype => this.MetaData?.EntityPrototype;

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
    get
    {
      IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
      return entityManager.GetComponentOrNull<MetaDataComponent>(entityManager.GetEntity(this));
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private TransformComponent? Transform
  {
    get
    {
      IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
      return entityManager.GetComponentOrNull<TransformComponent>(entityManager.GetEntity(this));
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private EntityUid _uid => IoCManager.Resolve<IEntityManager>().GetEntity(this);

  [Robust.Shared.ViewVariables.ViewVariables]
  private NetEntity _netId => this;
}
