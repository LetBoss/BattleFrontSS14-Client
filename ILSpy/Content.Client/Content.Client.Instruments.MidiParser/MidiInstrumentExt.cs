using Robust.Shared.Utility;

namespace Content.Client.Instruments.MidiParser;

public static class MidiInstrumentExt
{
	public static string GetStringRep(this MidiInstrument instrument)
	{
		return CaseConversion.PascalToKebab(instrument.ToString());
	}
}
