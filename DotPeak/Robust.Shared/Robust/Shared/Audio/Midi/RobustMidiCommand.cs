// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Midi.RobustMidiCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Audio.Midi;

public enum RobustMidiCommand : byte
{
  NoteOff = 128, // 0x80
  NoteOn = 144, // 0x90
  AfterTouch = 160, // 0xA0
  ControlChange = 176, // 0xB0
  ProgramChange = 192, // 0xC0
  ChannelPressure = 208, // 0xD0
  PitchBend = 224, // 0xE0
  SystemMessage = 240, // 0xF0
}
