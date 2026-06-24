// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.ParallaxControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Parallax.Managers;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Parallax;

public sealed class ParallaxControl : Control
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IParallaxManager _parallaxManager;
  [Dependency]
  private IRobustRandom _random;
  private string _parallaxPrototype = "FastSpace";

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 Offset { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float SpeedX { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float SpeedY { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float ScaleX { get; set; } = 1f;

  [Robust.Shared.ViewVariables.ViewVariables]
  public float ScaleY { get; set; } = 1f;

  [Robust.Shared.ViewVariables.ViewVariables]
  public string ParallaxPrototype
  {
    get => this._parallaxPrototype;
    set
    {
      this._parallaxPrototype = value;
      this._parallaxManager.LoadParallaxByName(value);
    }
  }

  public ParallaxControl()
  {
    IoCManager.InjectDependencies<ParallaxControl>(this);
    this.Offset = new Vector2((float) this._random.Next(0, 1000), (float) this._random.Next(0, 1000));
    this.RectClipContent = true;
    this._parallaxManager.LoadParallaxByName(this._parallaxPrototype);
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    float totalSeconds = (float) this._timing.RealTime.TotalSeconds;
    Vector2 vector2_1 = this.Offset + new Vector2(totalSeconds * this.SpeedX, totalSeconds * this.SpeedY);
    foreach (ParallaxLayerPrepared parallaxLayer in this._parallaxManager.GetParallaxLayers(this._parallaxPrototype))
    {
      Texture texture = parallaxLayer.Texture;
      Vector2i vector2i1;
      // ISSUE: explicit constructor call
      ((Vector2i) ref vector2i1).\u002Ector((int) ((double) texture.Size.X * (double) this.Size.X * (double) parallaxLayer.Config.Scale.X / 1920.0 * (double) this.ScaleX), (int) ((double) texture.Size.Y * (double) this.Size.X * (double) parallaxLayer.Config.Scale.Y / 1920.0 * (double) this.ScaleY));
      Vector2i pixelSize = this.PixelSize;
      vector2i1.X = Math.Max(vector2i1.X, 1);
      vector2i1.Y = Math.Max(vector2i1.Y, 1);
      if (parallaxLayer.Config.Tiled)
      {
        Vector2i vector2i2 = Vector2Helpers.Floored(vector2_1 * parallaxLayer.Config.Slowness);
        vector2i2.X %= vector2i1.X;
        vector2i2.Y %= vector2i1.Y;
        for (int x = -vector2i2.X; x < pixelSize.X; x += vector2i1.X)
        {
          for (int y = -vector2i2.Y; y < pixelSize.Y; y += vector2i1.Y)
            handle.DrawTextureRect(texture, UIBox2.FromDimensions(new Vector2((float) x, (float) y), Vector2i.op_Implicit(vector2i1)), new Color?());
        }
      }
      else
      {
        Vector2 vector2_2 = Vector2i.op_Implicit(Vector2i.op_Division(Vector2i.op_Subtraction(pixelSize, vector2i1), 2)) + parallaxLayer.Config.ControlHomePosition;
        handle.DrawTextureRect(texture, UIBox2.FromDimensions(vector2_2, Vector2i.op_Implicit(vector2i1)), new Color?());
      }
    }
  }
}
