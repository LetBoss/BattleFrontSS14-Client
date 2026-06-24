// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.Systems.SharedRadarConsoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Shuttles.Systems;

public abstract class SharedRadarConsoleSystem : EntitySystem
{
  public const float DefaultMaxRange = 256f;

  protected virtual void UpdateState(EntityUid uid, RadarConsoleComponent component)
  {
  }

  public void SetRange(EntityUid uid, float value, RadarConsoleComponent component)
  {
    if (component.MaxRange.Equals(value))
      return;
    component.MaxRange = value;
    this.Dirty(uid, (IComponent) component);
    this.UpdateState(uid, component);
  }
}
