using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Maths;

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
