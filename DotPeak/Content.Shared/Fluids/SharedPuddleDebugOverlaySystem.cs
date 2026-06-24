// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.SharedPuddleDebugOverlaySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System;

#nullable disable
namespace Content.Shared.Fluids;

public abstract class SharedPuddleDebugOverlaySystem : EntitySystem
{
  protected const float LocalViewRange = 16f;
  protected TimeSpan? NextTick;
  protected TimeSpan Cooldown = TimeSpan.FromSeconds(0.5);
}
