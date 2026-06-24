// Decompiled with JetBrains decompiler
// Type: Content.Client.Instruments.MidiParser.MidiInstrumentExt
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Utility;

#nullable enable
namespace Content.Client.Instruments.MidiParser;

public static class MidiInstrumentExt
{
  public static string GetStringRep(this MidiInstrument instrument)
  {
    return CaseConversion.PascalToKebab(instrument.ToString());
  }
}
