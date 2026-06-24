// Decompiled with JetBrains decompiler
// Type: Content.Shared.Alert.AlertKey
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Alert;

[NetSerializable]
[Serializable]
public struct AlertKey
{
  public readonly ProtoId<AlertCategoryPrototype>? AlertCategory;

  public ProtoId<AlertPrototype>? AlertType { get; private set; }

  public AlertKey(
    ProtoId<AlertPrototype>? alertType,
    ProtoId<AlertCategoryPrototype>? alertCategory)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CAlertType\u003Ek__BackingField = new ProtoId<AlertPrototype>?();
    this.AlertCategory = alertCategory;
    this.AlertType = alertType;
  }

  public bool Equals(AlertKey other)
  {
    if (this.AlertCategory.HasValue)
    {
      ProtoId<AlertCategoryPrototype>? alertCategory1 = other.AlertCategory;
      ProtoId<AlertCategoryPrototype>? alertCategory2 = this.AlertCategory;
      if (alertCategory1.HasValue != alertCategory2.HasValue)
        return false;
      return !alertCategory1.HasValue || ProtoId<AlertCategoryPrototype>.op_Equality(alertCategory1.GetValueOrDefault(), alertCategory2.GetValueOrDefault());
    }
    ProtoId<AlertPrototype>? alertType1 = this.AlertType;
    ProtoId<AlertPrototype>? alertType2 = other.AlertType;
    if ((alertType1.HasValue == alertType2.HasValue ? (alertType1.HasValue ? (ProtoId<AlertPrototype>.op_Equality(alertType1.GetValueOrDefault(), alertType2.GetValueOrDefault()) ? 1 : 0) : 1) : 0) == 0)
      return false;
    ProtoId<AlertCategoryPrototype>? alertCategory3 = this.AlertCategory;
    ProtoId<AlertCategoryPrototype>? alertCategory4 = other.AlertCategory;
    if (alertCategory3.HasValue != alertCategory4.HasValue)
      return false;
    return !alertCategory3.HasValue || ProtoId<AlertCategoryPrototype>.op_Equality(alertCategory3.GetValueOrDefault(), alertCategory4.GetValueOrDefault());
  }

  public override bool Equals(object? obj) => obj is AlertKey other && this.Equals(other);

  public override int GetHashCode()
  {
    return this.AlertCategory.HasValue ? this.AlertCategory.GetHashCode() : this.AlertType.GetHashCode();
  }

  public static AlertKey ForCategory(ProtoId<AlertCategoryPrototype> category)
  {
    return new AlertKey(new ProtoId<AlertPrototype>?(), new ProtoId<AlertCategoryPrototype>?(category));
  }
}
