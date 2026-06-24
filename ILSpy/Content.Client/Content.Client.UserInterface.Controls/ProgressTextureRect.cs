using System;
using System.Numerics;
using Content.Client.UserInterface.Systems;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

public sealed class ProgressTextureRect : TextureRect
{
	public float Progress;

	private readonly ProgressColorSystem _progressColor;

	public ProgressTextureRect()
	{
		_progressColor = IoCManager.Resolve<IEntityManager>().System<ProgressColorSystem>();
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = ((((TextureRect)this).Texture != null) ? ((TextureRect)this).GetDrawDimensions(((TextureRect)this).Texture) : UIBox2.FromDimensions(Vector2.Zero, Vector2i.op_Implicit(((Control)this).PixelSize)));
		val.Top = Math.Max(val.Bottom - val.Bottom * Progress, 0f);
		handle.DrawRect(val, _progressColor.GetProgressColor(Progress), true);
		((TextureRect)this).Draw(handle);
	}
}
