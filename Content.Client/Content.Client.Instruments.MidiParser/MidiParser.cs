using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Content.Shared.Instruments;
using Robust.Shared.Localization;

namespace Content.Client.Instruments.MidiParser;

public static class MidiParser
{
	public static bool TryGetMidiTracks(byte[] data, [NotNullWhen(true)] out MidiTrack[]? tracks, [NotNullWhen(false)] out string? error)
	{
		tracks = null;
		error = null;
		MidiStreamWrapper midiStreamWrapper = new MidiStreamWrapper(data);
		if (midiStreamWrapper.ReadString(4) != "MThd")
		{
			error = "Invalid file header";
			return false;
		}
		uint num = midiStreamWrapper.ReadUInt32();
		midiStreamWrapper.Skip(2);
		ushort num2 = midiStreamWrapper.ReadUInt16();
		midiStreamWrapper.Skip(2);
		midiStreamWrapper.Skip((int)(num - 6));
		List<MidiTrack> list = new List<MidiTrack>();
		for (int i = 0; i < num2; i++)
		{
			if (midiStreamWrapper.ReadString(4) != "MTrk")
			{
				tracks = null;
				error = "Track contains invalid header";
				return false;
			}
			MidiTrack midiTrack = new MidiTrack();
			uint num3 = midiStreamWrapper.ReadUInt32();
			long num4 = midiStreamWrapper.StreamPosition + num3;
			bool flag = false;
			byte? b = null;
			while (midiStreamWrapper.StreamPosition < num4)
			{
				midiStreamWrapper.ReadVariableLengthQuantity();
				byte b2 = midiStreamWrapper.ReadByte();
				if (b2 >= 128)
				{
					b = b2;
				}
				else
				{
					midiStreamWrapper.Skip(-1);
				}
				if (!b.HasValue)
				{
					tracks = null;
					error = "Track data not valid, expected status byte, got nothing.";
					return false;
				}
				byte b3 = (byte)(b & 0xF0).Value;
				switch (b)
				{
				case byte.MaxValue:
				{
					byte b4 = midiStreamWrapper.ReadByte();
					uint count2 = midiStreamWrapper.ReadVariableLengthQuantity();
					byte[] bytes = midiStreamWrapper.ReadBytes((int)count2);
					if (b4 == 0 || ((b4 < 1 || b4 > 15) ? true : false))
					{
						continue;
					}
					string text = Encoding.ASCII.GetString(bytes, 0, (int)count2);
					switch (b4)
					{
					case 3:
						if (midiTrack.TrackName == null)
						{
							midiTrack.TrackName = text;
						}
						break;
					case 4:
						if (midiTrack.InstrumentName == null)
						{
							midiTrack.InstrumentName = text;
						}
						break;
					}
					continue;
				}
				case 240:
				case 247:
				{
					uint count = midiStreamWrapper.ReadVariableLengthQuantity();
					midiStreamWrapper.Skip((int)count);
					b = null;
					continue;
				}
				}
				switch (b3)
				{
				case 192:
				{
					byte b5 = midiStreamWrapper.ReadByte();
					if (midiTrack.ProgramName == null && b5 < Enum.GetValues<MidiInstrument>().Length)
					{
						midiTrack.ProgramName = Loc.GetString("instruments-component-menu-midi-channel-" + ((MidiInstrument)b5).GetStringRep());
					}
					break;
				}
				case 128:
				case 144:
				case 160:
				case 176:
				case 224:
					flag = true;
					midiStreamWrapper.Skip(2);
					break;
				case 208:
					flag = true;
					midiStreamWrapper.Skip(1);
					break;
				default:
					error = $"Unknown MIDI event type {b:X2}";
					tracks = null;
					return false;
				}
			}
			if (flag)
			{
				list.Add(midiTrack);
			}
		}
		tracks = list.ToArray();
		return true;
	}
}
