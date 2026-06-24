// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Particles.CivLocalParticle
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Particles;

internal struct CivLocalParticle
{
  public Vector2 Pos;
  public Vector2 Vel;
  public Vector2 Gravity;
  public float Drag;
  public float Wind;
  public float Age;
  public float Life;
  public float Size0;
  public float Size1;
  public float A0;
  public float A1;
  public float Stretch;
  public Color Rgb;
  public Texture Tex;
}
