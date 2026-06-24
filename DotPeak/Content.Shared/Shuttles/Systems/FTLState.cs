// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.Systems.FTLState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Shuttles.Systems;

[Flags]
public enum FTLState : byte
{
  Invalid = 0,
  Available = 1,
  Starting = 2,
  Travelling = 4,
  Arriving = 8,
  Cooldown = 16, // 0x10
}
