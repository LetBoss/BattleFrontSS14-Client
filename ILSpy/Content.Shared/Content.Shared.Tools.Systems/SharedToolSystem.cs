using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Tools;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.Tools.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Tools.Systems;

public abstract class SharedToolSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class ToolDoAfterEvent : DoAfterEvent, ISerializationGenerated<ToolDoAfterEvent>, ISerializationGenerated
	{
		[DataField(null, false, 1, false, false, null)]
		public float Fuel;

		[DataField("target", false, 1, false, false, null)]
		public NetEntity? OriginalTarget;

		[DataField("wrappedEvent", false, 1, false, false, null)]
		public DoAfterEvent WrappedEvent;

		[DataField(null, false, 1, false, false, null)]
		public bool Predicted = true;

		private ToolDoAfterEvent()
		{
		}

		public ToolDoAfterEvent(float fuel, DoAfterEvent wrappedEvent, NetEntity? originalTarget)
		{
			Fuel = fuel;
			WrappedEvent = wrappedEvent;
			OriginalTarget = originalTarget;
		}

		public override DoAfterEvent Clone()
		{
			DoAfterEvent evClone = WrappedEvent.Clone();
			if (evClone == WrappedEvent)
			{
				return this;
			}
			return new ToolDoAfterEvent(Fuel, evClone, OriginalTarget);
		}

		public override bool IsDuplicate(DoAfterEvent other)
		{
			if (other is ToolDoAfterEvent toolDoAfter)
			{
				return WrappedEvent.IsDuplicate(toolDoAfter.WrappedEvent);
			}
			return false;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref ToolDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			DoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (ToolDoAfterEvent)definitionCast;
			if (!serialization.TryCustomCopy<ToolDoAfterEvent>(this, ref target, hookCtx, false, context))
			{
				float FuelTemp = 0f;
				if (!serialization.TryCustomCopy<float>(Fuel, ref FuelTemp, hookCtx, false, context))
				{
					FuelTemp = Fuel;
				}
				target.Fuel = FuelTemp;
				NetEntity? OriginalTargetTemp = null;
				if (!serialization.TryCustomCopy<NetEntity?>(OriginalTarget, ref OriginalTargetTemp, hookCtx, false, context))
				{
					OriginalTargetTemp = serialization.CreateCopy<NetEntity?>(OriginalTarget, hookCtx, context, false);
				}
				target.OriginalTarget = OriginalTargetTemp;
				DoAfterEvent WrappedEventTemp = null;
				if (WrappedEvent == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<DoAfterEvent>(WrappedEvent, ref WrappedEventTemp, hookCtx, true, context))
				{
					WrappedEventTemp = serialization.CreateCopy<DoAfterEvent>(WrappedEvent, hookCtx, context, false);
				}
				target.WrappedEvent = WrappedEventTemp;
				bool PredictedTemp = false;
				if (!serialization.TryCustomCopy<bool>(Predicted, ref PredictedTemp, hookCtx, false, context))
				{
					PredictedTemp = Predicted;
				}
				target.Predicted = PredictedTemp;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref ToolDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ToolDoAfterEvent cast = (ToolDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ToolDoAfterEvent cast = (ToolDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override ToolDoAfterEvent Instantiate()
		{
			return new ToolDoAfterEvent();
		}
	}

	[Serializable]
	[NetSerializable]
	protected sealed class LatticeCuttingCompleteEvent : DoAfterEvent, ISerializationGenerated<LatticeCuttingCompleteEvent>, ISerializationGenerated
	{
		[DataField(null, false, 1, true, false, null)]
		public NetCoordinates Coordinates;

		private LatticeCuttingCompleteEvent()
		{
		}

		public LatticeCuttingCompleteEvent(NetCoordinates coordinates)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			Coordinates = coordinates;
		}

		public override DoAfterEvent Clone()
		{
			return this;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref LatticeCuttingCompleteEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			DoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (LatticeCuttingCompleteEvent)definitionCast;
			if (!serialization.TryCustomCopy<LatticeCuttingCompleteEvent>(this, ref target, hookCtx, false, context))
			{
				NetCoordinates CoordinatesTemp = default(NetCoordinates);
				if (!serialization.TryCustomCopy<NetCoordinates>(Coordinates, ref CoordinatesTemp, hookCtx, false, context))
				{
					CoordinatesTemp = serialization.CreateCopy<NetCoordinates>(Coordinates, hookCtx, context, false);
				}
				target.Coordinates = CoordinatesTemp;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref LatticeCuttingCompleteEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			LatticeCuttingCompleteEvent cast = (LatticeCuttingCompleteEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			LatticeCuttingCompleteEvent cast = (LatticeCuttingCompleteEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override LatticeCuttingCompleteEvent Instantiate()
		{
			return new LatticeCuttingCompleteEvent();
		}
	}

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IPrototypeManager _protoMan;

	[Dependency]
	protected ISharedAdminLogManager AdminLogger;

	[Dependency]
	private ITileDefinitionManager _tileDefManager;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	protected SharedInteractionSystem InteractionSystem;

	[Dependency]
	protected ItemToggleSystem ItemToggle;

	[Dependency]
	private SharedMapSystem _maps;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	protected SharedSolutionContainerSystem SolutionContainerSystem;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private TileSystem _tiles;

	[Dependency]
	private TurfSystem _turfs;

	public const string CutQuality = "Cutting";

	public const string PulseQuality = "Pulsing";

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	public override void Initialize()
	{
		InitializeMultipleTool();
		InitializeTile();
		InitializeWelder();
		((EntitySystem)this).SubscribeLocalEvent<ToolComponent, ToolDoAfterEvent>((ComponentEventHandler<ToolComponent, ToolDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToolComponent, ExaminedEvent>((EntityEventRefHandler<ToolComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnDoAfter(EntityUid uid, ToolComponent tool, ToolDoAfterEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			PlayToolSound(uid, tool, args.User, args.Predicted);
		}
		DoAfterEvent ev = args.WrappedEvent;
		ev.DoAfter = args.DoAfter;
		if (args.OriginalTarget.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent(((EntitySystem)this).GetEntity(args.OriginalTarget.Value), (object)ev, false);
		}
		else
		{
			((EntitySystem)this).RaiseLocalEvent((object)ev);
		}
	}

	private void OnExamine(Entity<ToolComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Qualities.Count == 0)
		{
			return;
		}
		FormattedMessage message = new FormattedMessage();
		List<string> toolQualities = new List<string>();
		ToolQualityPrototype protoToolQuality = default(ToolQualityPrototype);
		foreach (string toolQuality in ent.Comp.Qualities)
		{
			if (_protoMan.TryIndex<ToolQualityPrototype>(toolQuality ?? string.Empty, ref protoToolQuality))
			{
				toolQualities.Add(base.Loc.GetString(protoToolQuality.Name));
			}
		}
		string qualitiesString = string.Join(", ", toolQualities);
		message.AddMarkupPermissive(base.Loc.GetString("tool-component-qualities", (ValueTuple<string, object>)("qualities", qualitiesString)));
		args.PushMessage(message);
	}

	public void PlayToolSound(EntityUid uid, ToolComponent tool, EntityUid? user, bool predicted = true)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (tool.UseSound != null)
		{
			if (predicted)
			{
				_audioSystem.PlayPredicted(tool.UseSound, uid, user, (AudioParams?)null);
			}
			else if (_net.IsServer)
			{
				_audioSystem.PlayPvs(tool.UseSound, uid, (AudioParams?)null);
			}
		}
	}

	public bool UseTool(EntityUid tool, EntityUid user, EntityUid? target, float doAfterDelay, [ForbidLiteral] IEnumerable<string> toolQualitiesNeeded, DoAfterEvent doAfterEv, float fuel = 0f, ToolComponent? toolComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DoAfterId? id;
		return UseTool(tool, user, target, TimeSpan.FromSeconds(doAfterDelay), toolQualitiesNeeded, doAfterEv, out id, fuel, toolComponent);
	}

	public bool UseTool(EntityUid tool, EntityUid user, EntityUid? target, TimeSpan delay, [ForbidLiteral] IEnumerable<string> toolQualitiesNeeded, DoAfterEvent doAfterEv, out DoAfterId? id, float fuel = 0f, ToolComponent? toolComponent = null, DuplicateConditions duplicateCondition = DuplicateConditions.None, bool predicted = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		id = null;
		if (!((EntitySystem)this).Resolve<ToolComponent>(tool, ref toolComponent, false))
		{
			return false;
		}
		if (!CanStartToolUse(tool, user, target, fuel, toolQualitiesNeeded, toolComponent))
		{
			return false;
		}
		RMCToolUseEvent ev = new RMCToolUseEvent(user, delay);
		((EntitySystem)this).RaiseLocalEvent<RMCToolUseEvent>(tool, ref ev, false);
		if (ev.Handled)
		{
			delay = ev.Delay;
		}
		ToolDoAfterEvent toolEvent = new ToolDoAfterEvent(fuel, doAfterEv, ((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null))
		{
			Predicted = predicted
		};
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay / toolComponent.SpeedModifier, toolEvent, tool, target, tool)
		{
			BreakOnDamage = true,
			BreakOnMove = true,
			BreakOnWeightlessMove = false,
			NeedHand = (tool != user),
			AttemptFrequency = ((fuel > 0f) ? AttemptFrequency.EveryTick : AttemptFrequency.Never),
			DuplicateCondition = duplicateCondition
		};
		_doAfterSystem.TryStartDoAfter(doAfterArgs, out id);
		return true;
	}

	public bool UseTool(EntityUid tool, EntityUid user, EntityUid? target, float doAfterDelay, [ForbidLiteral] string toolQualityNeeded, DoAfterEvent doAfterEv, float fuel = 0f, ToolComponent? toolComponent = null, DuplicateConditions duplicateCondition = DuplicateConditions.None)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DoAfterId? id;
		return UseTool(tool, user, target, TimeSpan.FromSeconds(doAfterDelay), new string[1] { toolQualityNeeded }, doAfterEv, out id, fuel, toolComponent);
	}

	public bool HasQuality(EntityUid uid, [ForbidLiteral] string quality, ToolComponent? tool = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ToolComponent>(uid, ref tool, false))
		{
			return tool.Qualities.Contains(quality);
		}
		return false;
	}

	public bool HasAllQualities(EntityUid uid, [ForbidLiteral] IEnumerable<string> qualities, ToolComponent? tool = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ToolComponent>(uid, ref tool, false))
		{
			return tool.Qualities.ContainsAll(qualities);
		}
		return false;
	}

	private bool CanStartToolUse(EntityUid tool, EntityUid user, EntityUid? target, float fuel, IEnumerable<string> toolQualitiesNeeded, ToolComponent? toolComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ToolComponent>(tool, ref toolComponent, true))
		{
			return false;
		}
		if (!toolComponent.Qualities.ContainsAll(toolQualitiesNeeded))
		{
			return false;
		}
		ToolUserAttemptUseEvent ev = new ToolUserAttemptUseEvent(target);
		((EntitySystem)this).RaiseLocalEvent<ToolUserAttemptUseEvent>(user, ref ev, false);
		if (ev.Cancelled)
		{
			return false;
		}
		ToolUseAttemptEvent beforeAttempt = new ToolUseAttemptEvent(user, fuel);
		((EntitySystem)this).RaiseLocalEvent<ToolUseAttemptEvent>(tool, beforeAttempt, false);
		if (((CancellableEntityEventArgs)beforeAttempt).Cancelled)
		{
			return false;
		}
		if (target.HasValue)
		{
			EntityUid? val = target;
			if (!val.HasValue || val.GetValueOrDefault() != tool)
			{
				((EntitySystem)this).RaiseLocalEvent<ToolUseAttemptEvent>(target.Value, beforeAttempt, false);
			}
		}
		return !((CancellableEntityEventArgs)beforeAttempt).Cancelled;
	}

	public void InitializeMultipleTool()
	{
		((EntitySystem)this).SubscribeLocalEvent<MultipleToolComponent, ComponentStartup>((ComponentEventHandler<MultipleToolComponent, ComponentStartup>)OnMultipleToolStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultipleToolComponent, ActivateInWorldEvent>((ComponentEventHandler<MultipleToolComponent, ActivateInWorldEvent>)OnMultipleToolActivated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultipleToolComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<MultipleToolComponent, AfterAutoHandleStateEvent>)OnMultipleToolHandleState, (Type[])null, (Type[])null);
	}

	private void OnMultipleToolHandleState(EntityUid uid, MultipleToolComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetMultipleTool(uid, component);
	}

	private void OnMultipleToolStartup(EntityUid uid, MultipleToolComponent multiple, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ToolComponent tool = default(ToolComponent);
		if (((EntitySystem)this).TryComp<ToolComponent>(uid, ref tool))
		{
			SetMultipleTool(uid, multiple, tool);
		}
	}

	private void OnMultipleToolActivated(EntityUid uid, MultipleToolComponent multiple, ActivateInWorldEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			((HandledEntityEventArgs)args).Handled = CycleMultipleTool(uid, multiple, args.User);
		}
	}

	public bool CycleMultipleTool(EntityUid uid, MultipleToolComponent? multiple = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MultipleToolComponent>(uid, ref multiple, true))
		{
			return false;
		}
		if (multiple.Entries.Length == 0)
		{
			return false;
		}
		multiple.CurrentEntry = (uint)((multiple.CurrentEntry + 1) % multiple.Entries.Length);
		SetMultipleTool(uid, multiple, null, playSound: true, user);
		return true;
	}

	public virtual void SetMultipleTool(EntityUid uid, MultipleToolComponent? multiple = null, ToolComponent? tool = null, bool playSound = false, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MultipleToolComponent, ToolComponent>(uid, ref multiple, ref tool, true))
		{
			return;
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)multiple, (MetaDataComponent)null);
		if (multiple.Entries.Length <= multiple.CurrentEntry)
		{
			multiple.CurrentQualityName = base.Loc.GetString("multiple-tool-component-no-behavior");
			return;
		}
		MultipleToolComponent.ToolEntry current = multiple.Entries[multiple.CurrentEntry];
		tool.UseSound = current.UseSound;
		tool.Qualities = current.Behavior;
		PryingComponent pryComp = default(PryingComponent);
		if (((EntitySystem)this).TryComp<PryingComponent>(uid, ref pryComp))
		{
			pryComp.Enabled = current.Behavior.Contains("Prying");
		}
		if (playSound && current.ChangeSound != null)
		{
			_audioSystem.PlayPredicted(current.ChangeSound, uid, user, (AudioParams?)null);
		}
		ToolQualityPrototype quality = default(ToolQualityPrototype);
		if (_protoMan.TryIndex<ToolQualityPrototype>(((IEnumerable<string>)current.Behavior).First(), ref quality))
		{
			multiple.CurrentQualityName = base.Loc.GetString(quality.Name);
		}
	}

	public void InitializeTile()
	{
		((EntitySystem)this).SubscribeLocalEvent<ToolTileCompatibleComponent, AfterInteractEvent>((EntityEventRefHandler<ToolTileCompatibleComponent, AfterInteractEvent>)OnToolTileAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToolTileCompatibleComponent, TileToolDoAfterEvent>((EntityEventRefHandler<ToolTileCompatibleComponent, TileToolDoAfterEvent>)OnToolTileComplete, (Type[])null, (Type[])null);
	}

	private void OnToolTileAfterInteract(Entity<ToolTileCompatibleComponent> ent, ref AfterInteractEvent args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && (!args.Target.HasValue || ((EntitySystem)this).HasComp<PuddleComponent>(args.Target)))
		{
			((HandledEntityEventArgs)args).Handled = UseToolOnTile(Entity<ToolTileCompatibleComponent, ToolComponent>.op_Implicit((ValueTuple<EntityUid, ToolTileCompatibleComponent, ToolComponent>)(Entity<ToolTileCompatibleComponent>.op_Implicit(ent), Entity<ToolTileCompatibleComponent>.op_Implicit(ent), null)), args.User, args.ClickLocation);
		}
	}

	private void OnToolTileComplete(Entity<ToolTileCompatibleComponent> ent, ref TileToolDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		ToolTileCompatibleComponent comp = ent.Comp;
		ToolComponent tool = default(ToolComponent);
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled || !((EntitySystem)this).TryComp<ToolComponent>(Entity<ToolTileCompatibleComponent>.op_Implicit(ent), ref tool))
		{
			return;
		}
		EntityUid gridUid = ((EntitySystem)this).GetEntity(args.Grid);
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref grid))
		{
			((EntitySystem)this).Log.Error("Attempted use tool on a non-existent grid?");
			return;
		}
		TileRef tileRef = _maps.GetTileRef(gridUid, grid, args.GridTile);
		EntityCoordinates coords = _maps.ToCoordinates(tileRef, grid);
		if ((!comp.RequiresUnobstructed || !_turfs.IsTileBlocked(gridUid, tileRef.GridIndices, CollisionGroup.MobMask)) && TryDeconstructWithToolQualities(tileRef, tool.Qualities))
		{
			ISharedAdminLogManager adminLogger = AdminLogger;
			LogStringHandler handler = new LogStringHandler(27, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
			handler.AppendLiteral(" used ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ToolTileCompatibleComponent>.op_Implicit(ent), (MetaDataComponent)null), "ToPrettyString(ent)");
			handler.AppendLiteral(" to edit the tile at ");
			handler.AppendFormatted<EntityCoordinates>(coords, "coords");
			adminLogger.Add(LogType.LatticeCut, LogImpact.Medium, ref handler);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool UseToolOnTile(Entity<ToolTileCompatibleComponent?, ToolComponent?> ent, EntityUid user, EntityCoordinates clickLocation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ToolTileCompatibleComponent, ToolComponent>(Entity<ToolTileCompatibleComponent, ToolComponent>.op_Implicit(ent), ref ent.Comp1, ref ent.Comp2, false))
		{
			return false;
		}
		ToolTileCompatibleComponent comp = ent.Comp1;
		ToolComponent tool = ent.Comp2;
		EntityUid gridUid = default(EntityUid);
		MapGridComponent mapGrid = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(_transformSystem.ToMapCoordinates(clickLocation, true), ref gridUid, ref mapGrid))
		{
			return false;
		}
		TileRef tileRef = _maps.GetTileRef(gridUid, mapGrid, clickLocation);
		ContentTileDefinition tileDef = (ContentTileDefinition)(object)_tileDefManager[tileRef.Tile.TypeId];
		if (!tool.Qualities.ContainsAny((IEnumerable<string>)tileDef.DeconstructTools))
		{
			return false;
		}
		if (string.IsNullOrWhiteSpace(tileDef.BaseTurf))
		{
			return false;
		}
		if (comp.RequiresUnobstructed && _turfs.IsTileBlocked(gridUid, tileRef.GridIndices, CollisionGroup.MobMask))
		{
			return false;
		}
		EntityCoordinates coordinates = _maps.GridTileToLocal(gridUid, mapGrid, tileRef.GridIndices);
		if (!InteractionSystem.InRangeUnobstructed(user, coordinates))
		{
			return false;
		}
		TileToolDoAfterEvent args = new TileToolDoAfterEvent(((EntitySystem)this).GetNetEntity(gridUid, (MetaDataComponent)null), tileRef.GridIndices);
		UseTool(Entity<ToolTileCompatibleComponent, ToolComponent>.op_Implicit(ent), user, Entity<ToolTileCompatibleComponent, ToolComponent>.op_Implicit(ent), comp.Delay, (IEnumerable<string>)tool.Qualities, args, out var _, 0f, tool);
		return true;
	}

	public bool TryDeconstructWithToolQualities(TileRef tileRef, PrototypeFlags<ToolQualityPrototype> withToolQualities)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		ContentTileDefinition tileDef = (ContentTileDefinition)(object)_tileDefManager[tileRef.Tile.TypeId];
		if (withToolQualities.ContainsAny((IEnumerable<string>)tileDef.DeconstructTools))
		{
			if (!_net.IsClient)
			{
				return _tiles.DeconstructTile(tileRef);
			}
			return true;
		}
		return false;
	}

	public void InitializeWelder()
	{
		((EntitySystem)this).SubscribeLocalEvent<WelderComponent, ExaminedEvent>((EntityEventRefHandler<WelderComponent, ExaminedEvent>)OnWelderExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WelderComponent, ToolUseAttemptEvent>((ComponentEventHandler<WelderComponent, ToolUseAttemptEvent>)delegate(EntityUid uid, WelderComponent comp, ToolUseAttemptEvent ev)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			CanCancelWelderUse(Entity<WelderComponent>.op_Implicit((uid, comp)), ev.User, ev.Fuel, (CancellableEntityEventArgs)(object)ev);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WelderComponent, DoAfterAttemptEvent<ToolDoAfterEvent>>((ComponentEventHandler<WelderComponent, DoAfterAttemptEvent<ToolDoAfterEvent>>)delegate(EntityUid uid, WelderComponent comp, DoAfterAttemptEvent<ToolDoAfterEvent> ev)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			CanCancelWelderUse(Entity<WelderComponent>.op_Implicit((uid, comp)), ev.Event.User, ev.Event.Fuel, (CancellableEntityEventArgs)(object)ev);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WelderComponent, ToolDoAfterEvent>((EntityEventRefHandler<WelderComponent, ToolDoAfterEvent>)OnWelderDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WelderComponent, ItemToggledEvent>((EntityEventRefHandler<WelderComponent, ItemToggledEvent>)OnToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WelderComponent, ItemToggleActivateAttemptEvent>((EntityEventRefHandler<WelderComponent, ItemToggleActivateAttemptEvent>)OnActivateAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WelderComponent, ItemToggleDeactivateAttemptEvent>((EntityEventRefHandler<WelderComponent, ItemToggleDeactivateAttemptEvent>)OnDeactivateAttempt, (Type[])null, (Type[])null);
	}

	public void TurnOn(Entity<WelderComponent> entity, EntityUid? user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (SolutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.FuelSolutionName, out Entity<SolutionComponent>? solutionComp, out Solution _))
		{
			SolutionContainerSystem.RemoveReagent(solutionComp.Value, ProtoId<ReagentPrototype>.op_Implicit(entity.Comp.FuelReagent), entity.Comp.FuelLitCost);
			ISharedAdminLogManager adminLogger = AdminLogger;
			LogStringHandler handler = new LogStringHandler(12, 2);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" toggled ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity.Owner)), "welder", "ToPrettyString(entity.Owner)");
			handler.AppendLiteral(" on");
			adminLogger.Add(LogType.InteractActivate, LogImpact.Low, ref handler);
			entity.Comp.Enabled = true;
			((EntitySystem)this).Dirty(Entity<WelderComponent>.op_Implicit(entity), (IComponent)(object)entity.Comp, (MetaDataComponent)null);
		}
	}

	public void TurnOff(Entity<WelderComponent> entity, EntityUid? user)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		ISharedAdminLogManager adminLogger = AdminLogger;
		LogStringHandler handler = new LogStringHandler(13, 2);
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "user", "ToPrettyString(user)");
		handler.AppendLiteral(" toggled ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity.Owner)), "welder", "ToPrettyString(entity.Owner)");
		handler.AppendLiteral(" off");
		adminLogger.Add(LogType.InteractActivate, LogImpact.Low, ref handler);
		entity.Comp.Enabled = false;
		((EntitySystem)this).Dirty(Entity<WelderComponent>.op_Implicit(entity), (IComponent)(object)entity.Comp, (MetaDataComponent)null);
	}

	public (FixedPoint2 fuel, FixedPoint2 capacity) GetWelderFuelAndCapacity(EntityUid uid, WelderComponent? welder = null, SolutionContainerManagerComponent? solutionContainer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<WelderComponent, SolutionContainerManagerComponent>(uid, ref welder, ref solutionContainer, true))
		{
			return default((FixedPoint2, FixedPoint2));
		}
		if (!SolutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, solutionContainer)), welder.FuelSolutionName, out Entity<SolutionComponent>? _, out Solution fuelSolution))
		{
			return default((FixedPoint2, FixedPoint2));
		}
		return (fuel: fuelSolution.GetTotalPrototypeQuantity(ProtoId<ReagentPrototype>.op_Implicit(welder.FuelReagent)), capacity: fuelSolution.MaxVolume);
	}

	private void OnWelderExamine(Entity<WelderComponent> entity, ref ExaminedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("WelderComponent"))
		{
			if (ItemToggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(entity.Owner)))
			{
				args.PushMarkup(base.Loc.GetString("welder-component-on-examine-welder-lit-message"));
			}
			else
			{
				args.PushMarkup(base.Loc.GetString("welder-component-on-examine-welder-not-lit-message"));
			}
			if (args.IsInDetailsRange)
			{
				var (fuel, capacity) = GetWelderFuelAndCapacity(entity.Owner, entity.Comp);
				args.PushMarkup(base.Loc.GetString("welder-component-on-examine-detailed-message", new(string, object)[4]
				{
					("colorName", (fuel < capacity / FixedPoint2.New(4f)) ? "darkorange" : "orange"),
					("fuelLeft", fuel),
					("fuelCapacity", capacity),
					("status", string.Empty)
				}));
			}
		}
	}

	private void CanCancelWelderUse(Entity<WelderComponent> entity, EntityUid user, float requiredFuel, CancellableEntityEventArgs ev)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!ItemToggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(entity.Owner)))
		{
			_popup.PopupClient(base.Loc.GetString("welder-component-welder-not-lit-message"), Entity<WelderComponent>.op_Implicit(entity), user);
			ev.Cancel();
		}
		FixedPoint2 currentFuel = GetWelderFuelAndCapacity(Entity<WelderComponent>.op_Implicit(entity)).fuel;
		if (requiredFuel > currentFuel)
		{
			_popup.PopupClient(base.Loc.GetString("welder-component-cannot-weld-message"), Entity<WelderComponent>.op_Implicit(entity), user);
			ev.Cancel();
		}
	}

	private void OnWelderDoAfter(Entity<WelderComponent> ent, ref ToolDoAfterEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && SolutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.FuelSolutionName, out Entity<SolutionComponent>? solution))
		{
			SolutionContainerSystem.RemoveReagent(solution.Value, ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.FuelReagent), FixedPoint2.New(args.Fuel));
		}
	}

	private void OnToggle(Entity<WelderComponent> entity, ref ItemToggledEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (args.Activated)
		{
			TurnOn(entity, args.User);
		}
		else
		{
			TurnOff(entity, args.User);
		}
	}

	private void OnActivateAttempt(Entity<WelderComponent> entity, ref ItemToggleActivateAttemptEvent args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue && !_actionBlocker.CanComplexInteract(args.User.Value))
		{
			args.Cancelled = true;
			return;
		}
		if (!SolutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.FuelSolutionName, out Entity<SolutionComponent>? _, out Solution solution))
		{
			args.Cancelled = true;
			args.Popup = base.Loc.GetString("welder-component-no-fuel-message");
			return;
		}
		FixedPoint2 fuel = solution.GetTotalPrototypeQuantity(ProtoId<ReagentPrototype>.op_Implicit(entity.Comp.FuelReagent));
		if (fuel == FixedPoint2.Zero || fuel < entity.Comp.FuelLitCost)
		{
			args.Popup = base.Loc.GetString("welder-component-no-fuel-message");
			args.Cancelled = true;
		}
	}

	private void OnDeactivateAttempt(Entity<WelderComponent> entity, ref ItemToggleDeactivateAttemptEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue && !_actionBlocker.CanComplexInteract(args.User.Value))
		{
			args.Cancelled = true;
		}
	}
}
