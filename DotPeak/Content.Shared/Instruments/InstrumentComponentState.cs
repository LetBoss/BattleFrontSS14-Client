// Decompiled with JetBrains decompiler
// Type: Content.Shared.Instruments.InstrumentComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections;

#nullable enable
namespace Content.Shared.Instruments;

[NetSerializable]
[Serializable]
public sealed class InstrumentComponentState : ComponentState
{
  public bool Playing;
  public byte InstrumentProgram;
  public byte InstrumentBank;
  public bool AllowPercussion;
  public bool AllowProgramChange;
  public bool RespectMidiLimits;
  public NetEntity? Master;
  public BitArray FilteredChannels;
}
