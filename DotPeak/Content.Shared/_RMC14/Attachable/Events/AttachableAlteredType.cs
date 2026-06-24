// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Events.AttachableAlteredType
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable disable
namespace Content.Shared._RMC14.Attachable.Events;

public enum AttachableAlteredType : byte
{
  Attached = 1,
  Detached = 2,
  Wielded = 4,
  Unwielded = 8,
  Activated = 16, // 0x10
  Deactivated = 32, // 0x20
  DetachedDeactivated = 34, // 0x22
  Interrupted = 64, // 0x40
  AppearanceChanged = 128, // 0x80
}
