using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Content.Shared.Parallax.Biomes;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Parallax;

public sealed class BiomeDebugOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IResourceCache _cache;

	[Dependency]
	private ITileDefinitionManager _tileDefManager;

	private BiomeSystem _biomes;

	private SharedMapSystem _maps;

	private Font _font;

	public override OverlaySpace Space => (OverlaySpace)2;

	public BiomeDebugOverlay()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		IoCManager.InjectDependencies<BiomeDebugOverlay>(this);
		_biomes = _entManager.System<BiomeSystem>();
		_maps = _entManager.System<SharedMapSystem>();
		_font = (Font)new VectorFont(_cache.GetResource<FontResource>("/EngineFonts/NotoSans/NotoSans-Regular.ttf", true), 12);
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid mapOrInvalid = _maps.GetMapOrInvalid((MapId?)args.MapId);
		return _entManager.HasComponent<BiomeComponent>(mapOrInvalid);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		MapCoordinates val = _eyeManager.ScreenToMap(mouseScreenPosition);
		if (val.MapId == MapId.Nullspace || val.MapId != args.MapId)
		{
			return;
		}
		EntityUid mapOrInvalid = _maps.GetMapOrInvalid((MapId?)args.MapId);
		BiomeComponent biomeComponent = default(BiomeComponent);
		MapGridComponent val2 = default(MapGridComponent);
		if (!_entManager.TryGetComponent<BiomeComponent>(mapOrInvalid, ref biomeComponent) || !_entManager.TryGetComponent<MapGridComponent>(mapOrInvalid, ref val2))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		Vector2i indices = _maps.WorldToTile(mapOrInvalid, val2, val.Position);
		if (_biomes.TryGetEntity(indices, biomeComponent, (Entity<MapGridComponent>?)Entity<MapGridComponent>.op_Implicit((mapOrInvalid, val2)), out string entity))
		{
			string value = "Entity: " + entity;
			stringBuilder.AppendLine(value);
		}
		if (_biomes.TryGetDecals(indices, biomeComponent.Layers, biomeComponent.Seed, (Entity<MapGridComponent>?)Entity<MapGridComponent>.op_Implicit((mapOrInvalid, val2)), out List<(string, Vector2)> decals))
		{
			string value2 = $"Decals: {decals.Count}";
			stringBuilder.AppendLine(value2);
			foreach (var item in decals)
			{
				string value3 = "- " + item.Item1;
				stringBuilder.AppendLine(value3);
			}
		}
		if (_biomes.TryGetBiomeTile(indices, biomeComponent.Layers, biomeComponent.Seed, (Entity<MapGridComponent>?)Entity<MapGridComponent>.op_Implicit((mapOrInvalid, val2)), out Tile? tile))
		{
			string value4 = "Tile: " + ((IPrototype)_tileDefManager[tile.Value.TypeId]).ID;
			stringBuilder.AppendLine(value4);
		}
		((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, mouseScreenPosition.Position + new Vector2(0f, 32f), stringBuilder.ToString());
	}
}
