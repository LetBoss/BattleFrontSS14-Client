// Decompiled with JetBrains decompiler
// Type: Content.Shared.Instruments.MidiTrack
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Text;

#nullable enable
namespace Content.Shared.Instruments;

[NetSerializable]
[Serializable]
public sealed class MidiTrack
{
  public string? TrackName;
  public string? InstrumentName;
  public string? ProgramName;
  private const string Postfix = "…";

  public override string ToString()
  {
    return $"Track Name: {this.TrackName}; Instrument Name: {this.InstrumentName}; Program Name: {this.ProgramName}";
  }

  public void TruncateFields(int limit)
  {
    if (this.InstrumentName != null)
      this.InstrumentName = this.Truncate(this.InstrumentName, limit);
    if (this.TrackName != null)
      this.TrackName = this.Truncate(this.TrackName, limit);
    if (this.ProgramName == null)
      return;
    this.ProgramName = this.Truncate(this.ProgramName, limit);
  }

  public void SanitizeFields()
  {
    if (this.InstrumentName != null)
      this.InstrumentName = MidiTrack.Sanitize(this.InstrumentName);
    if (this.TrackName != null)
      this.TrackName = MidiTrack.Sanitize(this.TrackName);
    if (this.ProgramName == null)
      return;
    this.ProgramName = MidiTrack.Sanitize(this.ProgramName);
  }

  private string Truncate(string input, int limit)
  {
    if (string.IsNullOrEmpty(input) || limit <= 0 || input.Length <= limit)
      return input;
    int length = limit - "…".Length;
    return input.Substring(0, length) + "…";
  }

  private static string Sanitize(string input)
  {
    StringBuilder stringBuilder = new StringBuilder(input.Length);
    foreach (char c in input)
    {
      if (!char.IsControl(c) && c <= '\u007F')
        stringBuilder.Append(c);
    }
    return stringBuilder.ToString();
  }
}
