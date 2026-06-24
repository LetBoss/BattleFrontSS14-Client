// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.Sniper.Focused.RMCFocusingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Rangefinder.Spotting;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged.Sniper.Focused;

public sealed class RMCFocusingSystem : EntitySystem
{
  private const string FocusedKey = "focused";
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCFocusingComponent, ComponentRemove>(new EntityEventRefHandler<RMCFocusingComponent, ComponentRemove>((object) this, __methodptr(OnComponentRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentRemove(Entity<RMCFocusingComponent> ent, ref ComponentRemove args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid owner = ent.Owner;
    EntityUid? nullable = localEntity;
    if ((nullable.HasValue ? (EntityUid.op_Inequality(owner, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this._appearance.SetData(ent.Comp.FocusTarget, (Enum) FocusedVisuals.Focused, (object) false, (AppearanceComponent) null);
  }

  public virtual void Update(float frameTime)
  {
    EntityQueryEnumerator<RMCFocusingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCFocusingComponent>();
    EntityUid entityUid1;
    RMCFocusingComponent focusingComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid1, ref focusingComponent))
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      if (focusingComponent.OldTarget.HasValue)
      {
        this._appearance.SetData(focusingComponent.OldTarget.Value, (Enum) FocusedVisuals.Focused, (object) false, (AppearanceComponent) null);
        focusingComponent.OldTarget = new EntityUid?();
      }
      EntityUid entityUid2 = entityUid1;
      EntityUid? nullable = localEntity;
      SpriteComponent spriteComponent;
      if ((nullable.HasValue ? (EntityUid.op_Inequality(entityUid2, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0 && !this.HasComp<SpotterWhitelistComponent>(entityUid1) || !this.TryComp<SpriteComponent>(focusingComponent.FocusTarget, ref spriteComponent) || !this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((focusingComponent.FocusTarget, spriteComponent)), "focused"))
        break;
      int num;
      this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((focusingComponent.FocusTarget, spriteComponent)), "focused", ref num, false);
      SpriteComponent.Layer layer;
      this._sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((focusingComponent.FocusTarget, spriteComponent)), num, ref layer, false);
      if (layer != null && layer.Visible)
        break;
      this._appearance.SetData(focusingComponent.FocusTarget, (Enum) FocusedVisuals.Focused, (object) true, (AppearanceComponent) null);
      SquadMemberComponent squadMemberComponent;
      if (!this.TryComp<SquadMemberComponent>(entityUid1, ref squadMemberComponent))
        break;
      this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((focusingComponent.FocusTarget, spriteComponent)), "focused", squadMemberComponent.BackgroundColor);
    }
  }
}
