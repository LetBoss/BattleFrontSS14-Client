// Decompiled with JetBrains decompiler
// Type: Content.Client.CombatMode.CombatModeIndicatorsOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Emplacements;
using Content.Client.Hands.Systems;
using Content.Shared._RMC14.CombatMode;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Wieldable.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System.Numerics;

#nullable enable
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

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public CombatModeIndicatorsOverlay(
    IInputManager input,
    IEntityManager entMan,
    IEyeManager eye,
    CombatModeSystem combatSys,
    HandsSystem hands)
  {
    Color white = Color.White;
    this.MainColor = ((Color) ref white).WithAlpha(0.3f);
    Color black = Color.Black;
    this.StrokeColor = ((Color) ref black).WithAlpha(0.5f);
    this.Scale = 0.6f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this._inputManager = input;
    this._entMan = entMan;
    this._eye = eye;
    this._combat = combatSys;
    this._hands = hands;
    SpriteSystem entitySystem = this._entMan.EntitySysManager.GetEntitySystem<SpriteSystem>();
    this._gunSight = entitySystem.Frame0((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/Interface/Misc/crosshair_pointers.rsi"), "gun_sight"));
    this._gunBoltSight = entitySystem.Frame0((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/Interface/Misc/crosshair_pointers.rsi"), "gun_bolt_sight"));
    this._meleeSight = entitySystem.Frame0((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/Interface/Misc/crosshair_pointers.rsi"), "melee_sight"));
    this._rmcCombatMode = entMan.System<RMCCombatModeSystem>();
    this._sprite = entMan.System<SpriteSystem>();
    this._rmcWeaponController = entMan.System<RMCWeaponControllerSystem>();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this._combat.IsInCombatMode() && base.BeforeDraw(ref args);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    if (MapId.op_Inequality(this._eye.PixelToMap(mouseScreenPosition).MapId, args.MapId))
      return;
    EntityUid? activeHandEntity = this._hands.GetActiveHandEntity();
    int num1 = this._entMan.HasComponent<GunComponent>(activeHandEntity) ? 1 : 0;
    bool flag = true;
    ChamberMagazineAmmoProviderComponent providerComponent;
    if (this._entMan.TryGetComponent<ChamberMagazineAmmoProviderComponent>(activeHandEntity, ref providerComponent))
      flag = providerComponent.BoltClosed ?? true;
    Vector2 position = mouseScreenPosition.Position;
    float num2 = args.ViewportControl is Control viewportControl ? viewportControl.UIScale : 1f;
    float num3 = (double) num2 > 1.25 ? 1.25f : num2;
    EntityUid? nullable = activeHandEntity;
    EntityUid? weapon;
    if (this._rmcWeaponController.TryGetControllingWeapon(out weapon))
      nullable = weapon;
    Texture sight = num1 != 0 ? (flag ? this._gunSight : this._gunBoltSight) : this._meleeSight;
    if (nullable.HasValue)
    {
      SpriteSpecifier.Rsi crosshair = this._rmcCombatMode.GetCrosshair(Entity<WieldedCrosshairComponent, WieldableComponent>.op_Implicit(nullable.Value));
      if (crosshair != null)
      {
        Texture texture = this._sprite.Frame0((SpriteSpecifier) crosshair);
        Vector2 vector2 = Vector2i.op_Multiply(texture.Size, num3);
        UIBox2 uiBox2 = UIBox2.FromDimensions(position - vector2 * 0.5f, vector2);
        ((OverlayDrawArgs) ref args).ScreenHandle.DrawTextureRect(texture, uiBox2, new Color?());
        return;
      }
    }
    this.DrawSight(sight, ((OverlayDrawArgs) ref args).ScreenHandle, position, num3 * this.Scale);
  }

  private void DrawSight(
    Texture sight,
    DrawingHandleScreen screen,
    Vector2 centerPos,
    float scale)
  {
    Vector2 vector2_1 = Vector2i.op_Multiply(sight.Size, scale);
    Vector2 vector2_2 = vector2_1 + new Vector2(7f, 7f);
    screen.DrawTextureRect(sight, UIBox2.FromDimensions(centerPos - vector2_1 * 0.5f, vector2_1), new Color?(this.StrokeColor));
    screen.DrawTextureRect(sight, UIBox2.FromDimensions(centerPos - vector2_2 * 0.5f, vector2_2), new Color?(this.MainColor));
  }
}
