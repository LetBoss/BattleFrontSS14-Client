// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.NetProbe.PubgNetProbeConsts
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared._PUBG.NetProbe;

public static class PubgNetProbeConsts
{
  public const int BytesInKb = 1024 /*0x0400*/;
  public const int MaxKb = 3072 /*0x0C00*/;
  public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5L);

  public static int ToBytes(int kb) => kb * 1024 /*0x0400*/;

  public static bool IsValidKb(int kb) => kb > 0 && kb <= 3072 /*0x0C00*/;
}
