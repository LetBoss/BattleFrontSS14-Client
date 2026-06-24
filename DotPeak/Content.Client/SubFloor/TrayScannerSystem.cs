// Decompiled with JetBrains decompiler
// Type: Content.Client.SubFloor.TrayScannerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vents;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.SubFloor;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.SubFloor;

public sealed class TrayScannerSystem : SharedTrayScannerSystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private TrayScanRevealSystem _trayScanReveal;
  private const string TRayAnimationKey = "trays";
  private const double AnimationLength = 0.3;
  public const LookupFlags Flags = (LookupFlags) 13;

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityQuery<TransformComponent> entityQuery1 = this.GetEntityQuery<TransformComponent>();
    TransformComponent transformComponent;
    if (!entityQuery1.TryGetComponent(localEntity, ref transformComponent))
      return;
    Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent, entityQuery1);
    MapId mapId = transformComponent.MapID;
    float num = 0.0f;
    EntityQuery<TrayScannerComponent> entityQuery2 = this.GetEntityQuery<TrayScannerComponent>();
    EntityQuery<RMCTrayCrawlerComponent> entityQuery3 = this.GetEntityQuery<RMCTrayCrawlerComponent>();
    bool flag1 = false;
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (this._inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(localEntity.Value), out containerSlotEnumerator))
    {
      ContainerSlot container;
      while (containerSlotEnumerator.MoveNext(out container))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) ((BaseContainer) container).ContainedEntities)
        {
          TrayScannerComponent scannerComponent;
          if (entityQuery2.TryGetComponent(containedEntity, ref scannerComponent) && scannerComponent.Enabled)
          {
            flag1 = true;
            num = MathF.Max(num, scannerComponent.Range);
          }
        }
      }
    }
    RMCTrayCrawlerComponent crawlerComponent;
    if (entityQuery3.TryGetComponent(localEntity.Value, ref crawlerComponent) && crawlerComponent.Enabled)
    {
      num = MathF.Max(crawlerComponent.Range, num);
      flag1 = true;
    }
    else
    {
      foreach (string enumerateHand in this._hands.EnumerateHands(Entity<HandsComponent>.op_Implicit(localEntity.Value)))
      {
        EntityUid? held;
        TrayScannerComponent scannerComponent;
        if (this._hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(localEntity.Value), enumerateHand, out held) && entityQuery2.TryGetComponent(held, ref scannerComponent) && scannerComponent.Enabled)
        {
          num = MathF.Max(scannerComponent.Range, num);
          flag1 = true;
          break;
        }
      }
    }
    HashSet<Entity<SubFloorHideComponent>> entitySet = new HashSet<Entity<SubFloorHideComponent>>();
    if (flag1)
    {
      this._lookup.GetEntitiesInRange<SubFloorHideComponent>(mapId, worldPosition, num, entitySet, (LookupFlags) 13);
      foreach (Entity<SubFloorHideComponent> entity in entitySet)
      {
        EntityUid entityUid;
        SubFloorHideComponent floorHideComponent;
        entity.Deconstruct(ref entityUid, ref floorHideComponent);
        EntityUid uid = entityUid;
        if (floorHideComponent.IsUnderCover || this._trayScanReveal.IsUnderRevealingEntity(uid))
          this.EnsureComp<TrayRevealedComponent>(uid);
      }
    }
    AllEntityQueryEnumerator<TrayRevealedComponent, SpriteComponent> entityQueryEnumerator = this.AllEntityQuery<TrayRevealedComponent, SpriteComponent>();
    EntityQuery<SubFloorHideComponent> entityQuery4 = this.GetEntityQuery<SubFloorHideComponent>();
    EntityUid uid1;
    TrayRevealedComponent revealedComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref uid1, ref revealedComponent, ref spriteComponent))
    {
      SubFloorHideComponent floorHideComponent;
      if (entityQuery4.TryGetComponent(uid1, ref floorHideComponent) && entitySet.Contains(Entity<SubFloorHideComponent>.op_Implicit((uid1, floorHideComponent))))
      {
        bool flag2;
        if ((!this._appearance.TryGetData<bool>(uid1, (Enum) SubFloorVisuals.ScannerRevealed, ref flag2, (AppearanceComponent) null) || !flag2) && (double) spriteComponent.Color.A > 0.800000011920929)
        {
          SpriteSystem sprite = this._sprite;
          Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((uid1, spriteComponent));
          Color color1 = spriteComponent.Color;
          Color color2 = ((Color) ref color1).WithAlpha(0.0f);
          sprite.SetColor(entity, color2);
        }
        this.SetRevealed(uid1, true);
        if ((double) spriteComponent.Color.A < 0.800000011920929 && !this._animation.HasRunningAnimation(uid1, "trays"))
        {
          AnimationPlayerSystem animation1 = this._animation;
          EntityUid entityUid = uid1;
          Animation animation2 = new Animation();
          animation2.Length = TimeSpan.FromSeconds(0.3);
          List<AnimationTrack> animationTracks = animation2.AnimationTracks;
          AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
          componentProperty.ComponentType = typeof (SpriteComponent);
          componentProperty.Property = "Color";
          List<AnimationTrackProperty.KeyFrame> keyFrames1 = ((AnimationTrackProperty) componentProperty).KeyFrames;
          Color color3 = spriteComponent.Color;
          AnimationTrackProperty.KeyFrame keyFrame1 = new AnimationTrackProperty.KeyFrame((object) ((Color) ref color3).WithAlpha(0.0f), 0.0f, (Func<float, float>) null);
          keyFrames1.Add(keyFrame1);
          List<AnimationTrackProperty.KeyFrame> keyFrames2 = ((AnimationTrackProperty) componentProperty).KeyFrames;
          Color color4 = spriteComponent.Color;
          AnimationTrackProperty.KeyFrame keyFrame2 = new AnimationTrackProperty.KeyFrame((object) ((Color) ref color4).WithAlpha(0.8f), 0.3f, (Func<float, float>) null);
          keyFrames2.Add(keyFrame2);
          animationTracks.Add((AnimationTrack) componentProperty);
          animation1.Play(entityUid, animation2, "trays");
        }
      }
      else if ((double) spriteComponent.Color.A <= 0.0)
      {
        this.SetRevealed(uid1, false);
        this.RemCompDeferred<TrayRevealedComponent>(uid1);
        SpriteSystem sprite = this._sprite;
        Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((uid1, spriteComponent));
        Color color5 = spriteComponent.Color;
        Color color6 = ((Color) ref color5).WithAlpha(1f);
        sprite.SetColor(entity, color6);
      }
      else
      {
        this.SetRevealed(uid1, true);
        if (!this._animation.HasRunningAnimation(uid1, "trays"))
        {
          AnimationPlayerSystem animation3 = this._animation;
          EntityUid entityUid = uid1;
          Animation animation4 = new Animation();
          animation4.Length = TimeSpan.FromSeconds(0.3);
          List<AnimationTrack> animationTracks = animation4.AnimationTracks;
          AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
          componentProperty.ComponentType = typeof (SpriteComponent);
          componentProperty.Property = "Color";
          ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) spriteComponent.Color, 0.0f, (Func<float, float>) null));
          List<AnimationTrackProperty.KeyFrame> keyFrames = ((AnimationTrackProperty) componentProperty).KeyFrames;
          Color color = spriteComponent.Color;
          AnimationTrackProperty.KeyFrame keyFrame = new AnimationTrackProperty.KeyFrame((object) ((Color) ref color).WithAlpha(0.0f), 0.3f, (Func<float, float>) null);
          keyFrames.Add(keyFrame);
          animationTracks.Add((AnimationTrack) componentProperty);
          animation3.Play(entityUid, animation4, "trays");
        }
      }
    }
  }

  private void SetRevealed(EntityUid uid, bool value)
  {
    this._appearance.SetData(uid, (Enum) SubFloorVisuals.ScannerRevealed, (object) value, (AppearanceComponent) null);
  }
}
