// Decompiled with JetBrains decompiler
// Type: Content.Shared.PDA.Ringer.RingerSetRingtoneMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.PDA.Ringer;

[NetSerializable]
[Serializable]
public sealed class RingerSetRingtoneMessage : BoundUserInterfaceMessage
{
  public Note[] Ringtone { get; }

  public RingerSetRingtoneMessage(Note[] ringTone) => this.Ringtone = ringTone;
}
