// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Medicine.PubgHealthOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Stylesheets;
using Content.Client.UserInterface.Screens;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._PUBG.Medicine;
using Content.Shared.CCVar;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Medical.Healing;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Medicine;

public sealed class PubgHealthOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IUserInterfaceManager _ui;
  private readonly MobThresholdSystem _threshold;
  private readonly Font _font;
  private int _offsetY;
  private int _offsetX;
  private const float BarWidth = 300f;
  private const float BarHeight = 20f;
  private const float BarMarginBottom = 40f;
  private const float BarMarginRight = 350f;
  private const float EnergyBarWidth = 300f;
  private const float EnergyBarHeight = 12f;
  private const float EnergyBarSpacing = 6f;
  private static readonly Color LabelColor = Color.White;
  private static readonly Color LabelShadowColor;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public PubgHealthOverlay()
  {
    IoCManager.InjectDependencies<PubgHealthOverlay>(this);
    this._threshold = this._entManager.System<MobThresholdSystem>();
    this._cfg.OnValueChanged<int>(CCVars.PubgHealthHudOffsetY, new Action<int>(this.OnOffsetChanged), true);
    this._cfg.OnValueChanged<int>(CCVars.PubgHealthHudOffsetX, new Action<int>(this.OnOffsetXChanged), true);
    this._font = this._resourceCache.NotoStack();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this._ui.ActiveScreen is BattlefrontGameScreen)
      return;
    UIBox2i viewportBounds = args.ViewportBounds;
    float barX = (float) ((double) ((UIBox2i) ref viewportBounds).Width - 300.0 - 350.0) + (float) this._offsetX;
    float barY = (float) ((double) ((UIBox2i) ref viewportBounds).Height - 40.0 - 20.0) - (float) this._offsetY;
    this.DrawBars(((OverlayDrawArgs) ref args).ScreenHandle, barX, barY, 300f);
  }

  public void DrawBars(
    DrawingHandleScreen handle,
    float barX,
    float barY,
    float barWidth,
    float barHeight = 20f,
    float resourceBarHeight = 12f,
    bool stackTempBar = false)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    DamageableComponent damageable;
    MobThresholdsComponent thresholdComponent;
    FixedPoint2? threshold;
    if (!localEntity.HasValue || !this._entManager.HasComponent<PubgMedicalUserComponent>(localEntity) || !this._entManager.TryGetComponent<DamageableComponent>(localEntity, ref damageable) || !this._entManager.TryGetComponent<MobThresholdsComponent>(localEntity, ref thresholdComponent) || !this._threshold.TryGetThresholdForState(localEntity.Value, MobState.Critical, out threshold, thresholdComponent))
      return;
    FixedPoint2 fixedPoint2_1 = threshold.Value;
    FixedPoint2 fixedPoint2_2 = FixedPoint2.Max(fixedPoint2_1 - damageable.TotalDamage, FixedPoint2.Zero);
    float num1 = (float) (fixedPoint2_2 / fixedPoint2_1);
    PubgTemporaryHealthComponent temporaryHealthComponent;
    this._entManager.TryGetComponent<PubgTemporaryHealthComponent>(localEntity, ref temporaryHealthComponent);
    float barY1 = barY - 6f - resourceBarHeight;
    if (stackTempBar)
      barY1 -= 6f + resourceBarHeight;
    CivStaminaComponent staminaComponent;
    if (this._entManager.TryGetComponent<CivStaminaComponent>(localEntity, ref staminaComponent) && (double) staminaComponent.MaxStamina > 0.0)
    {
      float percent = staminaComponent.Stamina / staminaComponent.MaxStamina;
      float? thresholdPercent = new float?();
      CivTeamMemberComponent teamMemberComponent;
      if (this._entManager.TryGetComponent<CivTeamMemberComponent>(localEntity, ref teamMemberComponent))
        thresholdPercent = new float?(Math.Clamp(teamMemberComponent.RunStaminaThresholdRatio, 0.0f, 1f));
      PubgHealthOverlay.DrawResourceBar(handle, barX, barY1, barWidth, resourceBarHeight, percent, Color.FromHex((ReadOnlySpan<char>) "#F6C445", new Color?()), thresholdPercent);
      this.DrawTextWithShadow(handle, new Vector2(barX + 4f, barY1 + 2f), "Стамина", 0.75f, PubgHealthOverlay.LabelColor);
      barY1 -= 6f + resourceBarHeight;
    }
    PubgEnergyComponent pubgEnergyComponent;
    if (this._entManager.TryGetComponent<PubgEnergyComponent>(localEntity, ref pubgEnergyComponent) && (double) pubgEnergyComponent.MaxEnergy > 0.0)
    {
      float percent = pubgEnergyComponent.Energy / pubgEnergyComponent.MaxEnergy;
      PubgHealthOverlay.DrawResourceBar(handle, barX, barY1, barWidth, resourceBarHeight, percent, Color.FromHex((ReadOnlySpan<char>) "#4FC3F7", new Color?()));
      this.DrawTextWithShadow(handle, new Vector2(barX + 4f, barY1 + 2f), "Обезболивающее", 0.75f, PubgHealthOverlay.LabelColor);
    }
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(barX - 2f, barY - 2f, (float) ((double) barX + (double) barWidth + 2.0), (float) ((double) barY + (double) barHeight + 2.0));
    handle.DrawRect(uiBox2_1, Color.FromHex((ReadOnlySpan<char>) "#1a1a1a", new Color?()), true);
    UIBox2 uiBox2_2;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_2).\u002Ector(barX - 1f, barY - 1f, (float) ((double) barX + (double) barWidth + 1.0), (float) ((double) barY + (double) barHeight + 1.0));
    handle.DrawRect(uiBox2_2, Color.FromHex((ReadOnlySpan<char>) "#333333", new Color?()), true);
    UIBox2 uiBox2_3;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_3).\u002Ector(barX, barY, barX + barWidth, barY + barHeight);
    handle.DrawRect(uiBox2_3, Color.FromHex((ReadOnlySpan<char>) "#2d2d2d", new Color?()), true);
    FixedPoint2 pendingHealAmount = this.GetPendingHealAmount(localEntity.Value, damageable, fixedPoint2_1);
    if (pendingHealAmount > FixedPoint2.Zero)
    {
      float num2 = (float) (FixedPoint2.Min(fixedPoint2_2 + pendingHealAmount, fixedPoint2_1) / fixedPoint2_1);
      float num3 = barWidth * num2;
      UIBox2 uiBox2_4;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_4).\u002Ector(barX, barY, barX + num3, barY + barHeight);
      handle.DrawRect(uiBox2_4, Color.FromHex((ReadOnlySpan<char>) "#FFFFFF55", new Color?()), true);
    }
    float num4 = barWidth * num1;
    UIBox2 uiBox2_5;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_5).\u002Ector(barX, barY, barX + num4, barY + barHeight);
    Color color = (double) num1 <= 0.800000011920929 ? ((double) num1 <= 0.5 ? ((double) num1 <= 0.25 ? Color.FromHex((ReadOnlySpan<char>) "#F44336", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#FF9800", new Color?())) : Color.FromHex((ReadOnlySpan<char>) "#8BC34A", new Color?())) : Color.FromHex((ReadOnlySpan<char>) "#4CAF50", new Color?());
    handle.DrawRect(uiBox2_5, color, true);
    if (temporaryHealthComponent != null && temporaryHealthComponent.TemporaryHealth > FixedPoint2.Zero)
    {
      float num5;
      float num6;
      float num7;
      float num8;
      if (stackTempBar)
      {
        num5 = barX;
        num6 = barWidth;
        num7 = resourceBarHeight;
        num8 = barY - 6f - resourceBarHeight;
      }
      else
      {
        num5 = (float) ((double) barX + (double) barWidth + 4.0);
        num6 = barWidth * 0.6f;
        num7 = barHeight;
        num8 = barY;
      }
      float num9 = (float) (temporaryHealthComponent.TemporaryHealth / temporaryHealthComponent.MaxTemporaryHealth);
      UIBox2 uiBox2_6;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_6).\u002Ector(num5 - 2f, num8 - 2f, (float) ((double) num5 + (double) num6 + 2.0), (float) ((double) num8 + (double) num7 + 2.0));
      handle.DrawRect(uiBox2_6, Color.FromHex((ReadOnlySpan<char>) "#1a1a1a", new Color?()), true);
      UIBox2 uiBox2_7;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_7).\u002Ector(num5 - 1f, num8 - 1f, (float) ((double) num5 + (double) num6 + 1.0), (float) ((double) num8 + (double) num7 + 1.0));
      handle.DrawRect(uiBox2_7, Color.FromHex((ReadOnlySpan<char>) "#333333", new Color?()), true);
      UIBox2 uiBox2_8;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_8).\u002Ector(num5, num8, num5 + num6, num8 + num7);
      handle.DrawRect(uiBox2_8, Color.FromHex((ReadOnlySpan<char>) "#2d2d2d", new Color?()), true);
      float num10 = num6 * num9;
      UIBox2 uiBox2_9;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_9).\u002Ector(num5, num8, num5 + num10, num8 + num7);
      handle.DrawRect(uiBox2_9, Color.FromHex((ReadOnlySpan<char>) "#215188", new Color?()), true);
    }
    FixedPoint2 fixedPoint2_3 = FixedPoint2.New(20);
    float num11 = (float) ((fixedPoint2_1 - fixedPoint2_3) / fixedPoint2_1);
    float num12 = barX + barWidth * num11;
    UIBox2 uiBox2_10;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_10).\u002Ector(num12 - 1f, barY, num12 + 1f, barY + barHeight);
    handle.DrawRect(uiBox2_10, Color.FromHex((ReadOnlySpan<char>) "#FFD700AA", new Color?()), true);
    this.DrawTextWithShadow(handle, new Vector2(barX + 4f, barY + 4f), "ХП", 1f, PubgHealthOverlay.LabelColor);
  }

  private void OnOffsetChanged(int offsetY) => this._offsetY = offsetY;

  private void OnOffsetXChanged(int offsetX) => this._offsetX = offsetX;

  private static void DrawResourceBar(
    DrawingHandleScreen handle,
    float barX,
    float barY,
    float barWidth,
    float barHeight,
    float percent,
    Color fillColor,
    float? thresholdPercent = null)
  {
    if ((double) percent < 0.0)
      percent = 0.0f;
    if ((double) percent > 1.0)
      percent = 1f;
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(barX - 2f, barY - 2f, (float) ((double) barX + (double) barWidth + 2.0), (float) ((double) barY + (double) barHeight + 2.0));
    handle.DrawRect(uiBox2_1, Color.FromHex((ReadOnlySpan<char>) "#1a1a1a", new Color?()), true);
    UIBox2 uiBox2_2;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_2).\u002Ector(barX - 1f, barY - 1f, (float) ((double) barX + (double) barWidth + 1.0), (float) ((double) barY + (double) barHeight + 1.0));
    handle.DrawRect(uiBox2_2, Color.FromHex((ReadOnlySpan<char>) "#333333", new Color?()), true);
    UIBox2 uiBox2_3;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_3).\u002Ector(barX, barY, barX + barWidth, barY + barHeight);
    handle.DrawRect(uiBox2_3, Color.FromHex((ReadOnlySpan<char>) "#2d2d2d", new Color?()), true);
    float num1 = barWidth * percent;
    UIBox2 uiBox2_4;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_4).\u002Ector(barX, barY, barX + num1, barY + barHeight);
    handle.DrawRect(uiBox2_4, fillColor, true);
    if (!thresholdPercent.HasValue)
      return;
    float num2 = Math.Clamp(thresholdPercent.Value, 0.0f, 1f);
    float num3 = barX + barWidth * num2;
    UIBox2 uiBox2_5;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_5).\u002Ector(num3 - 1f, barY, num3 + 1f, barY + barHeight);
    handle.DrawRect(uiBox2_5, Color.FromHex((ReadOnlySpan<char>) "#FFF1A8CC", new Color?()), true);
  }

  private void DrawTextWithShadow(
    DrawingHandleScreen handle,
    Vector2 position,
    string text,
    float scale,
    Color textColor)
  {
    handle.DrawString(this._font, position + new Vector2(1f, 1f), (ReadOnlySpan<char>) text, scale, PubgHealthOverlay.LabelShadowColor);
    handle.DrawString(this._font, position, (ReadOnlySpan<char>) text, scale, textColor);
  }

  private FixedPoint2 GetPendingHealAmount(
    EntityUid player,
    DamageableComponent damageable,
    FixedPoint2 maxHp)
  {
    DoAfterComponent doAfterComponent;
    if (!this._entManager.TryGetComponent<DoAfterComponent>(player, ref doAfterComponent))
      return FixedPoint2.Zero;
    foreach (Content.Shared.DoAfter.DoAfter doAfter in doAfterComponent.DoAfters.Values)
    {
      if (!doAfter.Cancelled && !doAfter.Completed)
      {
        EntityUid? entity = this._entManager.GetEntity(doAfter.Args.NetUsed);
        if (entity.HasValue)
        {
          PubgMedicalComponent medicalComponent;
          if (this._entManager.TryGetComponent<PubgMedicalComponent>(entity, ref medicalComponent))
          {
            FixedPoint2 a = medicalComponent.HealAmount;
            if (medicalComponent.MaxHealThreshold > FixedPoint2.Zero)
            {
              FixedPoint2 b = damageable.TotalDamage - medicalComponent.MaxHealThreshold;
              if (b <= FixedPoint2.Zero)
                return FixedPoint2.Zero;
              a = FixedPoint2.Min(a, b);
            }
            return a;
          }
          HealingComponent healingComponent;
          if (this._entManager.TryGetComponent<HealingComponent>(entity, ref healingComponent))
          {
            FixedPoint2 zero = FixedPoint2.Zero;
            foreach ((string _, FixedPoint2 fixedPoint2) in healingComponent.Damage.DamageDict)
            {
              if (fixedPoint2 < 0)
                zero -= fixedPoint2;
            }
            return zero;
          }
        }
      }
    }
    return FixedPoint2.Zero;
  }

  static PubgHealthOverlay()
  {
    Color black = Color.Black;
    PubgHealthOverlay.LabelShadowColor = ((Color) ref black).WithAlpha(0.85f);
  }
}
