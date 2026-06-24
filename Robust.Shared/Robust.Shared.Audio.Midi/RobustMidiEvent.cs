using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Audio.Midi;

[Serializable]
[NetSerializable]
public readonly struct RobustMidiEvent
{
	public const int MaxChannels = 16;

	public const int PercussionChannel = 9;

	public readonly byte Status;

	public readonly byte Data1;

	public readonly byte Data2;

	public readonly uint Tick;

	public int Channel => Status & 0xF;

	public int Command => Status & 0xF0;

	public RobustMidiCommand MidiCommand => (RobustMidiCommand)Command;

	public byte Key => Data1;

	public byte Velocity => Data2;

	public byte Control => Data1;

	public byte Value => Data2;

	public int Pitch => (Data2 << 8) | Data1;

	public byte Pressure => Data1;

	public byte Program => Data1;

	public RobustMidiEvent(byte status, byte data1, byte data2, uint tick)
	{
		Status = status;
		Data1 = data1;
		Data2 = data2;
		Tick = tick;
	}

	public RobustMidiEvent(RobustMidiEvent ev, uint tick)
	{
		Status = ev.Status;
		Data1 = ev.Data1;
		Data2 = ev.Data2;
		Tick = tick;
	}

	public override string ToString()
	{
		return $"{base.ToString()} >> CHANNEL: 0x{Channel:X} || COMMAND: 0x{Command:X} {MidiCommand} || DATA1: 0x{Data1:X} || DATA2: 0x{Data2:X} || TICK: {Tick} <<";
	}

	public static byte MakeStatus(byte channel, byte command)
	{
		return (byte)(command | channel);
	}

	public static byte MakeStatus(byte channel, RobustMidiCommand command)
	{
		return MakeStatus(channel, (byte)command);
	}

	public static RobustMidiEvent NoteOff(byte channel, byte key, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.NoteOff), key, 0, tick);
	}

	public static RobustMidiEvent NoteOn(byte channel, byte key, byte velocity, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.NoteOn), key, velocity, tick);
	}

	public static RobustMidiEvent AfterTouch(byte channel, byte key, byte value, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.AfterTouch), key, value, tick);
	}

	public static RobustMidiEvent ControlChange(byte channel, byte control, byte value, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.ControlChange), control, value, tick);
	}

	public static RobustMidiEvent ProgramChange(byte channel, byte program, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.ProgramChange), program, 0, tick);
	}

	public static RobustMidiEvent ChannelPressure(byte channel, byte pressure, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.ChannelPressure), pressure, 0, tick);
	}

	public static RobustMidiEvent PitchBend(byte channel, ushort pitch, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.PitchBend), (byte)pitch, (byte)(pitch >> 8), tick);
	}

	public static RobustMidiEvent BankSelect(byte channel, byte bank, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.ControlChange), 0, bank, tick);
	}

	public static RobustMidiEvent AllNotesOff(byte channel, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.SystemMessage), 11, 0, tick);
	}

	public static RobustMidiEvent ResetAllControllers(uint tick)
	{
		return new RobustMidiEvent(176, 121, 0, tick);
	}

	public static RobustMidiEvent SystemMessage(byte channel, byte control, uint tick)
	{
		return new RobustMidiEvent(MakeStatus(channel, RobustMidiCommand.SystemMessage), control, 0, tick);
	}

	public static RobustMidiEvent SystemReset(uint tick)
	{
		return new RobustMidiEvent(byte.MaxValue, 0, 0, tick);
	}
}
