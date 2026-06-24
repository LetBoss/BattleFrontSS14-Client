// Decompiled with JetBrains decompiler
// Type: Content.Shared.Alert.AlertPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Alert;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class AlertPrototype : IPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public List<SpriteSpecifier> Icons = new List<SpriteSpecifier>();
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId AlertViewEntity = EntProtoId.op_Implicit("AlertSpriteView");
  [DataField("minSeverity", false, 1, false, false, null)]
  private short _minSeverity = 1;
  [DataField(null, false, 1, false, false, null)]
  public short MaxSeverity = -1;
  [DataField(null, false, 1, false, false, null)]
  public bool ClientHandled;
  [DataField(null, false, 1, false, false, null)]
  public BaseAlertEvent? ClickEvent;
  [DataField(null, false, 1, false, false, null)]
  public BaseAlertEvent? AltClickEvent;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string Name { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public string Description { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertCategoryPrototype>? Category { get; private set; }

  public AlertKey AlertKey
  {
    get => new AlertKey(ProtoId<AlertPrototype>.op_Implicit(this.ID), this.Category);
  }

  public short MinSeverity => this.MaxSeverity != (short) -1 ? this._minSeverity : (short) -1;

  public bool SupportsSeverity => this.MaxSeverity != (short) -1;

  public SpriteSpecifier GetIcon(short? severity = null)
  {
    if (this.Icons.Count < (this.SupportsSeverity ? (int) this.MaxSeverity - (int) this.MinSeverity : 1))
      throw new InvalidOperationException("Insufficient number of icons given for alert " + this.ID);
    if (!this.SupportsSeverity)
      return this.Icons[0];
    short? nullable1 = severity.HasValue ? severity : throw new ArgumentException($"No severity specified but this alert ({this.AlertKey}) has severity.", nameof (severity));
    int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
    int minSeverity = (int) this.MinSeverity;
    if (nullable2.GetValueOrDefault() < minSeverity & nullable2.HasValue)
      throw new ArgumentOutOfRangeException(nameof (severity), $"Severity below minimum severity in {this.AlertKey}.");
    nullable1 = severity;
    nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
    int maxSeverity = (int) this.MaxSeverity;
    if (nullable2.GetValueOrDefault() > maxSeverity & nullable2.HasValue)
      throw new ArgumentOutOfRangeException(nameof (severity), $"Severity above maximum severity in {this.AlertKey}.");
    return this.Icons[(int) severity.Value - (int) this._minSeverity];
  }

  [ParentDataField(typeof (PrototypeIdArraySerializer<AlertPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [AbstractDataField(1)]
  public bool Abstract { get; private set; }
}
