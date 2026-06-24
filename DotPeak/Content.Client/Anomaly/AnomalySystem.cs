// Decompiled with JetBrains decompiler
// Type: Content.Client.Anomaly.AnomalySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gravity;
using Content.Shared.Anomaly;
using Content.Shared.Anomaly.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Anomaly;

public sealed class AnomalySystem : SharedAnomalySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private FloatingVisualizerSystem _floating;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnomalyComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<AnomalyComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnomalyComponent, ComponentStartup>(new ComponentEventHandler<AnomalyComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnomalyComponent, AnimationCompletedEvent>(new ComponentEventHandler<AnomalyComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationComplete)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnomalySupercriticalComponent, ComponentShutdown>(new EntityEventRefHandler<AnomalySupercriticalComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(EntityUid uid, AnomalyComponent component, ComponentStartup args)
  {
    this._floating.FloatAnimation(uid, component.FloatingOffset, component.AnimationKey, component.AnimationTime, false);
  }

  private void OnAnimationComplete(
    EntityUid uid,
    AnomalyComponent component,
    AnimationCompletedEvent args)
  {
    if (args.Key != component.AnimationKey)
      return;
    this._floating.FloatAnimation(uid, component.FloatingOffset, component.AnimationKey, component.AnimationTime, false);
  }

  private void OnAppearanceChanged(
    EntityUid uid,
    AnomalyComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    if (sprite == null)
      return;
    bool flag1;
    if (!this.Appearance.TryGetData<bool>(uid, (Enum) AnomalyVisuals.IsPulsing, ref flag1, args.Component))
      flag1 = false;
    bool flag2;
    if (this.Appearance.TryGetData<bool>(uid, (Enum) AnomalyVisuals.Supercritical, ref flag2, args.Component) & flag2)
      flag1 = flag2;
    if (this.HasComp<AnomalySupercriticalComponent>(uid))
      flag1 = true;
    int num1;
    int num2;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) AnomalyVisualLayers.Base, ref num1, false) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) AnomalyVisualLayers.Animated, ref num2, false))
      return;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, !flag1);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, flag1);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<AnomalySupercriticalComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AnomalySupercriticalComponent, SpriteComponent>();
    EntityUid entityUid;
    AnomalySupercriticalComponent supercriticalComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref supercriticalComponent, ref spriteComponent))
    {
      float num1 = 1f - (float) ((supercriticalComponent.EndTime - this._timing.CurTime) / supercriticalComponent.SupercriticalDuration);
      float num2 = (float) ((double) num1 * ((double) supercriticalComponent.MaxScaleAmount - 1.0) + 1.0);
      this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), new Vector2(num2, num2));
      byte num3 = (byte) (65.0 * (1.0 - (double) num1) + 190.0);
      int num4 = (int) num3;
      Color color1 = spriteComponent.Color;
      int abyte = (int) ((Color) ref color1).AByte;
      if (num4 < abyte)
      {
        SpriteSystem sprite = this._sprite;
        Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent));
        color1 = spriteComponent.Color;
        Color color2 = ((Color) ref color1).WithAlpha(num3);
        sprite.SetColor(entity, color2);
      }
    }
  }

  private void OnShutdown(Entity<AnomalySupercriticalComponent> ent, ref ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<AnomalySupercriticalComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), Vector2.One);
    SpriteSystem sprite = this._sprite;
    Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent));
    Color color1 = spriteComponent.Color;
    Color color2 = ((Color) ref color1).WithAlpha(1f);
    sprite.SetColor(entity, color2);
  }
}
