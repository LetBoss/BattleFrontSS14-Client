using System;
using System.Numerics;
using Content.Shared._CIV14merka.Pvo;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Pvo;

public sealed class CivPvoCounterOverlay : Overlay
{
	[Dependency]
	private readonly IEyeManager _eye;

	[Dependency]
	private readonly IPlayerManager _player;

	[Dependency]
	private readonly IResourceCache _cache;

	[Dependency]
	private readonly IEntityManager _entity;

	private readonly SharedTransformSystem _xform;

	private readonly Font _font;

	public override OverlaySpace Space => (OverlaySpace)2;

	public CivPvoCounterOverlay()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		IoCManager.InjectDependencies<CivPvoCounterOverlay>(this);
		_xform = _entity.System<SharedTransformSystem>();
		_font = (Font)new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), 12);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (!_entity.TryGetComponent<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent) || string.IsNullOrWhiteSpace(civTeamMemberComponent.SideId))
		{
			return;
		}
		MapId mapId = args.MapId;
		if (mapId == MapId.Nullspace)
		{
			return;
		}
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		EntityQueryEnumerator<CivPvoComponent, TransformComponent> val = _entity.EntityQueryEnumerator<CivPvoComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		CivPvoComponent civPvoComponent = default(CivPvoComponent);
		TransformComponent val3 = default(TransformComponent);
		UIBox2 val5 = default(UIBox2);
		while (val.MoveNext(ref val2, ref civPvoComponent, ref val3))
		{
			if (!(val3.MapID != mapId) && !string.IsNullOrWhiteSpace(civPvoComponent.SideId) && string.Equals(civPvoComponent.SideId, civTeamMemberComponent.SideId, StringComparison.OrdinalIgnoreCase))
			{
				int num = civPvoComponent.Tier1Charges + civPvoComponent.Tier2Charges;
				string text = (civPvoComponent.Infinite ? Loc.GetString("civ-eq-pvo-counter-infinite") : Loc.GetString("civ-eq-pvo-counter", new(string, object)[3]
				{
					("total", num),
					("max", civPvoComponent.MaxCharges),
					("tier2", civPvoComponent.Tier2Charges)
				}));
				Vector2 vector = _xform.GetWorldPosition(val2) + new Vector2(0f, 1.1f);
				Vector2 vector2 = _eye.WorldToScreen(vector);
				Vector2 dimensions = screenHandle.GetDimensions(_font, (ReadOnlySpan<char>)text, 1f);
				if (!(dimensions.X <= 0f))
				{
					float num2 = dimensions.X * 0.5f;
					Vector2 vector3 = new Vector2(vector2.X - num2, vector2.Y - dimensions.Y);
					Color val4 = ((num > 0) ? Color.LightGreen : Color.OrangeRed);
					((UIBox2)(ref val5))._002Ector(vector3.X - 3f, vector3.Y - 2f, vector3.X + dimensions.X + 3f, vector3.Y + dimensions.Y + 2f);
					UIBox2 val6 = val5;
					Color black = Color.Black;
					screenHandle.DrawRect(val6, ((Color)(ref black)).WithAlpha(0.55f), true);
					screenHandle.DrawString(_font, vector3, (ReadOnlySpan<char>)text, 1f, val4);
				}
			}
		}
	}
}
