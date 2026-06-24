using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Particles;

public sealed class CivLocalParticleOverlay : Overlay
{
	internal List<CivLocalParticle>? Particles;

	private DrawVertexUV2DColor[] _verts = Array.Empty<DrawVertexUV2DColor>();

	private readonly List<Texture> _texes = new List<Texture>();

	private readonly HashSet<Texture> _texSet = new HashSet<Texture>();

	public override OverlaySpace Space => (OverlaySpace)8;

	public override bool RequestScreenTexture => false;

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		List<CivLocalParticle> particles = Particles;
		if (particles != null)
		{
			return particles.Count > 0;
		}
		return false;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		List<CivLocalParticle> particles = Particles;
		if (particles == null || particles.Count == 0)
		{
			return;
		}
		if (_verts.Length < particles.Count * 6)
		{
			_verts = (DrawVertexUV2DColor[])(object)new DrawVertexUV2DColor[particles.Count * 6];
		}
		_texes.Clear();
		_texSet.Clear();
		foreach (CivLocalParticle item in particles)
		{
			if (_texSet.Add(item.Tex))
			{
				_texes.Add(item.Tex);
			}
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		foreach (Texture tex in _texes)
		{
			int num = 0;
			foreach (CivLocalParticle item2 in particles)
			{
				CivLocalParticle current3 = item2;
				if (current3.Tex == tex)
				{
					float num2 = ((current3.Life > 0f) ? Math.Clamp(current3.Age / current3.Life, 0f, 1f) : 1f);
					float num3 = current3.A0 + (current3.A1 - current3.A0) * num2;
					if (!(num3 <= 0.001f))
					{
						float num4 = current3.Size0 + (current3.Size1 - current3.Size0) * num2;
						Color val = Color.FromSrgb(((Color)(ref current3.Rgb)).WithAlpha(MathF.Min(num3, 1f)));
						Vector2 vector = ((current3.Vel.LengthSquared() > 0.0001f) ? Vector2.Normalize(current3.Vel) : new Vector2(0f, -1f));
						Vector2 vector2 = new Vector2(vector.Y, 0f - vector.X);
						float num5 = num4 * 0.5f;
						float num6 = num4 * current3.Stretch * 0.5f;
						Vector2 vector3 = current3.Pos - vector2 * num5 - vector * num6;
						Vector2 vector4 = current3.Pos + vector2 * num5 - vector * num6;
						Vector2 vector5 = current3.Pos + vector2 * num5 + vector * num6;
						Vector2 vector6 = current3.Pos - vector2 * num5 + vector * num6;
						int num7 = num * 6;
						_verts[num7] = new DrawVertexUV2DColor(vector3, new Vector2(0f, 1f), val);
						_verts[num7 + 1] = new DrawVertexUV2DColor(vector4, new Vector2(1f, 1f), val);
						_verts[num7 + 2] = new DrawVertexUV2DColor(vector5, new Vector2(1f, 0f), val);
						_verts[num7 + 3] = new DrawVertexUV2DColor(vector3, new Vector2(0f, 1f), val);
						_verts[num7 + 4] = new DrawVertexUV2DColor(vector5, new Vector2(1f, 0f), val);
						_verts[num7 + 5] = new DrawVertexUV2DColor(vector6, new Vector2(0f, 0f), val);
						num++;
					}
				}
			}
			if (num > 0)
			{
				((DrawingHandleBase)worldHandle).DrawPrimitives((DrawPrimitiveTopology)1, tex, (ReadOnlySpan<DrawVertexUV2DColor>)_verts.AsSpan(0, num * 6));
			}
		}
	}
}
