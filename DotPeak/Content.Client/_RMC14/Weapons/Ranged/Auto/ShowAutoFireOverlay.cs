// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.Auto.ShowAutoFireOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Weapons.Ranged.Auto;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged.Auto;

public sealed class ShowAutoFireOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  private readonly GunToggleableAutoFireSystem _autoFire;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public ShowAutoFireOverlay()
  {
    IoCManager.InjectDependencies<ShowAutoFireOverlay>(this);
    this._autoFire = this._entity.System<GunToggleableAutoFireSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ReadOnlySpan<Vector2> vertices = (ReadOnlySpan<Vector2>) this._autoFire.Shape.Vertices;
    Color red = Color.Red;
    Color color = ((Color) ref red).WithAlpha(0.5f);
    ((DrawingHandleBase) worldHandle).DrawPrimitives((DrawPrimitiveTopology) 2, vertices, color);
  }
}
