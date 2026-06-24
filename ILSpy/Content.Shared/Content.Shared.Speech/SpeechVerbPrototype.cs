using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Speech;

[Prototype(null, 1)]
public sealed class SpeechVerbPrototype : IPrototype
{
	[DataField("speechVerbStrings", false, 1, true, false, null)]
	public List<string> SpeechVerbStrings;

	[DataField("bold", false, 1, false, false, null)]
	public bool Bold;

	[DataField("fontSize", false, 1, false, false, null)]
	public int FontSize = 12;

	[DataField("fontId", false, 1, false, false, null)]
	public string FontId = "Default";

	[DataField("priority", false, 1, false, false, null)]
	public int Priority;

	[DataField(null, false, 1, true, false, null)]
	public LocId Name = LocId.op_Implicit(string.Empty);

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
