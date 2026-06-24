// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Hud.XenoHudOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Medical.HUD;
using Content.Client._RMC14.NightVision;
using Content.Shared._RMC14.Mobs;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Synth;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Finesse;
using Content.Shared._RMC14.Xenonids.Maturing;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Stacks;
using Content.Shared._RMC14.Xenonids.Rank;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Ghost;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rounding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Hud;

public sealed class XenoHudOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IGameTiming _timing;
  private readonly ContainerSystem _container;
  private readonly CMHealthIconsSystem _healthIcons;
  private readonly MobStateSystem _mobState;
  private readonly MobThresholdSystem _mobThresholds;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly EntityQuery<DamageableComponent> _damageableQuery;
  private readonly EntityQuery<XenoParasiteComponent> _xenoParasiteQuery;
  private readonly EntityQuery<MobStateComponent> _mobStateQuery;
  private readonly EntityQuery<MobThresholdsComponent> _mobThresholdsQuery;
  private readonly EntityQuery<XenoEnergyComponent> _xenoEnergyQuery;
  private readonly EntityQuery<XenoMaturingComponent> _xenoMaturingQuery;
  private readonly EntityQuery<XenoPlasmaComponent> _xenoPlasmaQuery;
  private readonly EntityQuery<TransformComponent> _xformQuery;
  private readonly EntityQuery<XenoShieldComponent> _xenoShieldQuery;
  private readonly EntityQuery<EntityActiveInvisibleComponent> _invisQuery;
  private readonly EntityQuery<XenoComponent> _xenoQuery;
  private readonly ShaderInstance _shader;
  private readonly ResPath _rsiPath = new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi");
  private readonly ResPath _rsiPathSlow = new ResPath("/Textures/_RMC14/Effects/xeno_stomp.rsi");
  private readonly ResPath _rsiPathFreeze = new ResPath("/Textures/_RMC14/Effects/xeno_freeze.rsi");

  public virtual OverlaySpace Space
  {
    get => !this._overlay.HasOverlay<NightVisionOverlay>() ? (OverlaySpace) 8 : (OverlaySpace) 4;
  }

  public XenoHudOverlay()
  {
    IoCManager.InjectDependencies<XenoHudOverlay>(this);
    this._container = this._entity.System<ContainerSystem>();
    this._healthIcons = this._entity.System<CMHealthIconsSystem>();
    this._mobState = this._entity.System<MobStateSystem>();
    this._mobThresholds = this._entity.System<MobThresholdSystem>();
    this._sprite = this._entity.System<SpriteSystem>();
    this._transform = this._entity.System<TransformSystem>();
    this._damageableQuery = this._entity.GetEntityQuery<DamageableComponent>();
    this._xenoParasiteQuery = this._entity.GetEntityQuery<XenoParasiteComponent>();
    this._mobStateQuery = this._entity.GetEntityQuery<MobStateComponent>();
    this._mobThresholdsQuery = this._entity.GetEntityQuery<MobThresholdsComponent>();
    this._xenoEnergyQuery = this._entity.GetEntityQuery<XenoEnergyComponent>();
    this._xenoMaturingQuery = this._entity.GetEntityQuery<XenoMaturingComponent>();
    this._xenoPlasmaQuery = this._entity.GetEntityQuery<XenoPlasmaComponent>();
    this._xformQuery = this._entity.GetEntityQuery<TransformComponent>();
    this._xenoShieldQuery = this._entity.GetEntityQuery<XenoShieldComponent>();
    this._invisQuery = this._entity.GetEntityQuery<EntityActiveInvisibleComponent>();
    this._xenoQuery = this._entity.GetEntityQuery<XenoComponent>();
    this._shader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("unshaded")).Instance();
    this.ZIndex = new int?(1);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    GhostComponent ghostComponent;
    bool flag1 = this._entity.TryGetComponent<GhostComponent>(((ISharedPlayerManager) this._players).LocalEntity, ref ghostComponent) && ghostComponent.CanGhostInteract;
    bool flag2 = this._entity.HasComponent<XenoComponent>(((ISharedPlayerManager) this._players).LocalEntity);
    bool flag3 = false;
    if (!this._entity.HasComponent<CMGhostXenoHudComponent>(((ISharedPlayerManager) this._players).LocalEntity))
    {
      if (!flag2 && !flag1)
        return;
    }
    else
    {
      if (this._entity.HasComponent<CMGhostXenoHudComponent>(((ISharedPlayerManager) this._players).LocalEntity))
        flag3 = true;
      flag2 = true;
    }
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle = eye != null ? eye.Rotation : new Angle();
    Matrix3x2 scale = Matrix3x2.CreateScale(new Vector2(1f, 1f));
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(angle)));
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    if (flag2)
    {
      this.DrawBars(in args, scale, rotation);
      if (!flag3)
        this.DrawDeadIcon(in args, scale, rotation);
      this.DrawAcidStacks(in args, scale, rotation);
      this.DrawMarkedIcons(in args, scale, rotation);
      this.DrawRank(in args, scale, rotation);
      this.DrawSlow(in args, scale, rotation);
      this.DrawStun(in args, scale, rotation);
    }
    if (flag2 | flag1)
    {
      this.DrawInfectedIcon(in args, scale, rotation);
      this.DrawSynthIcon(in args, scale, rotation);
    }
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
    Matrix3x2 identity = Matrix3x2.Identity;
    ((DrawingHandleBase) worldHandle).SetTransform(ref identity);
  }

  private void DrawBars(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    AllEntityQueryEnumerator<XenoComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<XenoComponent, SpriteComponent, TransformComponent>();
    EntityUid target;
    XenoComponent xenoComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref target, ref xenoComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(target, (MetaDataComponent) null, transformComponent))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((target, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          MobStateComponent component;
          if (!this._mobStateQuery.TryComp(target, ref component) || !this._mobState.IsDead(target, component))
          {
            this.UpdateHealth(Entity<XenoComponent, SpriteComponent, MobStateComponent>.op_Implicit((target, xenoComponent, spriteComponent, component)), worldHandle);
            this.UpdatePlasma(Entity<XenoComponent, SpriteComponent>.op_Implicit((target, xenoComponent, spriteComponent)), worldHandle);
            this.UpdateShields(Entity<XenoComponent, SpriteComponent>.op_Implicit((target, xenoComponent, spriteComponent)), worldHandle);
            this.UpdateEnergy(Entity<XenoComponent, SpriteComponent>.op_Implicit((target, xenoComponent, spriteComponent)), worldHandle);
          }
        }
      }
    }
  }

  private void DrawDeadIcon(
    in OverlayDrawArgs args,
    Matrix3x2 scaleMatrix,
    Matrix3x2 rotationMatrix)
  {
    SpriteSpecifier icon = this._healthIcons.GetDeadIcon().Icon;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    AllEntityQueryEnumerator<MobStateComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<MobStateComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    MobStateComponent mobStateComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref mobStateComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && mobStateComponent.CurrentState == MobState.Dead && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid, (MetaDataComponent) null, transformComponent) && !this._xenoParasiteQuery.HasComp(entityUid) && !this._invisQuery.HasComp(entityUid))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame(icon, this._timing.CurTime, true);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
  }

  private void DrawAcidStacks(
    in OverlayDrawArgs args,
    Matrix3x2 scaleMatrix,
    Matrix3x2 rotationMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    AllEntityQueryEnumerator<VictimXenoAcidStacksComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<VictimXenoAcidStacksComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    VictimXenoAcidStacksComponent acidStacksComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref acidStacksComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid, (MetaDataComponent) null, transformComponent) && !this._invisQuery.HasComp(entityUid))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(this._rsiPath, $"acid_stacks{Math.Clamp(acidStacksComponent.Current, 0, 4)}"), this._timing.CurTime, true);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
  }

  private void DrawRank(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQueryEnumerator<XenoRankComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.EntityQueryEnumerator<XenoRankComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    XenoRankComponent xenoRankComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref xenoRankComponent, ref spriteComponent, ref transformComponent))
    {
      if (xenoRankComponent.Rank >= 2 && xenoRankComponent.Rank <= 6 && !this._xenoMaturingQuery.HasComp(entityUid) && !MapId.op_Inequality(transformComponent.MapID, args.MapId) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid, (MetaDataComponent) null, transformComponent) && !this._invisQuery.HasComp(entityUid))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(this._rsiPath, $"hudxenoupgrade{xenoRankComponent.Rank}"), this._timing.CurTime, true);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
  }

  private void DrawMarkedIcons(
    in OverlayDrawArgs args,
    Matrix3x2 scaleMatrix,
    Matrix3x2 rotationMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    AllEntityQueryEnumerator<XenoMarkedComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<XenoMarkedComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    XenoMarkedComponent xenoMarkedComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref xenoMarkedComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid, (MetaDataComponent) null, transformComponent) && !this._invisQuery.HasComp(entityUid))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(this._rsiPath, "prae_tag"), this._timing.CurTime - xenoMarkedComponent.TimeAdded, false);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
  }

  private void DrawSlow(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    AllEntityQueryEnumerator<XenoSlowVisualsComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<XenoSlowVisualsComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    XenoSlowVisualsComponent visualsComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visualsComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid, (MetaDataComponent) null, transformComponent) && !this._invisQuery.HasComp(entityUid) && !this._xenoQuery.HasComp(entityUid))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(this._rsiPathSlow, "stomp"), this._timing.CurTime, true);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
  }

  private void DrawStun(in OverlayDrawArgs args, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    AllEntityQueryEnumerator<XenoImmobileVisualsComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<XenoImmobileVisualsComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    XenoImmobileVisualsComponent visualsComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visualsComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid, (MetaDataComponent) null, transformComponent) && !this._invisQuery.HasComp(entityUid) && !this._xenoQuery.HasComp(entityUid))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(this._rsiPathFreeze, "freeze"), this._timing.CurTime, true);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
  }

  private void DrawInfectedIcon(
    in OverlayDrawArgs args,
    Matrix3x2 scaleMatrix,
    Matrix3x2 rotationMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    AllEntityQueryEnumerator<VictimInfectedComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<VictimInfectedComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    VictimInfectedComponent infectedComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref infectedComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid, (MetaDataComponent) null, transformComponent) && !this._invisQuery.HasComp(entityUid))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          int index = Math.Min(infectedComponent.CurrentStage, infectedComponent.InfectedIcons.Length - 1);
          Texture frame = this._sprite.GetFrame(infectedComponent.InfectedIcons[index], this._timing.CurTime, true);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
  }

  private void DrawSynthIcon(
    in OverlayDrawArgs args,
    Matrix3x2 scaleMatrix,
    Matrix3x2 rotationMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    AllEntityQueryEnumerator<SynthComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<SynthComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    SynthComponent synthComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref synthComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && !((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid, (MetaDataComponent) null, transformComponent) && !this._invisQuery.HasComp(entityUid))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(this._rsiPath, "fake_tall"), this._timing.CurTime, true);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
  }

  private void UpdateHealth(
    Entity<XenoComponent, SpriteComponent, MobStateComponent?> ent,
    DrawingHandleWorld handle)
  {
    EntityUid entityUid;
    XenoComponent xenoComponent1;
    SpriteComponent spriteComponent1;
    MobStateComponent mobStateComponent;
    ent.Deconstruct(ref entityUid, ref xenoComponent1, ref spriteComponent1, ref mobStateComponent);
    EntityUid target = entityUid;
    XenoComponent xenoComponent2 = xenoComponent1;
    SpriteComponent spriteComponent2 = spriteComponent1;
    MobStateComponent component = mobStateComponent;
    DamageableComponent damageableComponent;
    if (!this._damageableQuery.TryComp(target, ref damageableComponent))
      return;
    FixedPoint2 totalDamage1 = damageableComponent.TotalDamage;
    FixedPoint2? threshold1 = new FixedPoint2?();
    FixedPoint2? threshold2 = new FixedPoint2?();
    MobThresholdsComponent thresholdComponent;
    if (this._mobThresholdsQuery.TryComp(target, ref thresholdComponent))
    {
      this._mobThresholds.TryGetThresholdForState(target, MobState.Critical, out threshold1, thresholdComponent);
      this._mobThresholds.TryGetDeadThreshold(target, out threshold2, thresholdComponent);
    }
    string str1;
    if (!this._mobState.IsCritical(target, component))
    {
      FixedPoint2? nullable;
      if (this._mobState.IsAlive(target) && threshold1.HasValue)
      {
        FixedPoint2 totalDamage2 = damageableComponent.TotalDamage;
        nullable = threshold1;
        if ((nullable.HasValue ? (totalDamage2 > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          goto label_7;
      }
      nullable = threshold1;
      if (!nullable.HasValue)
        threshold1 = threshold2;
      if (!threshold1.HasValue)
        return;
      nullable = threshold1;
      FixedPoint2 fixedPoint2 = totalDamage1;
      int levels = ContentHelpers.RoundToLevels((nullable.HasValue ? new FixedPoint2?(nullable.GetValueOrDefault() - fixedPoint2) : new FixedPoint2?()).Value.Double(), threshold1.Value.Double(), 11);
      string str2;
      if (levels <= 0)
        str2 = "0";
      else
        str2 = $"{levels * 10}";
      str1 = "xenohealth" + str2;
      goto label_22;
    }
label_7:
    if (!threshold1.HasValue)
      return;
    FixedPoint2 valueOrDefault = threshold1.GetValueOrDefault();
    if (!threshold2.HasValue)
      return;
    FixedPoint2 fixedPoint2_1 = threshold2.GetValueOrDefault() - valueOrDefault;
    int levels1 = ContentHelpers.RoundToLevels((totalDamage1 - valueOrDefault).Double(), fixedPoint2_1.Double(), 11);
    string str3;
    if (levels1 <= 0)
      str3 = "1";
    else
      str3 = $"{levels1 * 10}";
    str1 = "xenohealth-" + str3;
label_22:
    SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(this._rsiPath, str1);
    RSI.State state;
    if (!this._resourceCache.GetResource<RSIResource>(rsi.RsiPath, true).RSI.TryGetState(RSI.StateId.op_Implicit(rsi.RsiState), ref state))
      return;
    Texture frame = this._sprite.GetFrame((SpriteSpecifier) rsi, this._timing.CurTime, true);
    Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)));
    float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent2.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height) + xenoComponent2.HudOffset.Y;
    Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent2.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width) + xenoComponent2.HudOffset.X, y);
    ((DrawingHandleBase) handle).DrawTexture(frame, vector2, new Color?());
  }

  private void UpdatePlasma(Entity<XenoComponent, SpriteComponent> ent, DrawingHandleWorld handle)
  {
    EntityUid entityUid1;
    XenoComponent xenoComponent1;
    SpriteComponent spriteComponent1;
    ent.Deconstruct(ref entityUid1, ref xenoComponent1, ref spriteComponent1);
    EntityUid entityUid2 = entityUid1;
    XenoComponent xenoComponent2 = xenoComponent1;
    SpriteComponent spriteComponent2 = spriteComponent1;
    XenoPlasmaComponent xenoPlasmaComponent;
    if (!this._xenoPlasmaQuery.TryComp(entityUid2, ref xenoPlasmaComponent) || xenoPlasmaComponent.MaxPlasma == 0)
      return;
    FixedPoint2 plasma = xenoPlasmaComponent.Plasma;
    int maxPlasma = xenoPlasmaComponent.MaxPlasma;
    int levels = ContentHelpers.RoundToLevels(plasma.Double(), (double) maxPlasma, 11);
    string str;
    if (levels <= 0)
      str = "0";
    else
      str = $"{levels * 10}";
    Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "plasma" + str), this._timing.CurTime, true);
    Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)));
    float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent2.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height) + xenoComponent2.HudOffset.Y;
    Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent2.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width) + xenoComponent2.HudOffset.X, y);
    ((DrawingHandleBase) handle).DrawTexture(frame, vector2, new Color?());
  }

  private void UpdateShields(Entity<XenoComponent, SpriteComponent> ent, DrawingHandleWorld handle)
  {
    EntityUid entityUid;
    XenoComponent xenoComponent1;
    SpriteComponent spriteComponent1;
    ent.Deconstruct(ref entityUid, ref xenoComponent1, ref spriteComponent1);
    EntityUid target = entityUid;
    XenoComponent xenoComponent2 = xenoComponent1;
    SpriteComponent spriteComponent2 = spriteComponent1;
    FixedPoint2 fixedPoint2 = (FixedPoint2) 0;
    XenoShieldComponent xenoShieldComponent;
    if (!this._xenoShieldQuery.TryComp(target, ref xenoShieldComponent))
      return;
    FixedPoint2? threshold1 = new FixedPoint2?();
    FixedPoint2? threshold2 = new FixedPoint2?();
    MobThresholdsComponent thresholdComponent;
    if (this._mobThresholdsQuery.TryComp(target, ref thresholdComponent))
    {
      this._mobThresholds.TryGetThresholdForState(target, MobState.Critical, out threshold1, thresholdComponent);
      this._mobThresholds.TryGetDeadThreshold(target, out threshold2, thresholdComponent);
    }
    if (!threshold1.HasValue)
      threshold1 = threshold2;
    if (!threshold1.HasValue)
      return;
    FixedPoint2 shieldAmount = xenoShieldComponent.ShieldAmount;
    double max = threshold1.Value.Double();
    int levels = ContentHelpers.RoundToLevels(shieldAmount.Double(), max, 11);
    string str;
    if (levels <= 0)
      str = "0";
    else
      str = $"{levels * 10}";
    Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "xenoshield" + str), this._timing.CurTime, true);
    Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)));
    float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent2.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height) + xenoComponent2.HudOffset.Y;
    Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent2.Offset.X) / 2.0 - (double) frame.Width / 32.0 * (double) ((Box2) ref localBounds).Width) + xenoComponent2.HudOffset.X, y);
    ((DrawingHandleBase) handle).DrawTexture(frame, vector2, new Color?());
  }

  private void UpdateEnergy(Entity<XenoComponent, SpriteComponent> ent, DrawingHandleWorld handle)
  {
    XenoEnergyComponent xenoEnergyComponent;
    if (!this._xenoEnergyQuery.TryComp(Entity<XenoComponent, SpriteComponent>.op_Implicit(ent), ref xenoEnergyComponent) || xenoEnergyComponent.Max == 0)
      return;
    this.UpdatePurpleBar(ent, handle, (double) xenoEnergyComponent.Current, (double) xenoEnergyComponent.Max, xenoEnergyComponent.GenerationCap);
  }

  private void UpdatePurpleBar(
    Entity<XenoComponent, SpriteComponent> ent,
    DrawingHandleWorld handle,
    double energy,
    double max,
    int? generationCap)
  {
    EntityUid entityUid;
    XenoComponent xenoComponent1;
    SpriteComponent spriteComponent1;
    ent.Deconstruct(ref entityUid, ref xenoComponent1, ref spriteComponent1);
    XenoComponent xenoComponent2 = xenoComponent1;
    SpriteComponent spriteComponent2 = spriteComponent1;
    int levels1 = ContentHelpers.RoundToLevels(energy, max, 11);
    string str1;
    if (levels1 <= 0)
      str1 = "0";
    else
      str1 = $"{levels1 * 10}";
    Texture frame1 = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "xenoenergy" + str1), this._timing.CurTime, true);
    Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)));
    float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent2.Offset.Y) / 2.0 - (double) frame1.Height / 32.0 * (double) ((Box2) ref localBounds).Height) + xenoComponent2.HudOffset.Y;
    Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent2.Offset.X) / 2.0 - (double) frame1.Width / 32.0 * (double) ((Box2) ref localBounds).Width) + xenoComponent2.HudOffset.X, y);
    ((DrawingHandleBase) handle).DrawTexture(frame1, vector2, new Color?());
    if (!generationCap.HasValue)
      return;
    double num = energy;
    int? nullable1 = generationCap;
    double? nullable2 = nullable1.HasValue ? new double?((double) nullable1.GetValueOrDefault()) : new double?();
    double valueOrDefault = nullable2.GetValueOrDefault();
    if (!(num >= valueOrDefault & nullable2.HasValue))
      return;
    int levels2 = ContentHelpers.RoundToLevels((double) generationCap.Value, max, 11);
    string str2;
    if (levels2 <= 0)
      str2 = "0";
    else
      str2 = $"{levels2 * 10}";
    Texture frame2 = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_hud.rsi"), "cap" + str2), this._timing.CurTime, true);
    ((DrawingHandleBase) handle).DrawTexture(frame2, vector2, new Color?());
  }
}
