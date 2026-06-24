// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Midi.RobustMidiEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Audio.Midi;

[NetSerializable]
[Serializable]
public readonly struct RobustMidiEvent
{
  public const int MaxChannels = 16 /*0x10*/;
  public const int PercussionChannel = 9;
  public readonly byte Status;
  public readonly byte Data1;
  public readonly byte Data2;
  public readonly uint Tick;

  public int Channel => (int) this.Status & 15;

  public int Command => (int) this.Status & 240 /*0xF0*/;

  public RobustMidiCommand MidiCommand => (RobustMidiCommand) this.Command;

  public byte Key => this.Data1;

  public byte Velocity => this.Data2;

  public byte Control => this.Data1;

  public byte Value => this.Data2;

  public int Pitch => (int) this.Data2 << 8 | (int) this.Data1;

  public byte Pressure => this.Data1;

  public byte Program => this.Data1;

  public RobustMidiEvent(byte status, byte data1, byte data2, uint tick)
  {
    this.Status = status;
    this.Data1 = data1;
    this.Data2 = data2;
    this.Tick = tick;
  }

  public RobustMidiEvent(RobustMidiEvent ev, uint tick)
  {
    this.Status = ev.Status;
    this.Data1 = ev.Data1;
    this.Data2 = ev.Data2;
    this.Tick = tick;
  }

  public override string ToString()
  {
    return $"{base.ToString()} >> CHANNEL: 0x{this.Channel:X} || COMMAND: 0x{this.Command:X} {this.MidiCommand} || DATA1: 0x{this.Data1:X} || DATA2: 0x{this.Data2:X} || TICK: {this.Tick} <<";
  }

  public static byte MakeStatus(byte channel, byte command)
  {
    return (byte) ((uint) command | (uint) channel);
  }

  public static byte MakeStatus(byte channel, RobustMidiCommand command)
  {
    return RobustMidiEvent.MakeStatus(channel, (byte) command);
  }

  public static RobustMidiEvent NoteOff(byte channel, byte key, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.NoteOff), key, (byte) 0, tick);
  }

  public static RobustMidiEvent NoteOn(byte channel, byte key, byte velocity, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.NoteOn), key, velocity, tick);
  }

  public static RobustMidiEvent AfterTouch(byte channel, byte key, byte value, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.AfterTouch), key, value, tick);
  }

  public static RobustMidiEvent ControlChange(byte channel, byte control, byte value, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.ControlChange), control, value, tick);
  }

  public static RobustMidiEvent ProgramChange(byte channel, byte program, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.ProgramChange), program, (byte) 0, tick);
  }

  public static RobustMidiEvent ChannelPressure(byte channel, byte pressure, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.ChannelPressure), pressure, (byte) 0, tick);
  }

  public static RobustMidiEvent PitchBend(byte channel, ushort pitch, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.PitchBend), (byte) pitch, (byte) ((uint) pitch >> 8), tick);
  }

  public static RobustMidiEvent BankSelect(byte channel, byte bank, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.ControlChange), (byte) 0, bank, tick);
  }

  public static RobustMidiEvent AllNotesOff(byte channel, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.SystemMessage), (byte) 11, (byte) 0, tick);
  }

  public static RobustMidiEvent ResetAllControllers(uint tick)
  {
    return new RobustMidiEvent((byte) 176 /*0xB0*/, (byte) 121, (byte) 0, tick);
  }

  public static RobustMidiEvent SystemMessage(byte channel, byte control, uint tick)
  {
    return new RobustMidiEvent(RobustMidiEvent.MakeStatus(channel, RobustMidiCommand.SystemMessage), control, (byte) 0, tick);
  }

  public static RobustMidiEvent SystemReset(uint tick)
  {
    return new RobustMidiEvent(byte.MaxValue, (byte) 0, (byte) 0, tick);
  }
}
