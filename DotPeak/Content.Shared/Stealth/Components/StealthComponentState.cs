// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stealth.Components.StealthComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Stealth.Components;

[NetSerializable]
[Serializable]
public sealed class StealthComponentState : ComponentState
{
  public readonly float Visibility;
  public readonly TimeSpan? LastUpdated;
  public readonly bool Enabled;

  public StealthComponentState(float stealthLevel, TimeSpan? lastUpdated, bool enabled)
  {
    this.Visibility = stealthLevel;
    this.LastUpdated = lastUpdated;
    this.Enabled = enabled;
  }
}
