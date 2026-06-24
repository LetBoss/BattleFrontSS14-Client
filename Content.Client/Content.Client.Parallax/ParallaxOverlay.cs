using System.Numerics;
using Content.Client.Parallax.Managers;
using Content.Shared.CCVar;
using Content.Shared.Parallax.Biomes;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Parallax;

public sealed class ParallaxOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IConfigurationManager _configurationManager;

	[Dependency]
	private IParallaxManager _manager;

	private readonly SharedMapSystem _mapSystem;

	private readonly ParallaxSystem _parallax;

	public override OverlaySpace Space => (OverlaySpace)256;

	public ParallaxOverlay()
	{
		((Overlay)this).ZIndex = 0;
		IoCManager.InjectDependencies<ParallaxOverlay>(this);
		_mapSystem = _entManager.System<SharedMapSystem>();
		_parallax = _entManager.System<ParallaxSystem>();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (args.MapId == MapId.Nullspace || _entManager.HasComponent<BiomeComponent>(_mapSystem.GetMapOrInvalid((MapId?)args.MapId)))
		{
			return false;
		}
		return true;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		if (args.MapId == MapId.Nullspace || !_configurationManager.GetCVar<bool>(CCVars.ParallaxEnabled))
		{
			return;
		}
		IEye eye = args.Viewport.Eye;
		Vector2 vector = ((eye != null) ? eye.Position.Position : Vector2.Zero);
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		ParallaxLayerPrepared[] parallaxLayers = _parallax.GetParallaxLayers(args.MapId);
		float num = (float)_timing.RealTime.TotalSeconds;
		ParallaxLayerPrepared[] array = parallaxLayers;
		for (int i = 0; i < array.Length; i++)
		{
			ParallaxLayerPrepared parallaxLayerPrepared = array[i];
			ShaderInstance val = (string.IsNullOrEmpty(parallaxLayerPrepared.Config.Shader) ? null : _prototypeManager.Index<ShaderPrototype>(parallaxLayerPrepared.Config.Shader).Instance());
			((DrawingHandleBase)worldHandle).UseShader(val);
			Texture texture = parallaxLayerPrepared.Texture;
			Vector2 vector2 = texture.Size / 32f * parallaxLayerPrepared.Config.Scale;
			Vector2 vector3 = parallaxLayerPrepared.Config.WorldHomePosition + _manager.ParallaxAnchor;
			Vector2 vector4 = parallaxLayerPrepared.Config.Scrolling * num;
			Vector2 vector5 = (vector - vector3) * parallaxLayerPrepared.Config.Slowness + vector4;
			vector5 += vector3;
			vector5 += parallaxLayerPrepared.Config.WorldAdjustPosition;
			vector5 -= vector2 / 2f;
			if (parallaxLayerPrepared.Config.Tiled)
			{
				Vector2 vector6 = args.WorldAABB.BottomLeft - vector5;
				vector6 = Vector2i.op_Implicit(Vector2Helpers.Floored(vector6 / vector2)) * vector2;
				vector6 += vector5;
				for (float num2 = vector6.X; num2 < args.WorldAABB.Right; num2 += vector2.X)
				{
					for (float num3 = vector6.Y; num3 < args.WorldAABB.Top; num3 += vector2.Y)
					{
						worldHandle.DrawTextureRect(texture, Box2.FromDimensions(new Vector2(num2, num3), vector2), (Color?)null);
					}
				}
			}
			else
			{
				worldHandle.DrawTextureRect(texture, Box2.FromDimensions(vector5, vector2), (Color?)null);
			}
		}
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
