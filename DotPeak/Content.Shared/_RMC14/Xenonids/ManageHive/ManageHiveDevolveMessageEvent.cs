// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.ManageHiveDevolveMessageEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ManageHive;

[NetSerializable]
[Serializable]
public sealed record ManageHiveDevolveMessageEvent(EntProtoId Choice, string Message = "") : 
  DialogInputEvent(Message)
{
  public EntProtoId Choice { get; init; } = Choice;

  [CompilerGenerated]
  public sealed override bool Equals(DialogInputEvent? other) => this.Equals((object) other);

  [CompilerGenerated]
  public void Deconstruct(out EntProtoId Choice, out string Message)
  {
    Choice = this.Choice;
    Message = this.Message;
  }
}
