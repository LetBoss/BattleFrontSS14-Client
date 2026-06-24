// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pinpointer.NavMapChunkType
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable disable
namespace Content.Shared.Pinpointer;

public enum NavMapChunkType : byte
{
  Floor = 0,
  Wall = 4,
  Airlock = 8,
  Invalid = 255, // 0xFF
}
