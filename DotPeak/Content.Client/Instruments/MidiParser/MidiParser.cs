// Decompiled with JetBrains decompiler
// Type: Content.Client.Instruments.MidiParser.MidiParser
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Instruments;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Content.Client.Instruments.MidiParser;

public static class MidiParser
{
  public static bool TryGetMidiTracks(byte[] data, [NotNullWhen(true)] out MidiTrack[]? tracks, [NotNullWhen(false)] out string? error)
  {
    tracks = (MidiTrack[]) null;
    error = (string) null;
    MidiStreamWrapper midiStreamWrapper = new MidiStreamWrapper(data);
    if (midiStreamWrapper.ReadString(4) != "MThd")
    {
      error = "Invalid file header";
      return false;
    }
    uint num1 = midiStreamWrapper.ReadUInt32();
    midiStreamWrapper.Skip(2);
    ushort num2 = midiStreamWrapper.ReadUInt16();
    midiStreamWrapper.Skip(2);
    midiStreamWrapper.Skip((int) num1 - 6);
    List<MidiTrack> midiTrackList = new List<MidiTrack>();
    for (int index = 0; index < (int) num2; ++index)
    {
      if (midiStreamWrapper.ReadString(4) != "MTrk")
      {
        tracks = (MidiTrack[]) null;
        error = "Track contains invalid header";
        return false;
      }
      MidiTrack midiTrack = new MidiTrack();
      uint num3 = midiStreamWrapper.ReadUInt32();
      long num4 = midiStreamWrapper.StreamPosition + (long) num3;
      bool flag = false;
      byte? nullable1 = new byte?();
      while (midiStreamWrapper.StreamPosition < num4)
      {
        int num5 = (int) midiStreamWrapper.ReadVariableLengthQuantity();
        byte num6 = midiStreamWrapper.ReadByte();
        if (num6 >= (byte) 128 /*0x80*/)
          nullable1 = new byte?(num6);
        else
          midiStreamWrapper.Skip(-1);
        if (!nullable1.HasValue)
        {
          tracks = (MidiTrack[]) null;
          error = "Track data not valid, expected status byte, got nothing.";
          return false;
        }
        byte? nullable2 = nullable1;
        byte num7 = (byte) (nullable2.HasValue ? new int?((int) nullable2.GetValueOrDefault() & 240 /*0xF0*/) : new int?()).Value;
        if (nullable1.HasValue)
        {
          switch (nullable1.GetValueOrDefault())
          {
            case 240 /*0xF0*/:
            case 247:
              uint count1 = midiStreamWrapper.ReadVariableLengthQuantity();
              midiStreamWrapper.Skip((int) count1);
              nullable1 = new byte?();
              continue;
            case byte.MaxValue:
              byte num8 = midiStreamWrapper.ReadByte();
              uint count2 = midiStreamWrapper.ReadVariableLengthQuantity();
              byte[] bytes = midiStreamWrapper.ReadBytes((int) count2);
              if (num8 != (byte) 0 && num8 >= (byte) 1 && num8 <= (byte) 15)
              {
                string str = Encoding.ASCII.GetString(bytes, 0, (int) count2);
                switch (num8)
                {
                  case 3:
                    if (midiTrack.TrackName == null)
                    {
                      midiTrack.TrackName = str;
                      continue;
                    }
                    continue;
                  case 4:
                    if (midiTrack.InstrumentName == null)
                    {
                      midiTrack.InstrumentName = str;
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              }
              else
                continue;
          }
        }
        switch (num7)
        {
          case 128 /*0x80*/:
          case 144 /*0x90*/:
          case 160 /*0xA0*/:
          case 176 /*0xB0*/:
          case 224 /*0xE0*/:
            flag = true;
            midiStreamWrapper.Skip(2);
            continue;
          case 192 /*0xC0*/:
            byte instrument = midiStreamWrapper.ReadByte();
            if (midiTrack.ProgramName == null && (int) instrument < Enum.GetValues<MidiInstrument>().Length)
            {
              midiTrack.ProgramName = Loc.GetString("instruments-component-menu-midi-channel-" + ((MidiInstrument) instrument).GetStringRep());
              continue;
            }
            continue;
          case 208 /*0xD0*/:
            flag = true;
            midiStreamWrapper.Skip(1);
            continue;
          default:
            error = $"Unknown MIDI event type {nullable1:X2}";
            tracks = (MidiTrack[]) null;
            return false;
        }
      }
      if (flag)
        midiTrackList.Add(midiTrack);
    }
    tracks = midiTrackList.ToArray();
    return true;
  }
}
