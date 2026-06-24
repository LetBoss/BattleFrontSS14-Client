using System;
using System.Text;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments;

[Serializable]
[NetSerializable]
public sealed class MidiTrack
{
	public string? TrackName;

	public string? InstrumentName;

	public string? ProgramName;

	private const string Postfix = "…";

	public override string ToString()
	{
		return $"Track Name: {TrackName}; Instrument Name: {InstrumentName}; Program Name: {ProgramName}";
	}

	public void TruncateFields(int limit)
	{
		if (InstrumentName != null)
		{
			InstrumentName = Truncate(InstrumentName, limit);
		}
		if (TrackName != null)
		{
			TrackName = Truncate(TrackName, limit);
		}
		if (ProgramName != null)
		{
			ProgramName = Truncate(ProgramName, limit);
		}
	}

	public void SanitizeFields()
	{
		if (InstrumentName != null)
		{
			InstrumentName = Sanitize(InstrumentName);
		}
		if (TrackName != null)
		{
			TrackName = Sanitize(TrackName);
		}
		if (ProgramName != null)
		{
			ProgramName = Sanitize(ProgramName);
		}
	}

	private string Truncate(string input, int limit)
	{
		if (string.IsNullOrEmpty(input) || limit <= 0 || input.Length <= limit)
		{
			return input;
		}
		int truncatedLength = limit - "…".Length;
		return input.Substring(0, truncatedLength) + "…";
	}

	private static string Sanitize(string input)
	{
		StringBuilder sanitized = new StringBuilder(input.Length);
		foreach (char c in input)
		{
			if (!char.IsControl(c) && c <= '\u007f')
			{
				sanitized.Append(c);
			}
		}
		return sanitized.ToString();
	}
}
