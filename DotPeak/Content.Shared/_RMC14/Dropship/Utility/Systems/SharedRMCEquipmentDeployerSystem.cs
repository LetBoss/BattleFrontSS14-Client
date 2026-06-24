// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Systems.SharedRMCEquipmentDeployerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.AlertLevel;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Emplacements;
using Content.Shared._RMC14.PowerLoader;
using Content.Shared._RMC14.Sentry;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Systems;

public abstract class SharedRMCEquipmentDeployerSystem : EntitySystem
{
  [Dependency]
  private RMCAlertLevelSystem _alert;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedBuckleSystem _buckle;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SentrySystem _sentry;
  [Dependency]
  private SharedWeaponMountSystem _weaponMount;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCEquipmentDeployerComponent, MapInitEvent>(new EntityEventRefHandler<RMCEquipmentDeployerComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<RMCEquipmentDeployerComponent, InteractHandEvent>(new EntityEventRefHandler<RMCEquipmentDeployerComponent, InteractHandEvent>(this.OnInteract));
    this.SubscribeLocalEvent<RMCEquipmentDeployerComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<RMCEquipmentDeployerComponent, EntGotInsertedIntoContainerMessage>(this.OnInserted));
    this.SubscribeLocalEvent<RMCEquipmentDeployerComponent, EntGotRemovedFromContainerMessage>(new EntityEventRefHandler<RMCEquipmentDeployerComponent, EntGotRemovedFromContainerMessage>(this.OnRemovedFromContainer));
  }

  private void OnMapInit(Entity<RMCEquipmentDeployerComponent> ent, ref MapInitEvent args)
  {
    if (!ent.Comp.DeployPrototype.HasValue || this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.DeploySlotId).ContainedEntities.Count > 0)
      return;
    RMCEquipmentDeployerComponent comp = ent.Comp;
    EntProtoId? deployPrototype = ent.Comp.DeployPrototype;
    NetEntity? nullable = new NetEntity?(this.GetNetEntity(this.SpawnInContainerOrDrop(deployPrototype.HasValue ? (string) deployPrototype.GetValueOrDefault() : (string) null, (EntityUid) ent, ent.Comp.DeploySlotId)));
    comp.DeployEntity = nullable;
    this.DirtyField<RMCEquipmentDeployerComponent>(ent.Owner, ent.Comp, "DeployEntity");
  }

  private void OnInteract(Entity<RMCEquipmentDeployerComponent> ent, ref InteractHandEvent args)
  {
    EntityUid parentUid = this.Transform((EntityUid) ent).ParentUid;
    BaseContainer container;
    if (!ent.Comp.DeployEntity.HasValue || !ent.Comp.IsDeployableByHand || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.DeploySlotId, out container))
      return;
    Vector2 deployOffset1 = Vector2.Zero;
    float rotationOffset = 0.0f;
    DropshipWeaponPointComponent comp;
    if (container.ContainedEntities.Count > 0 && this.TryComp<DropshipWeaponPointComponent>(parentUid, out comp))
      this.TryGetOffset((EntityUid) ent, out deployOffset1, out rotationOffset, comp.Location);
    else if (ent.Comp.DeployEntity.HasValue && container.ContainedEntities.Count == 0)
    {
      EntityUid deployer = (EntityUid) ent;
      EntityUid? nullable = new EntityUid?(args.User);
      Vector2 deployOffset2 = new Vector2();
      EntityUid? user = nullable;
      this.TryDeploy(deployer, false, deployOffset2, user: user);
      return;
    }
    this.TryDeploy((EntityUid) ent, true, deployOffset1, rotationOffset, user: new EntityUid?(args.User));
  }

  private void OnInserted(
    Entity<RMCEquipmentDeployerComponent> ent,
    ref EntGotInsertedIntoContainerMessage args)
  {
    if (this.HasComp<PowerLoaderComponent>(args.Container.Owner))
    {
      ent.Comp.IsDeployable = false;
      this.DirtyField<RMCEquipmentDeployerComponent>(ent.Owner, ent.Comp, "IsDeployable");
    }
    else
    {
      if (this.HasComp<DropshipWeaponPointComponent>(args.Container.Owner))
        ent.Comp.AutoUnDeploy = true;
      ent.Comp.IsDeployable = true;
      this.DirtyField<RMCEquipmentDeployerComponent>(ent.Owner, ent.Comp, "IsDeployable");
    }
  }

  private void OnRemovedFromContainer(
    Entity<RMCEquipmentDeployerComponent> ent,
    ref EntGotRemovedFromContainerMessage args)
  {
    if (ent.Comp.DeployEntity.HasValue)
      this.TryDeploy((EntityUid) ent, false);
    ent.Comp.IsDeployable = false;
    ent.Comp.IsDeployed = false;
    ent.Comp.AutoDeploy = false;
    ent.Comp.AutoUnDeploy = false;
    this.DirtyFields<RMCEquipmentDeployerComponent>(ent.Owner, ent.Comp, (MetaDataComponent) null, "DeployEntity", "IsDeployed", "AutoDeploy", "AutoUnDeploy");
  }

  private void UpdateAppearance(Entity<RMCEquipmentDeployerComponent> ent, bool deployed)
  {
    EntityUid parentUid = this.Transform((EntityUid) ent).ParentUid;
    bool flag1 = this.HasComp<DropshipWeaponPointComponent>(parentUid);
    bool flag2 = this.HasComp<DropshipUtilityPointComponent>(parentUid);
    bool flag3 = this.HasComp<DropshipElectronicSystemPointComponent>(parentUid);
    string str1 = "";
    string str2 = "";
    DropshipAttachedSpriteComponent comp;
    if (!deployed && this.TryComp<DropshipAttachedSpriteComponent>((EntityUid) ent, out comp))
    {
      if (flag1)
      {
        SpriteSpecifier.Rsi weaponSlotSprite = comp.WeaponSlotSprite;
        if (weaponSlotSprite != null)
        {
          str1 = weaponSlotSprite.RsiPath.ToString();
          str2 = weaponSlotSprite.RsiState;
          goto label_17;
        }
      }
      if (flag2 | flag3)
      {
        SpriteSpecifier.Rsi sprite = comp.Sprite;
        if (sprite != null)
        {
          str1 = sprite.RsiPath.ToString();
          str2 = sprite.RsiState;
        }
      }
    }
    else if (deployed)
    {
      if (flag1)
      {
        SpriteSpecifier.Rsi weaponDeployedSprite = ent.Comp.WeaponDeployedSprite;
        if (weaponDeployedSprite != null)
        {
          str1 = weaponDeployedSprite.RsiPath.ToString();
          str2 = weaponDeployedSprite.RsiState;
          goto label_17;
        }
      }
      if (flag3)
      {
        SpriteSpecifier.Rsi electronicDeployedSprite = ent.Comp.ElectronicDeployedSprite;
        if (electronicDeployedSprite != null)
        {
          str1 = electronicDeployedSprite.RsiPath.ToString();
          str2 = electronicDeployedSprite.RsiState;
          goto label_17;
        }
      }
      if (flag2)
      {
        SpriteSpecifier.Rsi utilityDeployedSprite = ent.Comp.UtilityDeployedSprite;
        if (utilityDeployedSprite != null)
        {
          str1 = utilityDeployedSprite.RsiPath.ToString();
          str2 = utilityDeployedSprite.RsiState;
        }
      }
    }
label_17:
    if (flag1)
    {
      this._appearance.SetData(parentUid, (Enum) DropshipWeaponVisuals.Sprite, (object) str1);
      this._appearance.SetData(parentUid, (Enum) DropshipWeaponVisuals.State, (object) str2);
    }
    else if (flag2 | flag3)
    {
      this._appearance.SetData(parentUid, (Enum) DropshipUtilityVisuals.Sprite, (object) str1);
      this._appearance.SetData(parentUid, (Enum) DropshipUtilityVisuals.State, (object) str2);
    }
    else
      this._appearance.SetData((EntityUid) ent, (Enum) EquipmentDeployerVisuals.Sprite, (object) ent.Comp.IsDeployed);
  }

  public bool TryDeploy(
    EntityUid deployer,
    bool deploy,
    Vector2 deployOffset = default (Vector2),
    float rotationOffset = 0.0f,
    RMCEquipmentDeployerComponent? equipmentDeployerComponent = null,
    EntityUid? user = null)
  {
    BaseContainer container;
    if (this.TerminatingOrDeleted(deployer) || !this.Resolve<RMCEquipmentDeployerComponent>(deployer, ref equipmentDeployerComponent, false) || !this._container.TryGetContainer(deployer, equipmentDeployerComponent.DeploySlotId, out container) || !equipmentDeployerComponent.IsDeployable)
      return false;
    if (user.HasValue)
    {
      if (this._entityWhitelist.IsBlacklistPass(equipmentDeployerComponent.Blacklist, user.Value))
        return false;
      RMCAlertLevels? nullable = this._alert.Get();
      RMCAlertLevels alertLevelRequired = equipmentDeployerComponent.AlertLevelRequired;
      if (nullable.GetValueOrDefault() < alertLevelRequired & nullable.HasValue & deploy)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-sentry-not-emergency", (nameof (deployer), (object) deployer)), deployer, new EntityUid?(user.Value));
        return false;
      }
    }
    EntityUid? entity = this.GetEntity(equipmentDeployerComponent.DeployEntity);
    if (entity.HasValue)
    {
      StrapComponent comp1;
      if (this.TryComp<StrapComponent>(entity, out comp1))
      {
        foreach (EntityUid buckledEntity in comp1.BuckledEntities)
        {
          BuckleComponent comp2;
          if (this.TryComp<BuckleComponent>(buckledEntity, out comp2))
            this._buckle.Unbuckle((Entity<BuckleComponent>) (buckledEntity, comp2), new EntityUid?(buckledEntity));
        }
      }
      if (!deploy)
      {
        this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entity.Value, container);
      }
      else
      {
        this._container.EmptyContainer(container, destination: new EntityCoordinates?(this.Transform(deployer).Coordinates.Offset(deployOffset)));
        if (equipmentDeployerComponent.DeployEntity.HasValue)
        {
          TransformComponent transformComponent = this.Transform(entity.Value);
          transformComponent.LocalRotation = Angle.op_Addition(transformComponent.LocalRotation, Angle.FromDegrees((double) rotationOffset));
        }
      }
    }
    equipmentDeployerComponent.IsDeployed = deploy;
    this.DirtyField<RMCEquipmentDeployerComponent>(deployer, equipmentDeployerComponent, "IsDeployed");
    this.UpdateAppearance((Entity<RMCEquipmentDeployerComponent>) (deployer, equipmentDeployerComponent), deploy);
    this._audio.PlayPredicted(deploy ? equipmentDeployerComponent.DeployAudio : equipmentDeployerComponent.UnDeployAudio, this.Transform(deployer).Coordinates, user);
    return true;
  }

  public bool TryGetOffset(
    EntityUid deployer,
    out Vector2 deployOffset,
    out float rotationOffset,
    DropshipWeaponPointLocation? location = null,
    RMCEquipmentDeployerComponent? equipmentDeployerComponent = null)
  {
    deployOffset = Vector2.Zero;
    rotationOffset = 0.0f;
    if (!this.Resolve<RMCEquipmentDeployerComponent>(deployer, ref equipmentDeployerComponent, false) || !location.HasValue)
      return false;
    switch (location.GetValueOrDefault())
    {
      case DropshipWeaponPointLocation.StarboardFore:
        deployOffset = Vector2i.op_Implicit(equipmentDeployerComponent.StarboardForeDeployDirection);
        rotationOffset = equipmentDeployerComponent.ForeDeployRotationDegrees;
        return true;
      case DropshipWeaponPointLocation.PortFore:
        deployOffset = Vector2i.op_Implicit(equipmentDeployerComponent.PortForeDeployDirection);
        rotationOffset = equipmentDeployerComponent.ForeDeployRotationDegrees;
        return true;
      case DropshipWeaponPointLocation.StarboardWing:
        deployOffset = Vector2i.op_Implicit(equipmentDeployerComponent.StarboardWingDeployDirection);
        rotationOffset = equipmentDeployerComponent.StarboardWingDeployRotationDegrees;
        return true;
      case DropshipWeaponPointLocation.PortWing:
        deployOffset = Vector2i.op_Implicit(equipmentDeployerComponent.PortWingDeployDirection);
        rotationOffset = equipmentDeployerComponent.PortWingDeployRotationDegrees;
        return true;
      default:
        return false;
    }
  }

  public void SetAutoDeploy(
    EntityUid deployer,
    bool autoDeploy,
    RMCEquipmentDeployerComponent? equipmentDeployer = null)
  {
    if (!this.Resolve<RMCEquipmentDeployerComponent>(deployer, ref equipmentDeployer, false))
      return;
    equipmentDeployer.AutoDeploy = autoDeploy;
    this.Dirty(deployer, (IComponent) equipmentDeployer);
  }

  public bool TryGetContainer(EntityUid attachPoint, [NotNullWhen(true)] out BaseContainer? container)
  {
    container = (BaseContainer) null;
    DropshipUtilityPointComponent comp1;
    if (this.TryComp<DropshipUtilityPointComponent>(attachPoint, out comp1))
      this._container.TryGetContainer(attachPoint, comp1.UtilitySlotId, out container);
    DropshipWeaponPointComponent comp2;
    if (this.TryComp<DropshipWeaponPointComponent>(attachPoint, out comp2))
      this._container.TryGetContainer(attachPoint, comp2.WeaponContainerSlotId, out container);
    return container != null;
  }

  public bool TryGetDeployedAmmo(EntityUid deployed, [NotNullWhen(true)] out int? ammoCount, [NotNullWhen(true)] out int? ammoCapacity)
  {
    ammoCount = new int?();
    ammoCapacity = new int?();
    return this._weaponMount.TryGetWeaponAmmo(deployed, out ammoCount, out ammoCapacity);
  }

  public bool TryGetDeployedDamage(EntityUid deployed, out FixedPoint2 damage)
  {
    damage = (FixedPoint2) 0;
    DamageableComponent comp;
    if (!this.TryComp<DamageableComponent>(deployed, out comp) || comp.TotalDamage <= 0)
      return false;
    damage = comp.TotalDamage;
    return true;
  }
}
