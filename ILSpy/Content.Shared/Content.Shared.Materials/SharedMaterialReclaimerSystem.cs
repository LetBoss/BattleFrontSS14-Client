using System;
using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.Mobs.Components;
using Content.Shared.Stacks;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared.Materials;

public abstract class SharedMaterialReclaimerSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	protected SharedAmbientSoundSystem AmbientSound;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	protected SharedContainerSystem Container;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private EmagSystem _emag;

	public const string ActiveReclaimerContainerId = "active-material-reclaimer-container";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MaterialReclaimerComponent, ComponentShutdown>((ComponentEventHandler<MaterialReclaimerComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaterialReclaimerComponent, ExaminedEvent>((ComponentEventHandler<MaterialReclaimerComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaterialReclaimerComponent, GotEmaggedEvent>((ComponentEventRefHandler<MaterialReclaimerComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaterialReclaimerComponent, MapInitEvent>((ComponentEventHandler<MaterialReclaimerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CollideMaterialReclaimerComponent, StartCollideEvent>((ComponentEventRefHandler<CollideMaterialReclaimerComponent, StartCollideEvent>)OnCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveMaterialReclaimerComponent, ComponentStartup>((ComponentEventHandler<ActiveMaterialReclaimerComponent, ComponentStartup>)OnActiveStartup, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, MaterialReclaimerComponent component, MapInitEvent args)
	{
		component.NextSound = Timing.CurTime;
	}

	private void OnShutdown(EntityUid uid, MaterialReclaimerComponent component, ComponentShutdown args)
	{
		_audio.Stop(component.Stream, (AudioComponent)null);
	}

	private void OnExamined(EntityUid uid, MaterialReclaimerComponent component, ExaminedEvent args)
	{
		args.PushMarkup(base.Loc.GetString("recycler-count-items", (ValueTuple<string, object>)("items", component.ItemsProcessed)));
	}

	private void OnEmagged(EntityUid uid, MaterialReclaimerComponent component, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Interaction) && !_emag.CheckFlag(uid, EmagType.Interaction))
		{
			args.Handled = true;
		}
	}

	private void OnCollide(EntityUid uid, CollideMaterialReclaimerComponent component, ref StartCollideEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		MaterialReclaimerComponent reclaimer = default(MaterialReclaimerComponent);
		if (!(args.OurFixtureId != component.FixtureId) && ((EntitySystem)this).TryComp<MaterialReclaimerComponent>(uid, ref reclaimer))
		{
			TryStartProcessItem(uid, args.OtherEntity, reclaimer);
		}
	}

	private void OnActiveStartup(EntityUid uid, ActiveMaterialReclaimerComponent component, ComponentStartup args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.ReclaimingContainer = Container.EnsureContainer<Container>(uid, "active-material-reclaimer-container", (ContainerManagerComponent)null);
	}

	public bool TryStartProcessItem(EntityUid uid, EntityUid item, MaterialReclaimerComponent? component = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialReclaimerComponent>(uid, ref component, true))
		{
			return false;
		}
		if (!CanStart(uid, component))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<MobStateComponent>(item) && !CanGib(uid, item, component))
		{
			return false;
		}
		if (_whitelistSystem.IsWhitelistFail(component.Whitelist, item) || _whitelistSystem.IsBlacklistPass(component.Blacklist, item))
		{
			return false;
		}
		BaseContainer val = default(BaseContainer);
		if (Container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(item, null, null)), ref val) && !Container.TryRemoveFromContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(item), false))
		{
			return false;
		}
		if (user.HasValue)
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(39, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "player", "ToPrettyString(user.Value)");
			handler.AppendLiteral(" destroyed ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
			handler.AppendLiteral(" in the material reclaimer, ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			adminLog.Add(LogType.Action, LogImpact.High, ref handler);
		}
		if (Timing.CurTime > component.NextSound)
		{
			component.Stream = _audio.PlayPredicted(component.Sound, uid, user, (AudioParams?)null)?.Item1;
			component.NextSound = Timing.CurTime + component.SoundCooldown;
		}
		GotReclaimedEvent reclaimedEvent = new GotReclaimedEvent(((EntitySystem)this).Transform(uid).Coordinates);
		((EntitySystem)this).RaiseLocalEvent<GotReclaimedEvent>(item, ref reclaimedEvent, false);
		TimeSpan duration = GetReclaimingDuration(uid, item, component);
		if (duration == TimeSpan.Zero)
		{
			Reclaim(uid, item, 1f, component);
			return true;
		}
		ActiveMaterialReclaimerComponent active = ((EntitySystem)this).EnsureComp<ActiveMaterialReclaimerComponent>(uid);
		active.Duration = duration;
		active.EndTime = Timing.CurTime + duration;
		Container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(item), (BaseContainer)(object)active.ReclaimingContainer, (TransformComponent)null, false);
		return true;
	}

	public virtual bool TryFinishProcessItem(EntityUid uid, MaterialReclaimerComponent? component = null, ActiveMaterialReclaimerComponent? active = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialReclaimerComponent, ActiveMaterialReclaimerComponent>(uid, ref component, ref active, false))
		{
			return false;
		}
		((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)active);
		return true;
	}

	public virtual void Reclaim(EntityUid uid, EntityUid item, float completion = 1f, MaterialReclaimerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MaterialReclaimerComponent>(uid, ref component, true))
		{
			component.ItemsProcessed++;
			if (component.CutOffSound)
			{
				_audio.Stop(component.Stream, (AudioComponent)null);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public bool SetReclaimerEnabled(EntityUid uid, bool enabled, MaterialReclaimerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialReclaimerComponent>(uid, ref component, false))
		{
			return true;
		}
		if (component.Broken && enabled)
		{
			return false;
		}
		component.Enabled = enabled;
		AmbientSound.SetAmbience(uid, enabled && component.Powered);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		return true;
	}

	public bool CanStart(EntityUid uid, MaterialReclaimerComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ActiveMaterialReclaimerComponent>(uid))
		{
			return false;
		}
		if (component.Powered && component.Enabled)
		{
			return !component.Broken;
		}
		return false;
	}

	public bool CanGib(EntityUid uid, EntityUid victim, MaterialReclaimerComponent component)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (component.Powered && component.Enabled && !component.Broken && ((EntitySystem)this).HasComp<BodyComponent>(victim))
		{
			return _emag.CheckFlag(uid, EmagType.Interaction);
		}
		return false;
	}

	public TimeSpan GetReclaimingDuration(EntityUid reclaimer, EntityUid item, MaterialReclaimerComponent? reclaimerComponent = null, PhysicalCompositionComponent? compositionComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialReclaimerComponent>(reclaimer, ref reclaimerComponent, true))
		{
			return TimeSpan.Zero;
		}
		if (!reclaimerComponent.ScaleProcessSpeed || !((EntitySystem)this).Resolve<PhysicalCompositionComponent>(item, ref compositionComponent, false))
		{
			return reclaimerComponent.MinimumProcessDuration;
		}
		TimeSpan duration = TimeSpan.FromSeconds((float)(compositionComponent.MaterialComposition.Values.Sum() * (((EntitySystem)this).CompOrNull<StackComponent>(item)?.Count ?? 1)) / reclaimerComponent.MaterialProcessRate);
		if (duration < reclaimerComponent.MinimumProcessDuration)
		{
			duration = reclaimerComponent.MinimumProcessDuration;
		}
		return duration;
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<ActiveMaterialReclaimerComponent, MaterialReclaimerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveMaterialReclaimerComponent, MaterialReclaimerComponent>();
		EntityUid uid = default(EntityUid);
		ActiveMaterialReclaimerComponent active = default(ActiveMaterialReclaimerComponent);
		MaterialReclaimerComponent reclaimer = default(MaterialReclaimerComponent);
		while (query.MoveNext(ref uid, ref active, ref reclaimer))
		{
			if (!(Timing.CurTime < active.EndTime))
			{
				TryFinishProcessItem(uid, reclaimer, active);
			}
		}
	}
}
