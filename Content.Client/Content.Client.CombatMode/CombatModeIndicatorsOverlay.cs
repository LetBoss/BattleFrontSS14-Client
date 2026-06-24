using System.Numerics;
using Content.Client._RMC14.Emplacements;
using Content.Client.Hands.Systems;
using Content.Shared._RMC14.CombatMode;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Wieldable.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.CombatMode;

public sealed class CombatModeIndicatorsOverlay : Overlay
{
	private readonly IInputManager _inputManager;

	private readonly IEntityManager _entMan;

	private readonly IEyeManager _eye;

	private readonly CombatModeSystem _combat;

	private readonly HandsSystem _hands;

	private readonly RMCCombatModeSystem _rmcCombatMode;

	private readonly SpriteSystem _sprite;

	private readonly RMCWeaponControllerSystem _rmcWeaponController;

	private readonly Texture _gunSight;

	private readonly Texture _gunBoltSight;

	private readonly Texture _meleeSight;

	public Color MainColor;

	public Color StrokeColor;

	public float Scale;

	public override OverlaySpace Space => (OverlaySpace)2;

	public CombatModeIndicatorsOverlay(IInputManager input, IEntityManager entMan, IEyeManager eye, CombatModeSystem combatSys, HandsSystem hands)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		Color val = Color.White;
		MainColor = ((Color)(ref val)).WithAlpha(0.3f);
		val = Color.Black;
		StrokeColor = ((Color)(ref val)).WithAlpha(0.5f);
		Scale = 0.6f;
		((Overlay)this)._002Ector();
		_inputManager = input;
		_entMan = entMan;
		_eye = eye;
		_combat = combatSys;
		_hands = hands;
		SpriteSystem entitySystem = _entMan.EntitySysManager.GetEntitySystem<SpriteSystem>();
		_gunSight = entitySystem.Frame0((SpriteSpecifier)new Rsi(new ResPath("/Textures/Interface/Misc/crosshair_pointers.rsi"), "gun_sight"));
		_gunBoltSight = entitySystem.Frame0((SpriteSpecifier)new Rsi(new ResPath("/Textures/Interface/Misc/crosshair_pointers.rsi"), "gun_bolt_sight"));
		_meleeSight = entitySystem.Frame0((SpriteSpecifier)new Rsi(new ResPath("/Textures/Interface/Misc/crosshair_pointers.rsi"), "melee_sight"));
		_rmcCombatMode = entMan.System<RMCCombatModeSystem>();
		_sprite = entMan.System<SpriteSystem>();
		_rmcWeaponController = entMan.System<RMCWeaponControllerSystem>();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		if (!_combat.IsInCombatMode())
		{
			return false;
		}
		return ((Overlay)this).BeforeDraw(ref args);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		if (_eye.PixelToMap(mouseScreenPosition).MapId != args.MapId)
		{
			return;
		}
		EntityUid? activeHandEntity = _hands.GetActiveHandEntity();
		bool num = _entMan.HasComponent<GunComponent>(activeHandEntity);
		bool flag = true;
		ChamberMagazineAmmoProviderComponent chamberMagazineAmmoProviderComponent = default(ChamberMagazineAmmoProviderComponent);
		if (_entMan.TryGetComponent<ChamberMagazineAmmoProviderComponent>(activeHandEntity, ref chamberMagazineAmmoProviderComponent))
		{
			flag = chamberMagazineAmmoProviderComponent.BoltClosed ?? true;
		}
		Vector2 position = mouseScreenPosition.Position;
		IViewportControl viewportControl = args.ViewportControl;
		IViewportControl obj = ((viewportControl is Control) ? viewportControl : null);
		float num2 = ((obj != null) ? ((Control)obj).UIScale : 1f);
		float num3 = ((num2 > 1.25f) ? 1.25f : num2);
		EntityUid? val = activeHandEntity;
		if (_rmcWeaponController.TryGetControllingWeapon(out var weapon))
		{
			val = weapon;
		}
		Texture sight = ((!num) ? _meleeSight : (flag ? _gunSight : _gunBoltSight));
		if (val.HasValue)
		{
			Rsi crosshair = _rmcCombatMode.GetCrosshair(Entity<WieldedCrosshairComponent, WieldableComponent>.op_Implicit(val.Value));
			if (crosshair != null)
			{
				sight = _sprite.Frame0((SpriteSpecifier)(object)crosshair);
				Vector2 vector = sight.Size * num3;
				UIBox2 val2 = UIBox2.FromDimensions(position - vector * 0.5f, vector);
				((OverlayDrawArgs)(ref args)).ScreenHandle.DrawTextureRect(sight, val2, (Color?)null);
				return;
			}
		}
		DrawSight(sight, ((OverlayDrawArgs)(ref args)).ScreenHandle, position, num3 * Scale);
	}

	private void DrawSight(Texture sight, DrawingHandleScreen screen, Vector2 centerPos, float scale)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = sight.Size * scale;
		Vector2 vector2 = vector + new Vector2(7f, 7f);
		screen.DrawTextureRect(sight, UIBox2.FromDimensions(centerPos - vector * 0.5f, vector), (Color?)StrokeColor);
		screen.DrawTextureRect(sight, UIBox2.FromDimensions(centerPos - vector2 * 0.5f, vector2), (Color?)MainColor);
	}
}
