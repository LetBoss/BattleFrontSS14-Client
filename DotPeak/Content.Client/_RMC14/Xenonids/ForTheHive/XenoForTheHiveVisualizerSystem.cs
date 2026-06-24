// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.ForTheHive.XenoForTheHiveVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.ForTheHive;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.ForTheHive;

public sealed class XenoForTheHiveVisualizerSystem : VisualizerSystem<ForTheHiveComponent>
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ForTheHiveComponent, ForTheHiveActivatedEvent>(new EntityEventRefHandler<ForTheHiveComponent, ForTheHiveActivatedEvent>((object) this, __methodptr(OnForTheHiveAdded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ForTheHiveComponent, ForTheHiveCancelledEvent>(new EntityEventRefHandler<ForTheHiveComponent, ForTheHiveCancelledEvent>((object) this, __methodptr(OnForTheHiveRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnForTheHiveAdded(
    Entity<ForTheHiveComponent> xeno,
    ref ForTheHiveActivatedEvent args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(Entity<ForTheHiveComponent>.op_Implicit(xeno), ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((xeno.Owner, spriteComponent)), (Enum) ForTheHiveVisualLayers.Base, ref num, false) || xeno.Comp.ActiveSprite == null)
      return;
    this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((xeno.Owner, spriteComponent)), num, 0.0f);
    this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((xeno.Owner, spriteComponent)), num, new ResPath(xeno.Comp.ActiveSprite), new RSI.StateId?());
  }

  private void OnForTheHiveRemoved(
    Entity<ForTheHiveComponent> xeno,
    ref ForTheHiveCancelledEvent args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(Entity<ForTheHiveComponent>.op_Implicit(xeno), ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((xeno.Owner, spriteComponent)), (Enum) ForTheHiveVisualLayers.Base, ref num, false) || xeno.Comp.BaseSprite == null)
      return;
    this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((xeno.Owner, spriteComponent)), num, 0.0f);
    this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((xeno.Owner, spriteComponent)), num, new ResPath(xeno.Comp.BaseSprite), new RSI.StateId?());
  }

  protected virtual void OnAppearanceChange(
    EntityUid xeno,
    ForTheHiveComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    float num1;
    int num2;
    if (!((EntitySystem) this).HasComp<ActiveForTheHiveComponent>(xeno) || sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<float>(xeno, (Enum) ForTheHiveVisuals.Time, ref num1, args.Component) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((xeno, sprite)), (Enum) ForTheHiveVisualLayers.Base, ref num2, false) || (double) num1 < 0.0)
      return;
    this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((xeno, sprite)), num2, (float) component.AnimationTimeBase.TotalSeconds * num1);
  }
}
