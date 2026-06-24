using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Guidebook.Components;
using Content.Client.Light;
using Content.Client.Verbs;
using Content.Shared.Guidebook;
using Content.Shared.Interaction;
using Content.Shared.Light.Components;
using Content.Shared.Speech;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook;

public sealed class GuidebookSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private VerbSystem _verbSystem;

	[Dependency]
	private RgbLightControllerSystem _rgbLightControllerSystem;

	[Dependency]
	private SharedPointLightSystem _pointLightSystem;

	[Dependency]
	private TagSystem _tags;

	public const string GuideEmbedTag = "GuideEmbeded";

	private EntityUid _defaultUser;

	public event Action<List<ProtoId<GuideEntryPrototype>>, List<ProtoId<GuideEntryPrototype>>?, ProtoId<GuideEntryPrototype>?, bool, ProtoId<GuideEntryPrototype>?>? OnGuidebookOpen;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GuideHelpComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<GuideHelpComponent, GetVerbsEvent<ExamineVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GuideHelpComponent, ActivateInWorldEvent>((ComponentEventHandler<GuideHelpComponent, ActivateInWorldEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GuidebookControlsTestComponent, InteractHandEvent>((ComponentEventHandler<GuidebookControlsTestComponent, InteractHandEvent>)OnGuidebookControlsTestInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GuidebookControlsTestComponent, ActivateInWorldEvent>((ComponentEventHandler<GuidebookControlsTestComponent, ActivateInWorldEvent>)OnGuidebookControlsTestActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GuidebookControlsTestComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<GuidebookControlsTestComponent, GetVerbsEvent<AlternativeVerb>>)OnGuidebookControlsTestGetAlternateVerbs, (Type[])null, (Type[])null);
	}

	public EntityUid GetGuidebookUser()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			return localEntity.Value;
		}
		if (!((EntitySystem)this).Exists(_defaultUser))
		{
			_defaultUser = ((EntitySystem)this).Spawn((string)null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
		}
		return _defaultUser;
	}

	private void OnGetVerbs(EntityUid uid, GuideHelpComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		if (component.Guides.Count != 0 && !_tags.HasTag(uid, ProtoId<TagPrototype>.op_Implicit("GuideEmbeded")))
		{
			args.Verbs.Add(new ExamineVerb
			{
				Text = base.Loc.GetString("guide-help-verb"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/information.svg.192dpi.png")),
				Act = delegate
				{
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					this.OnGuidebookOpen?.Invoke(component.Guides, null, null, component.IncludeChildren, component.Guides[0]);
				},
				ClientExclusive = true,
				CloseMenu = true
			});
		}
	}

	public void OpenHelp(List<ProtoId<GuideEntryPrototype>> guides)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		this.OnGuidebookOpen?.Invoke(guides, null, null, arg4: true, guides[0]);
	}

	private void OnInteract(EntityUid uid, GuideHelpComponent component, ActivateInWorldEvent args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted && component.OpenOnActivation && component.Guides.Count != 0 && !_tags.HasTag(uid, ProtoId<TagPrototype>.op_Implicit("GuideEmbeded")))
		{
			this.OnGuidebookOpen?.Invoke(component.Guides, null, null, component.IncludeChildren, component.Guides[0]);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnGuidebookControlsTestGetAlternateVerbs(EntityUid uid, GuidebookControlsTestComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		args.Verbs.Add(new AlternativeVerb
		{
			Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				if (((EntitySystem)this).Transform(uid).LocalRotation != Angle.Zero)
				{
					TransformComponent obj = ((EntitySystem)this).Transform(uid);
					obj.LocalRotation -= Angle.FromDegrees(90.0);
				}
			},
			Text = base.Loc.GetString("guidebook-monkey-unspin"),
			Priority = -9999
		});
		args.Verbs.Add(new AlternativeVerb
		{
			Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				((EntitySystem)this).EnsureComp<PointLightComponent>(uid);
				_pointLightSystem.SetEnabled(uid, false, (SharedPointLightComponent)null, (MetaDataComponent)null);
				RgbLightControllerComponent rgb = ((EntitySystem)this).EnsureComp<RgbLightControllerComponent>(uid);
				SpriteComponent val = ((EntitySystem)this).EnsureComp<SpriteComponent>(uid);
				List<int> list = new List<int>();
				for (int i = 0; i < val.AllLayers.Count(); i++)
				{
					list.Add(i);
				}
				_rgbLightControllerSystem.SetLayers(uid, list, rgb);
			},
			Text = base.Loc.GetString("guidebook-monkey-disco"),
			Priority = -9998
		});
	}

	private void OnGuidebookControlsTestActivateInWorld(EntityUid uid, GuidebookControlsTestComponent component, ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent obj = ((EntitySystem)this).Transform(uid);
		obj.LocalRotation += Angle.FromDegrees(90.0);
	}

	private void OnGuidebookControlsTestInteractHand(EntityUid uid, GuidebookControlsTestComponent component, InteractHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SpeechComponent speechComponent = default(SpeechComponent);
		if (((EntitySystem)this).TryComp<SpeechComponent>(uid, ref speechComponent))
		{
			ProtoId<SpeechSoundsPrototype>? speechSounds = speechComponent.SpeechSounds;
			_ = speechSounds.HasValue;
		}
	}

	public void FakeClientActivateInWorld(EntityUid activated)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ActivateInWorldEvent activateInWorldEvent = new ActivateInWorldEvent(GetGuidebookUser(), activated, complex: true);
		((EntitySystem)this).RaiseLocalEvent<ActivateInWorldEvent>(activated, activateInWorldEvent, false);
	}

	public void FakeClientAltActivateInWorld(EntityUid activated)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		SortedSet<Verb> localVerbs = _verbSystem.GetLocalVerbs(activated, GetGuidebookUser(), typeof(AlternativeVerb), force: true);
		if (localVerbs.Any())
		{
			_verbSystem.ExecuteVerb(localVerbs.First(), GetGuidebookUser(), activated);
		}
	}

	public void FakeClientUse(EntityUid activated)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		InteractHandEvent interactHandEvent = new InteractHandEvent(GetGuidebookUser(), activated);
		((EntitySystem)this).RaiseLocalEvent<InteractHandEvent>(activated, interactHandEvent, false);
	}
}
