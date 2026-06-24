using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert;

[Serializable]
[NetSerializable]
public struct AlertKey
{
	public readonly ProtoId<AlertCategoryPrototype>? AlertCategory;

	public ProtoId<AlertPrototype>? AlertType { get; private set; }

	public AlertKey(ProtoId<AlertPrototype>? alertType, ProtoId<AlertCategoryPrototype>? alertCategory)
	{
		AlertType = null;
		AlertCategory = alertCategory;
		AlertType = alertType;
	}

	public bool Equals(AlertKey other)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (AlertCategory.HasValue)
		{
			ProtoId<AlertCategoryPrototype>? alertCategory = other.AlertCategory;
			ProtoId<AlertCategoryPrototype>? alertCategory2 = AlertCategory;
			if (alertCategory.HasValue != alertCategory2.HasValue)
			{
				return false;
			}
			if (!alertCategory.HasValue)
			{
				return true;
			}
			return alertCategory.GetValueOrDefault() == alertCategory2.GetValueOrDefault();
		}
		ProtoId<AlertPrototype>? alertType = AlertType;
		ProtoId<AlertPrototype>? alertType2 = other.AlertType;
		if (alertType.HasValue == alertType2.HasValue && (!alertType.HasValue || alertType.GetValueOrDefault() == alertType2.GetValueOrDefault()))
		{
			ProtoId<AlertCategoryPrototype>? alertCategory2 = AlertCategory;
			ProtoId<AlertCategoryPrototype>? alertCategory = other.AlertCategory;
			if (alertCategory2.HasValue != alertCategory.HasValue)
			{
				return false;
			}
			if (!alertCategory2.HasValue)
			{
				return true;
			}
			return alertCategory2.GetValueOrDefault() == alertCategory.GetValueOrDefault();
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is AlertKey other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (AlertCategory.HasValue)
		{
			return AlertCategory.GetHashCode();
		}
		return AlertType.GetHashCode();
	}

	public static AlertKey ForCategory(ProtoId<AlertCategoryPrototype> category)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return new AlertKey(null, category);
	}
}
