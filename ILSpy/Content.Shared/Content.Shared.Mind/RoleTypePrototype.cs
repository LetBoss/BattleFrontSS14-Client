using System;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Mind;

[Prototype(null, 1)]
public sealed class RoleTypePrototype : IPrototype
{
	public static readonly LocId FallbackName = LocId.op_Implicit("role-type-crew-aligned-name");

	public const string FallbackSymbol = "";

	public static readonly Color FallbackColor = Color.FromHex((ReadOnlySpan<char>)"#eeeeee", (Color?)null);

	[DataField(null, false, 1, false, false, null)]
	public LocId Name = FallbackName;

	[DataField(null, false, 1, false, false, null)]
	public Color Color = FallbackColor;

	[DataField(null, false, 1, false, false, null)]
	public string Symbol = "";

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
