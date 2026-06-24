using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Content.Client.Verbs;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client.Examine;

public sealed class ExamineSystem : ExamineSystemShared
{
	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private VerbSystem _verbSystem;

	[Dependency]
	private SpriteSystem _sprite;

	private List<Verb> _verbList = new List<Verb>();

	public const string StyleClassEntityTooltip = "entity-tooltip";

	private EntityUid _examinedEntity;

	private Popup? _examineTooltipOpen;

	private ScreenCoordinates _popupPos;

	private CancellationTokenSource? _requestCancelTokenSource;

	private int _idCounter;

	public override void Initialize()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		base.Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).SubscribeLocalEvent<GetVerbsEvent<ExamineVerb>>((EntityEventHandler<GetVerbsEvent<ExamineVerb>>)AddExamineVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<ExamineSystemMessages.ExamineInfoResponseMessage>((EntityEventHandler<ExamineSystemMessages.ExamineInfoResponseMessage>)OnExamineInfoResponse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemComponent, DroppedEvent>((ComponentEventHandler<ItemComponent, DroppedEvent>)OnExaminedItemDropped, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(ContentKeyFunctions.ExamineEntity, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleExamine), true, true)).Register<ExamineSystem>();
		_idCounter = 0;
	}

	private void OnExaminedItemDropped(EntityUid item, ItemComponent comp, DroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (((EntityUid)(ref user)).Valid && _examineTooltipOpen != null && item == _examinedEntity)
		{
			user = args.User;
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			if (localEntity.HasValue && user == localEntity.GetValueOrDefault())
			{
				CloseTooltip();
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Popup examineTooltipOpen = _examineTooltipOpen;
		if (examineTooltipOpen == null || !((Control)examineTooltipOpen).Visible || !((EntityUid)(ref _examinedEntity)).Valid)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (!CanExamine(valueOrDefault, _examinedEntity))
			{
				CloseTooltip();
			}
		}
	}

	public override void Shutdown()
	{
		CommandBinds.Unregister<ExamineSystem>();
		((EntitySystem)this).Shutdown();
	}

	public override bool CanExamine(EntityUid examiner, MapCoordinates target, SharedInteractionSystem.Ignored? predicate = null, EntityUid? examined = null, ExaminerComponent? examinerComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ExaminerComponent>(examiner, ref examinerComp, false))
		{
			return false;
		}
		if (examinerComp.SkipChecks)
		{
			return true;
		}
		if (examinerComp.CheckInRangeUnOccluded)
		{
			Box2Rotated worldViewbounds = _eyeManager.GetWorldViewbounds();
			if (!((Box2Rotated)(ref worldViewbounds)).Contains(target.Position))
			{
				return false;
			}
		}
		return base.CanExamine(examiner, target, predicate, examined, examinerComp);
	}

	private bool HandleExamine(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entityUid = args.EntityUid;
		if (!((EntityUid)(ref args.EntityUid)).IsValid() || !((EntitySystem)this).Exists(entityUid))
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (CanExamine(valueOrDefault, entityUid))
			{
				DoExamine(entityUid);
				return true;
			}
		}
		return false;
	}

	private void AddExamineVerb(GetVerbsEvent<ExamineVerb> args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		if (CanExamine(args.User, args.Target))
		{
			ExamineVerb examineVerb = new ExamineVerb();
			examineVerb.Category = VerbCategory.Examine;
			examineVerb.Priority = 10;
			examineVerb.Act = delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				DoExamine(args.Target, centeredOnCursor: false);
			};
			examineVerb.Text = ((EntitySystem)this).Loc.GetString("examine-verb-name");
			examineVerb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"));
			examineVerb.ShowOnExamineTooltip = false;
			examineVerb.ClientExclusive = true;
			args.Verbs.Add(examineVerb);
		}
	}

	private void OnExamineInfoResponse(ExamineSystemMessages.ExamineInfoResponseMessage ev)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && (ev.Id == 0 || ev.Id == _idCounter))
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(ev.EntityUid);
			OpenTooltip(localEntity.Value, entity, ev.CenterAtCursor, ev.OpenAtOldTooltip, ev.KnowTarget);
			UpdateTooltipInfo(localEntity.Value, entity, ev.Message, ev.Verbs, getVerbs: false);
		}
	}

	public override void SendExamineTooltip(EntityUid player, EntityUid target, FormattedMessage message, bool getVerbs, bool centerAtCursor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		OpenTooltip(player, target, centerAtCursor);
		UpdateTooltipInfo(player, target, message, null, getVerbs);
	}

	public void OpenTooltip(EntityUid player, EntityUid target, bool centeredOnCursor = true, bool openAtOldTooltip = true, bool knowTarget = true)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		ScreenCoordinates? val = ((_examineTooltipOpen != null) ? new ScreenCoordinates?(_popupPos) : ((ScreenCoordinates?)null));
		CloseTooltip();
		_examinedEntity = target;
		if (openAtOldTooltip && val.HasValue)
		{
			_popupPos = val.Value;
		}
		else if (centeredOnCursor)
		{
			_popupPos = _userInterfaceManager.MousePositionScaled;
		}
		else
		{
			_popupPos = _eyeManager.CoordinatesToScreen(((EntitySystem)this).Transform(target).Coordinates);
			_popupPos = _userInterfaceManager.ScreenToUIPosition(_popupPos);
		}
		_examineTooltipOpen = new Popup
		{
			MaxWidth = 400f
		};
		((Control)_userInterfaceManager.ModalRoot).AddChild((Control)(object)_examineTooltipOpen);
		PanelContainer val2 = new PanelContainer
		{
			Name = "ExaminePopupPanel"
		};
		((Control)val2).AddStyleClass("entity-tooltip");
		Color lightGray = Color.LightGray;
		((Control)val2).ModulateSelfOverride = ((Color)(ref lightGray)).WithAlpha(0.9f);
		((Control)_examineTooltipOpen).AddChild((Control)(object)val2);
		BoxContainer val3 = new BoxContainer
		{
			Name = "ExaminePopupVbox",
			Orientation = (LayoutOrientation)1,
			MaxWidth = ((Control)_examineTooltipOpen).MaxWidth
		};
		((Control)val2).AddChild((Control)(object)val3);
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 5,
			Margin = new Thickness(6f, 0f, 6f, 0f)
		};
		((Control)val3).AddChild((Control)(object)val4);
		if (((EntitySystem)this).HasComp<SpriteComponent>(target))
		{
			SpriteView val5 = new SpriteView
			{
				OverrideDirection = (Direction)0,
				SetSize = new Vector2(32f, 32f)
			};
			val5.SetEntity((EntityUid?)target);
			((Control)val4).AddChild((Control)(object)val5);
		}
		if (knowTarget)
		{
			string text = FormattedMessage.EscapeText((string)Identity.Name(target, (IEntityManager)(object)((EntitySystem)this).EntityManager, player));
			FormattedMessage val6 = FormattedMessage.FromMarkupPermissive("[bold]" + text + "[/bold]");
			RichTextLabel val7 = new RichTextLabel();
			val7.SetMessage(val6, (Color?)null);
			((Control)val4).AddChild((Control)(object)val7);
		}
		else
		{
			RichTextLabel val8 = new RichTextLabel();
			val8.SetMessage(FormattedMessage.FromMarkupOrThrow("[bold]???[/bold]"), (Color?)null);
			((Control)val4).AddChild((Control)(object)val8);
		}
		((Control)val2).Measure(Vector2Helpers.Infinity);
		Vector2 vector = Vector2.Max(new Vector2(300f, 0f), ((Control)val2).DesiredSize);
		_examineTooltipOpen.Open((UIBox2?)UIBox2.FromDimensions(_popupPos.Position, vector), (Vector2?)null, (Vector2?)null);
	}

	public void UpdateTooltipInfo(EntityUid player, EntityUid target, FormattedMessage message, List<Verb>? verbs = null, bool getVerbs = true)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		Popup? examineTooltipOpen = _examineTooltipOpen;
		Control val = ((examineTooltipOpen != null) ? ((Control)examineTooltipOpen).GetChild(0).GetChild(0) : null);
		if (val == null)
		{
			return;
		}
		foreach (MarkupNode node in message.Nodes)
		{
			if (node.Name == null && !string.IsNullOrWhiteSpace(((MarkupParameter)(ref node.Value)).StringValue ?? ""))
			{
				RichTextLabel val2 = new RichTextLabel
				{
					Margin = new Thickness(4f, 4f, 0f, 4f)
				};
				val2.SetMessage(message, (Color?)null);
				val.AddChild((Control)(object)val2);
				break;
			}
		}
		SortedSet<Verb> localVerbs = _verbSystem.GetLocalVerbs(target, player, typeof(ExamineVerb));
		if (!getVerbs)
		{
			_verbList.AddRange(localVerbs);
			foreach (Verb verb in _verbList)
			{
				if (!verb.ClientExclusive)
				{
					localVerbs.Remove(verb);
				}
			}
			_verbList.Clear();
		}
		if (verbs != null)
		{
			localVerbs.UnionWith(verbs);
		}
		AddVerbsToTooltip(localVerbs);
	}

	private void AddVerbsToTooltip(IEnumerable<Verb> verbs)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		if (_examineTooltipOpen == null)
		{
			return;
		}
		BoxContainer val = new BoxContainer
		{
			Name = "ExamineButtonsHBox",
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)0,
			VerticalAlignment = (VAlignment)3
		};
		BoxContainer val2 = new BoxContainer
		{
			Name = "HoverExamineHBox",
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)1,
			VerticalAlignment = (VAlignment)2,
			HorizontalExpand = true
		};
		BoxContainer val3 = new BoxContainer
		{
			Name = "ClickExamineHBox",
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)3,
			VerticalAlignment = (VAlignment)2,
			HorizontalExpand = true
		};
		foreach (Verb verb in verbs)
		{
			if (verb is ExamineVerb { Icon: not null, ShowOnExamineTooltip: not false } examineVerb)
			{
				ExamineButton examineButton = new ExamineButton(examineVerb, _sprite);
				if (examineVerb.HoverVerb)
				{
					((Control)val2).AddChild((Control)(object)examineButton);
					continue;
				}
				((BaseButton)examineButton).OnPressed += VerbButtonPressed;
				((Control)val3).AddChild((Control)(object)examineButton);
			}
		}
		Popup? examineTooltipOpen = _examineTooltipOpen;
		Control val4 = ((examineTooltipOpen != null) ? ((Control)examineTooltipOpen).GetChild(0).GetChild(0) : null);
		if (val4 == null)
		{
			((Control)val).Orphan();
			return;
		}
		Control[] source = ((IEnumerable<Control>)val4.Children).Where((Control c) => c.Name == "ExamineButtonsHBox").ToArray();
		if (source.Any())
		{
			val4.Children.Remove(source.First());
		}
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		val4.AddChild((Control)(object)val);
	}

	public void VerbButtonPressed(ButtonEventArgs obj)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (obj.Button is ExamineButton examineButton)
		{
			_verbSystem.ExecuteVerb(_examinedEntity, examineButton.Verb);
			if (examineButton.Verb.CloseMenu ?? examineButton.Verb.CloseMenuDefault)
			{
				CloseTooltip();
			}
		}
	}

	public void DoExamine(EntityUid entity, bool centeredOnCursor = true, EntityUid? userOverride = null)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? examiner = userOverride ?? ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (examiner.HasValue)
		{
			OpenTooltip(examiner.Value, entity, centeredOnCursor, openAtOldTooltip: false);
			FormattedMessage examineText = GetExamineText(entity, examiner);
			UpdateTooltipInfo(examiner.Value, entity, examineText);
			if (!((EntitySystem)this).IsClientSide(entity, (MetaDataComponent)null))
			{
				_idCounter++;
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new ExamineSystemMessages.RequestExamineInfoMessage(((EntitySystem)this).GetNetEntity(entity, (MetaDataComponent)null), _idCounter, getVerbs: true));
			}
			((EntitySystem)this).RaiseLocalEvent<ClientExaminedEvent>(entity, new ClientExaminedEvent(entity, examiner.Value), false);
		}
	}

	private unsafe void CloseTooltip()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (_examineTooltipOpen != null)
		{
			Enumerator enumerator = ((Control)_examineTooltipOpen).Children.GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator)).MoveNext())
				{
					if (((Enumerator)(ref enumerator)).Current is ExamineButton examineButton)
					{
						((BaseButton)examineButton).OnPressed -= VerbButtonPressed;
					}
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
			((Control)_examineTooltipOpen).Orphan();
			_examineTooltipOpen = null;
		}
		if (_requestCancelTokenSource != null)
		{
			_requestCancelTokenSource.Cancel();
			_requestCancelTokenSource = null;
		}
	}
}
