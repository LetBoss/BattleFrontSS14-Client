// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Admin.RMCAdminFactionMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Admin;

[NetSerializable]
[Serializable]
public sealed class RMCAdminFactionMsg(RMCAdminFactionMsgType type, string left, string right) : 
  EuiMessageBase
{
  public readonly RMCAdminFactionMsgType Type = type;
  public readonly string Left = left;
  public readonly string Right = right;
}
