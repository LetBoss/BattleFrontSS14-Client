using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

namespace Content.Shared.Alert;

[Prototype(null, 1)]
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

	public AlertKey AlertKey => new AlertKey(ProtoId<AlertPrototype>.op_Implicit(ID), Category);

	public short MinSeverity
	{
		get
		{
			if (MaxSeverity != -1)
			{
				return _minSeverity;
			}
			return -1;
		}
	}

	public bool SupportsSeverity => MaxSeverity != -1;

	[ParentDataField(typeof(PrototypeIdArraySerializer<AlertPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	public SpriteSpecifier GetIcon(short? severity = null)
	{
		int minIcons = ((!SupportsSeverity) ? 1 : (MaxSeverity - MinSeverity));
		if (Icons.Count < minIcons)
		{
			throw new InvalidOperationException("Insufficient number of icons given for alert " + ID);
		}
		if (!SupportsSeverity)
		{
			return Icons[0];
		}
		if (!severity.HasValue)
		{
			throw new ArgumentException($"No severity specified but this alert ({AlertKey}) has severity.", "severity");
		}
		if (severity < MinSeverity)
		{
			throw new ArgumentOutOfRangeException("severity", $"Severity below minimum severity in {AlertKey}.");
		}
		if (severity > MaxSeverity)
		{
			throw new ArgumentOutOfRangeException("severity", $"Severity above maximum severity in {AlertKey}.");
		}
		return Icons[severity.Value - _minSeverity];
	}
}
