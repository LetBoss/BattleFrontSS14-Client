// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.IBudgetEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable enable
namespace Content.Shared.Random;

public interface IBudgetEntry : IProbEntry
{
  float Cost { get; set; }

  string Proto { get; set; }
}
