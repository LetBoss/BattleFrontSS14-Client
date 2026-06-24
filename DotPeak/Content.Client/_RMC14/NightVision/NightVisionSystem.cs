// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.NightVision.NightVisionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Burrow;
using Content.Shared.Examine;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.NightVision;

public sealed class NightVisionSystem : SharedNightVisionSystem
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private ILightManager _light;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private SpriteSystem _sprite;
  private EntityQuery<XenoComponent> _xenoQuery;
  private EntityQuery<NightVisionComponent> _nvQuery;
  private static readonly bool DisableMesons = true;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<NightVisionComponent, LocalPlayerAttachedEvent>(new EntityEventRefHandler<NightVisionComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnNightVisionAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<NightVisionComponent, LocalPlayerDetachedEvent>(new EntityEventRefHandler<NightVisionComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnNightVisionDetached)), (Type[]) null, (Type[]) null);
    this._xenoQuery = this._entity.GetEntityQuery<XenoComponent>();
    this._nvQuery = this._entity.GetEntityQuery<NightVisionComponent>();
  }

  private void OnNightVisionAttached(
    Entity<NightVisionComponent> ent,
    ref LocalPlayerAttachedEvent args)
  {
    this.NightVisionChanged(ent);
  }

  private void OnNightVisionDetached(
    Entity<NightVisionComponent> ent,
    ref LocalPlayerDetachedEvent args)
  {
    this.Off();
  }

  protected override void NightVisionChanged(Entity<NightVisionComponent> ent)
  {
    EntityUid entityUid = Entity<NightVisionComponent>.op_Implicit(ent);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    switch (ent.Comp.State)
    {
      case NightVisionState.Off:
        this.Off();
        break;
      case NightVisionState.Half:
        this.Half(ent);
        break;
      case NightVisionState.Full:
        this.Full(ent);
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  protected override void NightVisionRemoved(Entity<NightVisionComponent> ent)
  {
    EntityUid entityUid = Entity<NightVisionComponent>.op_Implicit(ent);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this.Off();
  }

  private void SetMesons(bool on)
  {
    if (NightVisionSystem.DisableMesons || !((ISharedPlayerManager) this._player).LocalEntity.HasValue)
      return;
    this._eye.SetDrawFov(((ISharedPlayerManager) this._player).LocalEntity.Value, !on, (EyeComponent) null);
  }

  private void Off()
  {
    this._overlay.RemoveOverlay<NightVisionOverlay>();
    this._overlay.RemoveOverlay<NightVisionFilterOverlay>();
    this._overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
    this._light.DrawLighting = true;
    this.SetMesons(false);
    this.SetMesonSprites(false);
  }

  private void Half(Entity<NightVisionComponent> ent)
  {
    if (ent.Comp.Overlay)
      this._overlay.AddOverlay((Overlay) new NightVisionOverlay());
    if (ent.Comp.Green)
      this._overlay.AddOverlay((Overlay) new NightVisionFilterOverlay());
    this._overlay.AddOverlay((Overlay) new HalfNightVisionBrightnessOverlay());
    this._light.DrawLighting = true;
    this.SetMesons(ent.Comp.Mesons);
  }

  private void Full(Entity<NightVisionComponent> ent)
  {
    if (ent.Comp.Overlay)
      this._overlay.AddOverlay((Overlay) new NightVisionOverlay());
    if (ent.Comp.Green)
      this._overlay.AddOverlay((Overlay) new NightVisionFilterOverlay());
    this._light.DrawLighting = false;
    this.SetMesons(ent.Comp.Mesons);
  }

  private void SetMesonSprites(bool mesons)
  {
    if (NightVisionSystem.DisableMesons || !((ISharedPlayerManager) this._player).LocalEntity.HasValue)
      return;
    ref EntityQuery<XenoComponent> local = ref this._xenoQuery;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid1 = localEntity.Value;
    bool flag = local.HasComp(entityUid1);
    EntityQueryEnumerator<RMCMesonsNonviewableComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCMesonsNonviewableComponent, SpriteComponent>();
    EntityUid entityUid2;
    RMCMesonsNonviewableComponent nonviewableComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid2, ref nonviewableComponent, ref spriteComponent))
    {
      if (flag && nonviewableComponent.XenoVisible)
      {
        this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent)), true);
      }
      else
      {
        XenoBurrowComponent xenoBurrowComponent;
        if (!this.TryComp<XenoBurrowComponent>(entityUid2, ref xenoBurrowComponent) || !xenoBurrowComponent.Active)
        {
          SpriteSystem sprite = this._sprite;
          Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent));
          int num;
          if (mesons)
          {
            ExamineSystemShared examine = this._examine;
            localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
            EntityUid origin = localEntity.Value;
            EntityUid other = entityUid2;
            num = examine.InRangeUnOccluded(origin, other) ? 1 : 0;
          }
          else
            num = 1;
          sprite.SetVisible(entity, num != 0);
        }
      }
    }
  }

  public virtual void Update(float frameTime)
  {
    NightVisionComponent nightVisionComponent;
    if (!((ISharedPlayerManager) this._player).LocalEntity.HasValue || !this._nvQuery.TryComp(((ISharedPlayerManager) this._player).LocalEntity.Value, ref nightVisionComponent) || nightVisionComponent.State == NightVisionState.Off)
      return;
    this.SetMesonSprites(nightVisionComponent.Mesons);
  }
}
