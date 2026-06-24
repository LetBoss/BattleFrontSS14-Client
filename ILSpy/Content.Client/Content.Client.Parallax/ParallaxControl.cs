using System;
using System.Numerics;
using Content.Client.Parallax.Managers;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

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

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Vector2 Offset { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float SpeedX { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float SpeedY { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ScaleX { get; set; } = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ScaleY { get; set; } = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ParallaxPrototype
	{
		get
		{
			return _parallaxPrototype;
		}
		set
		{
			_parallaxPrototype = value;
			_parallaxManager.LoadParallaxByName(value);
		}
	}

	public ParallaxControl()
	{
		IoCManager.InjectDependencies<ParallaxControl>(this);
		Offset = new Vector2(_random.Next(0, 1000), _random.Next(0, 1000));
		((Control)this).RectClipContent = true;
		_parallaxManager.LoadParallaxByName(_parallaxPrototype);
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)_timing.RealTime.TotalSeconds;
		Vector2 vector = Offset + new Vector2(num * SpeedX, num * SpeedY);
		ParallaxLayerPrepared[] parallaxLayers = _parallaxManager.GetParallaxLayers(_parallaxPrototype);
		Vector2i val = default(Vector2i);
		for (int i = 0; i < parallaxLayers.Length; i++)
		{
			ParallaxLayerPrepared parallaxLayerPrepared = parallaxLayers[i];
			Texture texture = parallaxLayerPrepared.Texture;
			((Vector2i)(ref val))._002Ector((int)((float)texture.Size.X * ((Control)this).Size.X * parallaxLayerPrepared.Config.Scale.X / 1920f * ScaleX), (int)((float)texture.Size.Y * ((Control)this).Size.X * parallaxLayerPrepared.Config.Scale.Y / 1920f * ScaleY));
			Vector2i pixelSize = ((Control)this).PixelSize;
			val.X = Math.Max(val.X, 1);
			val.Y = Math.Max(val.Y, 1);
			if (parallaxLayerPrepared.Config.Tiled)
			{
				Vector2i val2 = Vector2Helpers.Floored(vector * parallaxLayerPrepared.Config.Slowness);
				val2.X %= val.X;
				val2.Y %= val.Y;
				for (int j = -val2.X; j < pixelSize.X; j += val.X)
				{
					for (int k = -val2.Y; k < pixelSize.Y; k += val.Y)
					{
						handle.DrawTextureRect(texture, UIBox2.FromDimensions(new Vector2(j, k), Vector2i.op_Implicit(val)), (Color?)null);
					}
				}
			}
			else
			{
				Vector2 vector2 = Vector2i.op_Implicit((pixelSize - val) / 2) + parallaxLayerPrepared.Config.ControlHomePosition;
				handle.DrawTextureRect(texture, UIBox2.FromDimensions(vector2, Vector2i.op_Implicit(val)), (Color?)null);
			}
		}
	}
}
