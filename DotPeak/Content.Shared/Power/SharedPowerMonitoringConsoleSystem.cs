// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.SharedPowerMonitoringConsoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Power;

public abstract class SharedPowerMonitoringConsoleSystem : EntitySystem
{
  public const int ChunkSize = 5;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int GetFlag(Vector2i relativeTile) => 1 << relativeTile.X * 5 + relativeTile.Y;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetTileFromIndex(int index) => new Vector2i(index / 5, index % 5);
}
