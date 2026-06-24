using System;
using Content.Shared.Access.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Turrets;

public abstract class SharedDeployableTurretSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private UseDelaySystem _useDelay;

	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedWiresSystem _wires;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DeployableTurretComponent, ActivateInWorldEvent>((EntityEventRefHandler<DeployableTurretComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableTurretComponent, AttemptChangePanelEvent>((EntityEventRefHandler<DeployableTurretComponent, AttemptChangePanelEvent>)OnAttemptChangeWirePanelWire, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableTurretComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<DeployableTurretComponent, GetVerbsEvent<Verb>>)OnGetVerb, (Type[])null, (Type[])null);
	}

	private void OnGetVerb(Entity<DeployableTurretComponent> ent, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.CanComplexInteract && _accessReader.IsAllowed(args.User, Entity<DeployableTurretComponent>.op_Implicit(ent)))
		{
			EntityUid user = args.User;
			Verb verb = new Verb
			{
				Priority = 1,
				Text = (ent.Comp.Enabled ? base.Loc.GetString("deployable-turret-component-deactivate") : base.Loc.GetString("deployable-turret-component-activate")),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/Spare/poweronoff.svg.192dpi.png")),
				Disabled = !HasAmmo(ent),
				Impact = LogImpact.Low,
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					TryToggleState(ent, user);
				}
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnActivate(Entity<DeployableTurretComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (!((EntitySystem)this).TryComp<UseDelayComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref useDelay) || _useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<DeployableTurretComponent>.op_Implicit(ent), useDelay)), checkDelayed: true))
		{
			if (!_accessReader.IsAllowed(args.User, Entity<DeployableTurretComponent>.op_Implicit(ent)))
			{
				_popup.PopupClient(base.Loc.GetString("deployable-turret-component-access-denied"), Entity<DeployableTurretComponent>.op_Implicit(ent), args.User);
				_audio.PlayPredicted(ent.Comp.AccessDeniedSound, Entity<DeployableTurretComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
			}
			else
			{
				TryToggleState(ent, args.User);
			}
		}
	}

	private void OnAttemptChangeWirePanelWire(Entity<DeployableTurretComponent> ent, ref AttemptChangePanelEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled && !args.Cancelled)
		{
			_popup.PopupClient(base.Loc.GetString("deployable-turret-component-cannot-access-wires"), Entity<DeployableTurretComponent>.op_Implicit(ent), args.User);
			args.Cancelled = true;
		}
	}

	public bool TryToggleState(Entity<DeployableTurretComponent> ent, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return TrySetState(ent, !ent.Comp.Enabled, user);
	}

	public bool TrySetState(Entity<DeployableTurretComponent> ent, bool enabled, EntityUid? user = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (enabled && ent.Comp.CurrentState == DeployableTurretState.Broken)
		{
			if (user.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("deployable-turret-component-is-broken"), Entity<DeployableTurretComponent>.op_Implicit(ent), user.Value);
			}
			return false;
		}
		if (enabled && !HasAmmo(ent))
		{
			if (user.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("deployable-turret-component-no-ammo"), Entity<DeployableTurretComponent>.op_Implicit(ent), user.Value);
			}
			return false;
		}
		SetState(ent, enabled, user);
		return true;
	}

	protected virtual void SetState(Entity<DeployableTurretComponent> ent, bool enabled, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled != enabled)
		{
			WiresPanelComponent wires = default(WiresPanelComponent);
			if (enabled && ((EntitySystem)this).TryComp<WiresPanelComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref wires) && wires.Open)
			{
				_wires.TogglePanel(Entity<DeployableTurretComponent>.op_Implicit(ent), wires, open: false);
				_audio.PlayPredicted(wires.ScrewdriverCloseSound, Entity<DeployableTurretComponent>.op_Implicit(ent), user, (AudioParams?)null);
			}
			float animTimeRemaining = MathF.Max((float)(ent.Comp.AnimationCompletionTime - _timing.CurTime).TotalSeconds, 0f);
			float animTimeNext = (enabled ? ent.Comp.DeploymentLength : ent.Comp.RetractionLength);
			ent.Comp.AnimationCompletionTime = _timing.CurTime + TimeSpan.FromSeconds(animTimeNext + animTimeRemaining);
			DamageableComponent damageable = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref damageable))
			{
				ProtoId<DamageModifierSetPrototype>? damageSetID = (enabled ? ent.Comp.DeployedDamageModifierSetId : ent.Comp.RetractedDamageModifierSetId);
				DamageableSystem damageable2 = _damageable;
				EntityUid uid = Entity<DeployableTurretComponent>.op_Implicit(ent);
				ProtoId<DamageModifierSetPrototype>? val = damageSetID;
				damageable2.SetDamageModifierSetId(uid, val.HasValue ? ProtoId<DamageModifierSetPrototype>.op_Implicit(val.GetValueOrDefault()) : null, damageable);
			}
			FixturesComponent fixtures = default(FixturesComponent);
			if (ent.Comp.DeployedFixture != null && ((EntitySystem)this).TryComp<FixturesComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref fixtures) && fixtures.Fixtures.TryGetValue(ent.Comp.DeployedFixture, out var fixture))
			{
				_physics.SetHard(Entity<DeployableTurretComponent>.op_Implicit(ent), fixture, enabled, (FixturesComponent)null);
			}
			string msg = (enabled ? "deployable-turret-component-activating" : "deployable-turret-component-deactivating");
			_popup.PopupClient(base.Loc.GetString(msg), Entity<DeployableTurretComponent>.op_Implicit(ent), user);
			ent.Comp.Enabled = enabled;
			((EntitySystem)this).DirtyField<DeployableTurretComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ent.Comp, "Enabled", (MetaDataComponent)null);
		}
	}

	public bool HasAmmo(Entity<DeployableTurretComponent> ent)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		GetAmmoCountEvent ammoCountEv = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref ammoCountEv, false);
		return ammoCountEv.Count > 0;
	}
}
