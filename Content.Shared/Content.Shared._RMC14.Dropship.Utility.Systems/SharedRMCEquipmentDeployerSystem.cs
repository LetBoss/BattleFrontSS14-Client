using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
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
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

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
		((EntitySystem)this).SubscribeLocalEvent<RMCEquipmentDeployerComponent, MapInitEvent>((EntityEventRefHandler<RMCEquipmentDeployerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCEquipmentDeployerComponent, InteractHandEvent>((EntityEventRefHandler<RMCEquipmentDeployerComponent, InteractHandEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCEquipmentDeployerComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<RMCEquipmentDeployerComponent, EntGotInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCEquipmentDeployerComponent, EntGotRemovedFromContainerMessage>((EntityEventRefHandler<RMCEquipmentDeployerComponent, EntGotRemovedFromContainerMessage>)OnRemovedFromContainer, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<RMCEquipmentDeployerComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DeployPrototype.HasValue && ((BaseContainer)_container.EnsureContainer<ContainerSlot>(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent), ent.Comp.DeploySlotId, (ContainerManagerComponent)null)).ContainedEntities.Count <= 0)
		{
			RMCEquipmentDeployerComponent comp = ent.Comp;
			EntProtoId? deployPrototype = ent.Comp.DeployPrototype;
			comp.DeployEntity = ((EntitySystem)this).GetNetEntity(((EntitySystem)this).SpawnInContainerOrDrop(deployPrototype.HasValue ? EntProtoId.op_Implicit(deployPrototype.GetValueOrDefault()) : null, Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent), ent.Comp.DeploySlotId, (TransformComponent)null, (ContainerManagerComponent)null, (ComponentRegistry)null), (MetaDataComponent)null);
			((EntitySystem)this).DirtyField<RMCEquipmentDeployerComponent>(ent.Owner, ent.Comp, "DeployEntity", (MetaDataComponent)null);
		}
	}

	private void OnInteract(Entity<RMCEquipmentDeployerComponent> ent, ref InteractHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		EntityUid parent = ((EntitySystem)this).Transform(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent)).ParentUid;
		BaseContainer container = default(BaseContainer);
		if (ent.Comp.DeployEntity.HasValue && ent.Comp.IsDeployableByHand && _container.TryGetContainer(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent), ent.Comp.DeploySlotId, ref container, (ContainerManagerComponent)null))
		{
			Vector2 deployOffset = Vector2.Zero;
			float rotationOffset = 0f;
			DropshipWeaponPointComponent weaponPoint = default(DropshipWeaponPointComponent);
			if (container.ContainedEntities.Count > 0 && ((EntitySystem)this).TryComp<DropshipWeaponPointComponent>(parent, ref weaponPoint))
			{
				TryGetOffset(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent), out deployOffset, out rotationOffset, weaponPoint.Location);
			}
			else if (ent.Comp.DeployEntity.HasValue && container.ContainedEntities.Count == 0)
			{
				EntityUid deployer = Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent);
				EntityUid? user = args.User;
				TryDeploy(deployer, deploy: false, default(Vector2), 0f, null, user);
				return;
			}
			TryDeploy(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent), deploy: true, deployOffset, rotationOffset, null, args.User);
		}
	}

	private void OnInserted(Entity<RMCEquipmentDeployerComponent> ent, ref EntGotInsertedIntoContainerMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<PowerLoaderComponent>(((ContainerModifiedMessage)args).Container.Owner))
		{
			ent.Comp.IsDeployable = false;
			((EntitySystem)this).DirtyField<RMCEquipmentDeployerComponent>(ent.Owner, ent.Comp, "IsDeployable", (MetaDataComponent)null);
			return;
		}
		if (((EntitySystem)this).HasComp<DropshipWeaponPointComponent>(((ContainerModifiedMessage)args).Container.Owner))
		{
			ent.Comp.AutoUnDeploy = true;
		}
		ent.Comp.IsDeployable = true;
		((EntitySystem)this).DirtyField<RMCEquipmentDeployerComponent>(ent.Owner, ent.Comp, "IsDeployable", (MetaDataComponent)null);
	}

	private void OnRemovedFromContainer(Entity<RMCEquipmentDeployerComponent> ent, ref EntGotRemovedFromContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DeployEntity.HasValue)
		{
			TryDeploy(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent), deploy: false);
		}
		ent.Comp.IsDeployable = false;
		ent.Comp.IsDeployed = false;
		ent.Comp.AutoDeploy = false;
		ent.Comp.AutoUnDeploy = false;
		((EntitySystem)this).DirtyFields<RMCEquipmentDeployerComponent>(ent.Owner, ent.Comp, (MetaDataComponent)null, new string[4] { "DeployEntity", "IsDeployed", "AutoDeploy", "AutoUnDeploy" });
	}

	private void UpdateAppearance(Entity<RMCEquipmentDeployerComponent> ent, bool deployed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		EntityUid parent = ((EntitySystem)this).Transform(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent)).ParentUid;
		bool isWeaponPoint = ((EntitySystem)this).HasComp<DropshipWeaponPointComponent>(parent);
		bool isUtilityPoint = ((EntitySystem)this).HasComp<DropshipUtilityPointComponent>(parent);
		bool isElectronicPoint = ((EntitySystem)this).HasComp<DropshipElectronicSystemPointComponent>(parent);
		string rsiPath = "";
		string rsiState = "";
		DropshipAttachedSpriteComponent attached = default(DropshipAttachedSpriteComponent);
		if (!deployed && ((EntitySystem)this).TryComp<DropshipAttachedSpriteComponent>(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent), ref attached))
		{
			if (isWeaponPoint)
			{
				Rsi weaponAttachRsi = attached.WeaponSlotSprite;
				if (weaponAttachRsi != null)
				{
					rsiPath = ((object)weaponAttachRsi.RsiPath/*cast due to constrained. prefix*/).ToString();
					rsiState = weaponAttachRsi.RsiState;
					goto IL_016b;
				}
			}
			if (isUtilityPoint || isElectronicPoint)
			{
				Rsi sprite = attached.Sprite;
				if (sprite != null)
				{
					rsiPath = ((object)sprite.RsiPath/*cast due to constrained. prefix*/).ToString();
					rsiState = sprite.RsiState;
				}
			}
		}
		else if (deployed)
		{
			if (isWeaponPoint)
			{
				Rsi weaponDeployRsi = ent.Comp.WeaponDeployedSprite;
				if (weaponDeployRsi != null)
				{
					rsiPath = ((object)weaponDeployRsi.RsiPath/*cast due to constrained. prefix*/).ToString();
					rsiState = weaponDeployRsi.RsiState;
					goto IL_016b;
				}
			}
			if (isElectronicPoint)
			{
				Rsi electronicDeployRsi = ent.Comp.ElectronicDeployedSprite;
				if (electronicDeployRsi != null)
				{
					rsiPath = ((object)electronicDeployRsi.RsiPath/*cast due to constrained. prefix*/).ToString();
					rsiState = electronicDeployRsi.RsiState;
					goto IL_016b;
				}
			}
			if (isUtilityPoint)
			{
				Rsi utilityDeployRsi = ent.Comp.UtilityDeployedSprite;
				if (utilityDeployRsi != null)
				{
					rsiPath = ((object)utilityDeployRsi.RsiPath/*cast due to constrained. prefix*/).ToString();
					rsiState = utilityDeployRsi.RsiState;
				}
			}
		}
		goto IL_016b;
		IL_016b:
		if (isWeaponPoint)
		{
			_appearance.SetData(parent, (Enum)DropshipWeaponVisuals.Sprite, (object)rsiPath, (AppearanceComponent)null);
			_appearance.SetData(parent, (Enum)DropshipWeaponVisuals.State, (object)rsiState, (AppearanceComponent)null);
		}
		else if (isUtilityPoint || isElectronicPoint)
		{
			_appearance.SetData(parent, (Enum)DropshipUtilityVisuals.Sprite, (object)rsiPath, (AppearanceComponent)null);
			_appearance.SetData(parent, (Enum)DropshipUtilityVisuals.State, (object)rsiState, (AppearanceComponent)null);
		}
		else
		{
			_appearance.SetData(Entity<RMCEquipmentDeployerComponent>.op_Implicit(ent), (Enum)EquipmentDeployerVisuals.Sprite, (object)ent.Comp.IsDeployed, (AppearanceComponent)null);
		}
	}

	public bool TryDeploy(EntityUid deployer, bool deploy, Vector2 deployOffset = default(Vector2), float rotationOffset = 0f, RMCEquipmentDeployerComponent? equipmentDeployerComponent = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).TerminatingOrDeleted(deployer, (MetaDataComponent)null))
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<RMCEquipmentDeployerComponent>(deployer, ref equipmentDeployerComponent, false))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(deployer, equipmentDeployerComponent.DeploySlotId, ref container, (ContainerManagerComponent)null))
		{
			return false;
		}
		if (!equipmentDeployerComponent.IsDeployable)
		{
			return false;
		}
		if (user.HasValue)
		{
			if (_entityWhitelist.IsBlacklistPass(equipmentDeployerComponent.Blacklist, user.Value))
			{
				return false;
			}
			if (_alert.Get() < equipmentDeployerComponent.AlertLevelRequired && deploy)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-sentry-not-emergency", (ValueTuple<string, object>)("deployer", deployer)), deployer, user.Value);
				return false;
			}
		}
		EntityUid? deployingEntity = ((EntitySystem)this).GetEntity(equipmentDeployerComponent.DeployEntity);
		if (deployingEntity.HasValue)
		{
			StrapComponent strap = default(StrapComponent);
			if (((EntitySystem)this).TryComp<StrapComponent>(deployingEntity, ref strap))
			{
				BuckleComponent buckle = default(BuckleComponent);
				foreach (EntityUid strapped in strap.BuckledEntities)
				{
					if (((EntitySystem)this).TryComp<BuckleComponent>(strapped, ref buckle))
					{
						_buckle.Unbuckle(Entity<BuckleComponent>.op_Implicit((strapped, buckle)), strapped);
					}
				}
			}
			if (!deploy)
			{
				_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(deployingEntity.Value), container, (TransformComponent)null, false);
			}
			else
			{
				SharedContainerSystem container2 = _container;
				BaseContainer obj = container;
				EntityCoordinates coordinates = ((EntitySystem)this).Transform(deployer).Coordinates;
				container2.EmptyContainer(obj, false, (EntityCoordinates?)((EntityCoordinates)(ref coordinates)).Offset(deployOffset), true);
				if (equipmentDeployerComponent.DeployEntity.HasValue)
				{
					TransformComponent obj2 = ((EntitySystem)this).Transform(deployingEntity.Value);
					obj2.LocalRotation += Angle.FromDegrees((double)rotationOffset);
				}
			}
		}
		equipmentDeployerComponent.IsDeployed = deploy;
		((EntitySystem)this).DirtyField<RMCEquipmentDeployerComponent>(deployer, equipmentDeployerComponent, "IsDeployed", (MetaDataComponent)null);
		UpdateAppearance(Entity<RMCEquipmentDeployerComponent>.op_Implicit((deployer, equipmentDeployerComponent)), deploy);
		SoundSpecifier audio = (deploy ? equipmentDeployerComponent.DeployAudio : equipmentDeployerComponent.UnDeployAudio);
		_audio.PlayPredicted(audio, ((EntitySystem)this).Transform(deployer).Coordinates, user, (AudioParams?)null);
		return true;
	}

	public bool TryGetOffset(EntityUid deployer, out Vector2 deployOffset, out float rotationOffset, DropshipWeaponPointLocation? location = null, RMCEquipmentDeployerComponent? equipmentDeployerComponent = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		deployOffset = Vector2.Zero;
		rotationOffset = 0f;
		if (!((EntitySystem)this).Resolve<RMCEquipmentDeployerComponent>(deployer, ref equipmentDeployerComponent, false))
		{
			return false;
		}
		switch (location)
		{
		case DropshipWeaponPointLocation.PortFore:
			deployOffset = Vector2i.op_Implicit(equipmentDeployerComponent.PortForeDeployDirection);
			rotationOffset = equipmentDeployerComponent.ForeDeployRotationDegrees;
			return true;
		case DropshipWeaponPointLocation.PortWing:
			deployOffset = Vector2i.op_Implicit(equipmentDeployerComponent.PortWingDeployDirection);
			rotationOffset = equipmentDeployerComponent.PortWingDeployRotationDegrees;
			return true;
		case DropshipWeaponPointLocation.StarboardFore:
			deployOffset = Vector2i.op_Implicit(equipmentDeployerComponent.StarboardForeDeployDirection);
			rotationOffset = equipmentDeployerComponent.ForeDeployRotationDegrees;
			return true;
		case DropshipWeaponPointLocation.StarboardWing:
			deployOffset = Vector2i.op_Implicit(equipmentDeployerComponent.StarboardWingDeployDirection);
			rotationOffset = equipmentDeployerComponent.StarboardWingDeployRotationDegrees;
			return true;
		default:
			return false;
		}
	}

	public void SetAutoDeploy(EntityUid deployer, bool autoDeploy, RMCEquipmentDeployerComponent? equipmentDeployer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RMCEquipmentDeployerComponent>(deployer, ref equipmentDeployer, false))
		{
			equipmentDeployer.AutoDeploy = autoDeploy;
			((EntitySystem)this).Dirty(deployer, (IComponent)(object)equipmentDeployer, (MetaDataComponent)null);
		}
	}

	public bool TryGetContainer(EntityUid attachPoint, [NotNullWhen(true)] out BaseContainer? container)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		container = null;
		DropshipUtilityPointComponent utilityPoint = default(DropshipUtilityPointComponent);
		if (((EntitySystem)this).TryComp<DropshipUtilityPointComponent>(attachPoint, ref utilityPoint))
		{
			_container.TryGetContainer(attachPoint, utilityPoint.UtilitySlotId, ref container, (ContainerManagerComponent)null);
		}
		DropshipWeaponPointComponent weaponPoint = default(DropshipWeaponPointComponent);
		if (((EntitySystem)this).TryComp<DropshipWeaponPointComponent>(attachPoint, ref weaponPoint))
		{
			_container.TryGetContainer(attachPoint, weaponPoint.WeaponContainerSlotId, ref container, (ContainerManagerComponent)null);
		}
		return container != null;
	}

	public bool TryGetDeployedAmmo(EntityUid deployed, [NotNullWhen(true)] out int? ammoCount, [NotNullWhen(true)] out int? ammoCapacity)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		ammoCount = null;
		ammoCapacity = null;
		return _weaponMount.TryGetWeaponAmmo(deployed, out ammoCount, out ammoCapacity);
	}

	public bool TryGetDeployedDamage(EntityUid deployed, out FixedPoint2 damage)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		damage = 0;
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(deployed, ref damageable))
		{
			return false;
		}
		if (damageable.TotalDamage <= 0)
		{
			return false;
		}
		damage = damageable.TotalDamage;
		return true;
	}
}
