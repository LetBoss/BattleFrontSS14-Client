using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Dataset;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Interaction;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.Tag;
using Content.Shared.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Paper;

public sealed class PaperSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private IPrototypeManager _protoMan;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private TagSystem _tagSystem;

	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	[Dependency]
	private MetaDataSystem _metaSystem;

	[Dependency]
	private SharedAudioSystem _audio;

	private static readonly ProtoId<TagPrototype> WriteIgnoreStampsTag = ProtoId<TagPrototype>.op_Implicit("WriteIgnoreStamps");

	private static readonly ProtoId<TagPrototype> WriteTag = ProtoId<TagPrototype>.op_Implicit("Write");

	private EntityQuery<PaperComponent> _paperQuery;

	public override void Initialize()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PaperComponent, MapInitEvent>((EntityEventRefHandler<PaperComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperComponent, ComponentInit>((EntityEventRefHandler<PaperComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperComponent, BeforeActivatableUIOpenEvent>((EntityEventRefHandler<PaperComponent, BeforeActivatableUIOpenEvent>)BeforeUIOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperComponent, ExaminedEvent>((EntityEventRefHandler<PaperComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperComponent, InteractUsingEvent>((EntityEventRefHandler<PaperComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperComponent, PaperComponent.PaperInputTextMessage>((EntityEventRefHandler<PaperComponent, PaperComponent.PaperInputTextMessage>)OnInputTextMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RandomPaperContentComponent, MapInitEvent>((EntityEventRefHandler<RandomPaperContentComponent, MapInitEvent>)OnRandomPaperContentMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivateOnPaperOpenedComponent, PaperWriteEvent>((EntityEventRefHandler<ActivateOnPaperOpenedComponent, PaperWriteEvent>)OnPaperWrite, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperComponent, PaperComponent.PaperSignatureRequestMessage>((EntityEventRefHandler<PaperComponent, PaperComponent.PaperSignatureRequestMessage>)OnSignatureRequest, (Type[])null, (Type[])null);
		_paperQuery = ((EntitySystem)this).GetEntityQuery<PaperComponent>();
	}

	private void OnMapInit(Entity<PaperComponent> entity, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(entity.Comp.Content))
		{
			SetContent(entity, base.Loc.GetString(entity.Comp.Content));
		}
	}

	private void OnInit(Entity<PaperComponent> entity, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.Mode = PaperComponent.PaperAction.Read;
		UpdateUserInterface(entity);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<PaperComponent>.op_Implicit(entity), ref appearance))
		{
			if (entity.Comp.Content != "")
			{
				_appearance.SetData(Entity<PaperComponent>.op_Implicit(entity), (Enum)PaperComponent.PaperVisuals.Status, (object)PaperComponent.PaperStatus.Written, appearance);
			}
			if (entity.Comp.StampState != null)
			{
				_appearance.SetData(Entity<PaperComponent>.op_Implicit(entity), (Enum)PaperComponent.PaperVisuals.Stamp, (object)entity.Comp.StampState, appearance);
			}
		}
	}

	private void BeforeUIOpen(Entity<PaperComponent> entity, ref BeforeActivatableUIOpenEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.Mode = PaperComponent.PaperAction.Read;
		UpdateUserInterface(entity);
	}

	private void OnExamined(Entity<PaperComponent> entity, ref ExaminedEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsInDetailsRange)
		{
			return;
		}
		using (args.PushGroup("PaperComponent"))
		{
			if (entity.Comp.Content != "")
			{
				args.PushMarkup(base.Loc.GetString("paper-component-examine-detail-has-words", (ValueTuple<string, object>)("paper", entity)));
			}
			if (entity.Comp.StampedBy.Count > 0)
			{
				string commaSeparated = string.Join(", ", entity.Comp.StampedBy.Select((StampDisplayInfo s) => base.Loc.GetString(s.StampedName)));
				args.PushMarkup(base.Loc.GetString("paper-component-examine-detail-stamped-by", (ValueTuple<string, object>)("paper", entity), (ValueTuple<string, object>)("stamps", commaSeparated)));
			}
		}
	}

	private void OnInteractUsing(Entity<PaperComponent> entity, ref InteractUsingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		bool editable = entity.Comp.StampedBy.Count == 0 || _tagSystem.HasTag(args.Used, WriteIgnoreStampsTag);
		StampComponent stampComp = default(StampComponent);
		if (_tagSystem.HasTag(args.Used, WriteTag))
		{
			if (editable)
			{
				if (entity.Comp.EditingDisabled)
				{
					string paperEditingDisabledMessage = base.Loc.GetString("paper-tamper-proof-modified-message");
					_popupSystem.PopupClient(paperEditingDisabledMessage, Entity<PaperComponent>.op_Implicit(entity), args.User);
					((HandledEntityEventArgs)args).Handled = true;
					return;
				}
				PaperWriteAttemptEvent ev = new PaperWriteAttemptEvent(entity.Owner);
				((EntitySystem)this).RaiseLocalEvent<PaperWriteAttemptEvent>(args.User, ref ev, false);
				if (ev.Cancelled)
				{
					if (ev.FailReason != null)
					{
						string fileWriteMessage = base.Loc.GetString(ev.FailReason);
						_popupSystem.PopupClient(fileWriteMessage, entity.Owner, args.User);
					}
					((HandledEntityEventArgs)args).Handled = true;
					return;
				}
				PaperWriteEvent writeEvent = new PaperWriteEvent(args.User, Entity<PaperComponent>.op_Implicit(entity));
				((EntitySystem)this).RaiseLocalEvent<PaperWriteEvent>(args.Used, ref writeEvent, false);
				entity.Comp.Mode = PaperComponent.PaperAction.Write;
				_uiSystem.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum)PaperComponent.PaperUiKey.Key, (EntityUid?)args.User, false);
				UpdateUserInterface(entity);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
		else if (((EntitySystem)this).TryComp<StampComponent>(args.Used, ref stampComp) && TryStamp(entity, GetStampInfo(stampComp), stampComp.StampState))
		{
			string stampPaperOtherMessage = base.Loc.GetString("paper-component-action-stamp-paper-other", new(string, object)[3]
			{
				("user", args.User),
				("target", args.Target),
				("stamp", args.Used)
			});
			_popupSystem.PopupEntity(stampPaperOtherMessage, args.User, Filter.PvsExcept(args.User, 2f, (IEntityManager)(object)base.EntityManager), recordReplay: true);
			string stampPaperSelfMessage = base.Loc.GetString("paper-component-action-stamp-paper-self", (ValueTuple<string, object>)("target", args.Target), (ValueTuple<string, object>)("stamp", args.Used));
			_popupSystem.PopupClient(stampPaperSelfMessage, args.User, args.User);
			_audio.PlayPredicted(stampComp.Sound, Entity<PaperComponent>.op_Implicit(entity), (EntityUid?)args.User, (AudioParams?)null);
			UpdateUserInterface(entity);
		}
	}

	private static StampDisplayInfo GetStampInfo(StampComponent stamp)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		StampDisplayInfo result = new StampDisplayInfo();
		result.StampedName = stamp.StampedName;
		result.StampedColor = stamp.StampedColor;
		return result;
	}

	private void OnInputTextMessage(Entity<PaperComponent> entity, ref PaperComponent.PaperInputTextMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		PaperWriteAttemptEvent ev = new PaperWriteAttemptEvent(entity.Owner);
		((EntitySystem)this).RaiseLocalEvent<PaperWriteAttemptEvent>(((BaseBoundUserInterfaceEvent)args).Actor, ref ev, false);
		if (ev.Cancelled)
		{
			return;
		}
		if (args.Text.Length <= entity.Comp.ContentSize)
		{
			SetContent(entity, args.Text);
			PaperComponent.PaperStatus paperStatus = ((!string.IsNullOrWhiteSpace(args.Text)) ? PaperComponent.PaperStatus.Written : PaperComponent.PaperStatus.Blank);
			AppearanceComponent appearance = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<PaperComponent>.op_Implicit(entity), ref appearance))
			{
				_appearance.SetData(Entity<PaperComponent>.op_Implicit(entity), (Enum)PaperComponent.PaperVisuals.Status, (object)paperStatus, appearance);
			}
			MetaDataComponent meta = default(MetaDataComponent);
			if (((EntitySystem)this).TryComp(Entity<PaperComponent>.op_Implicit(entity), ref meta))
			{
				_metaSystem.SetEntityDescription(Entity<PaperComponent>.op_Implicit(entity), "", meta);
			}
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(37, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
			handler.AppendLiteral(" has written on ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<PaperComponent>.op_Implicit(entity), (MetaDataComponent)null), "entity", "ToPrettyString(entity)");
			handler.AppendLiteral(" the following text: ");
			handler.AppendFormatted(args.Text);
			adminLogger.Add(LogType.Chat, LogImpact.Low, ref handler);
			_audio.PlayPvs(entity.Comp.Sound, Entity<PaperComponent>.op_Implicit(entity), (AudioParams?)null);
		}
		entity.Comp.Mode = PaperComponent.PaperAction.Read;
		UpdateUserInterface(entity);
	}

	private void OnRandomPaperContentMapInit(Entity<RandomPaperContentComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		PaperComponent paperComp = default(PaperComponent);
		if (!_paperQuery.TryComp(Entity<RandomPaperContentComponent>.op_Implicit(ent), ref paperComp))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RandomPaperContentComponent>.op_Implicit(ent), (MetaDataComponent)null)} has a {"RandomPaperContentComponent"} but no {"PaperComponent"}!");
			((EntitySystem)this).RemCompDeferred(Entity<RandomPaperContentComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp);
		}
		else
		{
			LocalizedDatasetPrototype dataset = _protoMan.Index<LocalizedDatasetPrototype>(ent.Comp.Dataset);
			string pick = RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)dataset.Values);
			_metaSystem.SetEntityName(Entity<RandomPaperContentComponent>.op_Implicit(ent), base.Loc.GetString(pick), (MetaDataComponent)null, true);
			_metaSystem.SetEntityDescription(Entity<RandomPaperContentComponent>.op_Implicit(ent), base.Loc.GetString(pick + ".desc"), (MetaDataComponent)null);
			SetContent(Entity<PaperComponent>.op_Implicit((Entity<RandomPaperContentComponent>.op_Implicit(ent), paperComp)), base.Loc.GetString(pick + ".content"));
			((EntitySystem)this).RemCompDeferred(Entity<RandomPaperContentComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp);
		}
	}

	private void OnPaperWrite(Entity<ActivateOnPaperOpenedComponent> entity, ref PaperWriteEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_interaction.UseInHandInteraction(args.User, Entity<ActivateOnPaperOpenedComponent>.op_Implicit(entity));
	}

	public bool TryStamp(Entity<PaperComponent> entity, StampDisplayInfo stampInfo, string spriteStampState)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.StampedBy.Contains(stampInfo))
		{
			entity.Comp.StampedBy.Add(stampInfo);
			string cleanedContent = CleanUnfilledTags(entity.Comp.Content);
			if (cleanedContent != entity.Comp.Content)
			{
				SetContent(entity, cleanedContent);
			}
			((EntitySystem)this).Dirty<PaperComponent>(entity, (MetaDataComponent)null);
			AppearanceComponent appearance = default(AppearanceComponent);
			if (entity.Comp.StampState == null && ((EntitySystem)this).TryComp<AppearanceComponent>(Entity<PaperComponent>.op_Implicit(entity), ref appearance))
			{
				entity.Comp.StampState = spriteStampState;
				_appearance.SetData(Entity<PaperComponent>.op_Implicit(entity), (Enum)PaperComponent.PaperVisuals.Stamp, (object)entity.Comp.StampState, appearance);
			}
		}
		return true;
	}

	public void CopyStamps(Entity<PaperComponent?> source, Entity<PaperComponent?> target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PaperComponent>(Entity<PaperComponent>.op_Implicit(source), ref source.Comp, true) && ((EntitySystem)this).Resolve<PaperComponent>(Entity<PaperComponent>.op_Implicit(target), ref target.Comp, true))
		{
			target.Comp.StampedBy = new List<StampDisplayInfo>(source.Comp.StampedBy);
			target.Comp.StampState = source.Comp.StampState;
			((EntitySystem)this).Dirty<PaperComponent>(target, (MetaDataComponent)null);
			AppearanceComponent appearance = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<PaperComponent>.op_Implicit(target), ref appearance))
			{
				_appearance.SetData(Entity<PaperComponent>.op_Implicit(target), (Enum)PaperComponent.PaperVisuals.Stamp, (object)(target.Comp.StampState ?? ""), appearance);
			}
		}
	}

	public void SetContent(EntityUid entity, string content)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		PaperComponent paper = default(PaperComponent);
		if (((EntitySystem)this).TryComp<PaperComponent>(entity, ref paper))
		{
			SetContent(Entity<PaperComponent>.op_Implicit((entity, paper)), content);
		}
	}

	public void SetContent(Entity<PaperComponent> entity, string content)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.Content = content;
		((EntitySystem)this).Dirty<PaperComponent>(entity, (MetaDataComponent)null);
		UpdateUserInterface(entity);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<PaperComponent>.op_Implicit(entity), ref appearance))
		{
			PaperComponent.PaperStatus status = ((!string.IsNullOrWhiteSpace(content)) ? PaperComponent.PaperStatus.Written : PaperComponent.PaperStatus.Blank);
			_appearance.SetData(Entity<PaperComponent>.op_Implicit(entity), (Enum)PaperComponent.PaperVisuals.Status, (object)status, appearance);
		}
	}

	private void UpdateUserInterface(Entity<PaperComponent> entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		_uiSystem.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum)PaperComponent.PaperUiKey.Key, (BoundUserInterfaceState)(object)new PaperComponent.PaperBoundUserInterfaceState(entity.Comp.Content, entity.Comp.StampedBy, entity.Comp.Mode));
	}

	private void OnSignatureRequest(Entity<PaperComponent> entity, ref PaperComponent.PaperSignatureRequestMessage args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		string signature = GetPlayerSignature(((BaseBoundUserInterfaceEvent)args).Actor);
		string newText = ReplaceNthSignatureTag(entity.Comp.Content, args.SignatureIndex, signature);
		SetContent(entity, newText);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(25, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" signed ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<PaperComponent>.op_Implicit(entity), (MetaDataComponent)null), "entity", "ToPrettyString(entity)");
		handler.AppendLiteral(" with signature: ");
		handler.AppendFormatted(signature);
		adminLogger.Add(LogType.Chat, LogImpact.Low, ref handler);
	}

	private string GetPlayerSignature(EntityUid player)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		string name = string.Empty;
		string rank = string.Empty;
		string role = string.Empty;
		EntityUid identityEntity = player;
		IdentityComponent identity = default(IdentityComponent);
		if (((EntitySystem)this).TryComp<IdentityComponent>(player, ref identity))
		{
			EntityUid? containedEntity = identity.IdentityEntitySlot.ContainedEntity;
			if (containedEntity.HasValue)
			{
				EntityUid idEntity = containedEntity.GetValueOrDefault();
				identityEntity = idEntity;
			}
		}
		name = ((EntitySystem)this).MetaData(identityEntity).EntityName;
		RankComponent rankComp = default(RankComponent);
		if (((EntitySystem)this).TryComp<RankComponent>(player, ref rankComp))
		{
			rank = base.EntityManager.System<SharedRankSystem>().GetRankString(player, isShort: true) ?? string.Empty;
		}
		MindContainerComponent mindContainer = default(MindContainerComponent);
		if (((EntitySystem)this).TryComp<MindContainerComponent>(player, ref mindContainer) && mindContainer.Mind.HasValue)
		{
			List<RoleInfo> roleInfo = base.EntityManager.System<SharedRoleSystem>().MindGetAllRoleInfo(Entity<MindComponent>.op_Implicit((ValueTuple<EntityUid, MindComponent>)(mindContainer.Mind.Value, null)));
			if (roleInfo.Count > 0)
			{
				role = base.Loc.GetString(roleInfo[0].Name);
			}
		}
		string signature = string.Empty;
		if (!string.IsNullOrEmpty(rank) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(role))
		{
			return $"{rank} {name}, {role}";
		}
		if (!string.IsNullOrEmpty(rank) && !string.IsNullOrEmpty(name))
		{
			return rank + " " + name;
		}
		if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(role))
		{
			return name + ", " + role;
		}
		return name;
	}

	private static string ReplaceNthSignatureTag(string text, int index, string replacement)
	{
		int currentIndex = 0;
		int pos = 0;
		while (pos < text.Length)
		{
			int foundPos = text.IndexOf("[signature]", pos);
			if (foundPos == -1)
			{
				break;
			}
			if (currentIndex == index)
			{
				return text.Substring(0, foundPos) + replacement + text.Substring(foundPos + "[signature]".Length);
			}
			currentIndex++;
			pos = foundPos + "[signature]".Length;
		}
		return text;
	}

	private static string CleanUnfilledTags(string text)
	{
		return text.Replace("[form]", string.Empty).Replace("[signature]", string.Empty).Replace("[check]", "☐");
	}
}
