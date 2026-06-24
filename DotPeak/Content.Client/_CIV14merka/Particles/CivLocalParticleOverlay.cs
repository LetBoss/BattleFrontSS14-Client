// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Particles.CivLocalParticleOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Particles;

public sealed class CivLocalParticleOverlay : Overlay
{
  internal List<CivLocalParticle>? Particles;
  private DrawVertexUV2DColor[] _verts = Array.Empty<DrawVertexUV2DColor>();
  private readonly List<Texture> _texes = new List<Texture>();
  private readonly HashSet<Texture> _texSet = new HashSet<Texture>();

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public virtual bool RequestScreenTexture => false;

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    List<CivLocalParticle> particles = this.Particles;
    return particles != null && particles.Count > 0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    List<CivLocalParticle> particles = this.Particles;
    if (particles == null || particles.Count == 0)
      return;
    if (this._verts.Length < particles.Count * 6)
      this._verts = new DrawVertexUV2DColor[particles.Count * 6];
    this._texes.Clear();
    this._texSet.Clear();
    foreach (CivLocalParticle civLocalParticle in particles)
    {
      if (this._texSet.Add(civLocalParticle.Tex))
        this._texes.Add(civLocalParticle.Tex);
    }
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    foreach (Texture tex in this._texes)
    {
      int num1 = 0;
      foreach (CivLocalParticle civLocalParticle in particles)
      {
        if (civLocalParticle.Tex == tex)
        {
          float num2 = (double) civLocalParticle.Life > 0.0 ? Math.Clamp(civLocalParticle.Age / civLocalParticle.Life, 0.0f, 1f) : 1f;
          float x = civLocalParticle.A0 + (civLocalParticle.A1 - civLocalParticle.A0) * num2;
          if ((double) x > 1.0 / 1000.0)
          {
            double num3 = (double) civLocalParticle.Size0 + ((double) civLocalParticle.Size1 - (double) civLocalParticle.Size0) * (double) num2;
            Color color = Color.FromSrgb(((Color) ref civLocalParticle.Rgb).WithAlpha(MathF.Min(x, 1f)));
            Vector2 vector2_1 = (double) civLocalParticle.Vel.LengthSquared() > 9.9999997473787516E-05 ? Vector2.Normalize(civLocalParticle.Vel) : new Vector2(0.0f, -1f);
            Vector2 vector2_2 = new Vector2(vector2_1.Y, -vector2_1.X);
            float num4 = (float) (num3 * 0.5);
            float num5 = (float) (num3 * (double) civLocalParticle.Stretch * 0.5);
            Vector2 vector2_3 = civLocalParticle.Pos - vector2_2 * num4 - vector2_1 * num5;
            Vector2 vector2_4 = civLocalParticle.Pos + vector2_2 * num4 - vector2_1 * num5;
            Vector2 vector2_5 = civLocalParticle.Pos + vector2_2 * num4 + vector2_1 * num5;
            Vector2 vector2_6 = civLocalParticle.Pos - vector2_2 * num4 + vector2_1 * num5;
            int index = num1 * 6;
            this._verts[index] = new DrawVertexUV2DColor(vector2_3, new Vector2(0.0f, 1f), color);
            this._verts[index + 1] = new DrawVertexUV2DColor(vector2_4, new Vector2(1f, 1f), color);
            this._verts[index + 2] = new DrawVertexUV2DColor(vector2_5, new Vector2(1f, 0.0f), color);
            this._verts[index + 3] = new DrawVertexUV2DColor(vector2_3, new Vector2(0.0f, 1f), color);
            this._verts[index + 4] = new DrawVertexUV2DColor(vector2_5, new Vector2(1f, 0.0f), color);
            this._verts[index + 5] = new DrawVertexUV2DColor(vector2_6, new Vector2(0.0f, 0.0f), color);
            ++num1;
          }
        }
      }
      if (num1 > 0)
        ((DrawingHandleBase) worldHandle).DrawPrimitives((DrawPrimitiveTopology) 1, tex, (ReadOnlySpan<DrawVertexUV2DColor>) this._verts.AsSpan<DrawVertexUV2DColor>(0, num1 * 6));
    }
  }
}
