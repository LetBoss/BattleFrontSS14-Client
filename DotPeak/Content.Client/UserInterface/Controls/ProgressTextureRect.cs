// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.ProgressTextureRect
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class ProgressTextureRect : TextureRect
{
  public float Progress;
  private readonly ProgressColorSystem _progressColor;

  public ProgressTextureRect()
  {
    this._progressColor = IoCManager.Resolve<IEntityManager>().System<ProgressColorSystem>();
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    UIBox2 uiBox2 = this.Texture != null ? this.GetDrawDimensions(this.Texture) : UIBox2.FromDimensions(Vector2.Zero, Vector2i.op_Implicit(((Control) this).PixelSize));
    uiBox2.Top = Math.Max(uiBox2.Bottom - uiBox2.Bottom * this.Progress, 0.0f);
    handle.DrawRect(uiBox2, this._progressColor.GetProgressColor(this.Progress), true);
    base.Draw(handle);
  }
}
