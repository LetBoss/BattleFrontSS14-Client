// Decompiled with JetBrains decompiler
// Type: Content.Shared.Rotation.SharedRotationVisualsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared.Rotation;

public abstract class SharedRotationVisualsSystem : EntitySystem
{
  public void SetHorizontalAngle(Entity<RotationVisualsComponent?> ent, Angle angle)
  {
    if (!this.Resolve<RotationVisualsComponent>((EntityUid) ent, ref ent.Comp, false) || ((Angle) ref ent.Comp.HorizontalRotation).Equals(angle))
      return;
    ent.Comp.HorizontalRotation = angle;
    this.Dirty<RotationVisualsComponent>(ent);
  }

  public void ResetHorizontalAngle(Entity<RotationVisualsComponent?> ent)
  {
    if (!this.Resolve<RotationVisualsComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    this.SetHorizontalAngle(ent, ent.Comp.DefaultRotation);
  }
}
