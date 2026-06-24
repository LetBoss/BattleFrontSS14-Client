// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.ManageHiveJellyMessageEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ManageHive;

[NetSerializable]
[Serializable]
public sealed record ManageHiveJellyMessageEvent(NetEntity Xeno, string Name, string Message = "") : 
  DialogInputEvent(Message)
{
  public NetEntity Xeno { get; init; } = Xeno;

  public string Name { get; init; } = Name;

  [CompilerGenerated]
  public sealed override bool Equals(DialogInputEvent? other) => this.Equals((object) other);

  [CompilerGenerated]
  public void Deconstruct(out NetEntity Xeno, out string Name, out string Message)
  {
    Xeno = this.Xeno;
    Name = this.Name;
    Message = this.Message;
  }
}
