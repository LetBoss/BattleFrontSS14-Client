// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.GuidebookSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GuideHelpComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<GuideHelpComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnGetVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GuideHelpComponent, ActivateInWorldEvent>(new ComponentEventHandler<GuideHelpComponent, ActivateInWorldEvent>((object) this, __methodptr(OnInteract)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GuidebookControlsTestComponent, InteractHandEvent>(new ComponentEventHandler<GuidebookControlsTestComponent, InteractHandEvent>((object) this, __methodptr(OnGuidebookControlsTestInteractHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GuidebookControlsTestComponent, ActivateInWorldEvent>(new ComponentEventHandler<GuidebookControlsTestComponent, ActivateInWorldEvent>((object) this, __methodptr(OnGuidebookControlsTestActivateInWorld)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GuidebookControlsTestComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<GuidebookControlsTestComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(OnGuidebookControlsTestGetAlternateVerbs)), (Type[]) null, (Type[]) null);
  }

  public EntityUid GetGuidebookUser()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (localEntity.HasValue)
      return localEntity.Value;
    if (!this.Exists(this._defaultUser))
      this._defaultUser = this.Spawn((string) null, MapCoordinates.Nullspace, (ComponentRegistry) null, new Angle());
    return this._defaultUser;
  }

  private void OnGetVerbs(
    EntityUid uid,
    GuideHelpComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    if (component.Guides.Count == 0 || this._tags.HasTag(uid, ProtoId<TagPrototype>.op_Implicit("GuideEmbeded")))
      return;
    SortedSet<ExamineVerb> verbs = args.Verbs;
    ExamineVerb examineVerb = new ExamineVerb();
    examineVerb.Text = this.Loc.GetString("guide-help-verb");
    examineVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/information.svg.192dpi.png"));
    examineVerb.Act = (Action) (() =>
    {
      Action<List<ProtoId<GuideEntryPrototype>>, List<ProtoId<GuideEntryPrototype>>, ProtoId<GuideEntryPrototype>?, bool, ProtoId<GuideEntryPrototype>?> onGuidebookOpen = this.OnGuidebookOpen;
      if (onGuidebookOpen == null)
        return;
      onGuidebookOpen(component.Guides, (List<ProtoId<GuideEntryPrototype>>) null, new ProtoId<GuideEntryPrototype>?(), component.IncludeChildren, new ProtoId<GuideEntryPrototype>?(component.Guides[0]));
    });
    examineVerb.ClientExclusive = true;
    examineVerb.CloseMenu = new bool?(true);
    verbs.Add(examineVerb);
  }

  public void OpenHelp(List<ProtoId<GuideEntryPrototype>> guides)
  {
    Action<List<ProtoId<GuideEntryPrototype>>, List<ProtoId<GuideEntryPrototype>>, ProtoId<GuideEntryPrototype>?, bool, ProtoId<GuideEntryPrototype>?> onGuidebookOpen = this.OnGuidebookOpen;
    if (onGuidebookOpen == null)
      return;
    onGuidebookOpen(guides, (List<ProtoId<GuideEntryPrototype>>) null, new ProtoId<GuideEntryPrototype>?(), true, new ProtoId<GuideEntryPrototype>?(guides[0]));
  }

  private void OnInteract(EntityUid uid, GuideHelpComponent component, ActivateInWorldEvent args)
  {
    if (!this._timing.IsFirstTimePredicted || !component.OpenOnActivation || component.Guides.Count == 0 || this._tags.HasTag(uid, ProtoId<TagPrototype>.op_Implicit("GuideEmbeded")))
      return;
    Action<List<ProtoId<GuideEntryPrototype>>, List<ProtoId<GuideEntryPrototype>>, ProtoId<GuideEntryPrototype>?, bool, ProtoId<GuideEntryPrototype>?> onGuidebookOpen = this.OnGuidebookOpen;
    if (onGuidebookOpen != null)
      onGuidebookOpen(component.Guides, (List<ProtoId<GuideEntryPrototype>>) null, new ProtoId<GuideEntryPrototype>?(), component.IncludeChildren, new ProtoId<GuideEntryPrototype>?(component.Guides[0]));
    args.Handled = true;
  }

  private void OnGuidebookControlsTestGetAlternateVerbs(
    EntityUid uid,
    GuidebookControlsTestComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    SortedSet<AlternativeVerb> verbs1 = args.Verbs;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() =>
    {
      if (!Angle.op_Inequality(this.Transform(uid).LocalRotation, Angle.Zero))
        return;
      TransformComponent transformComponent = this.Transform(uid);
      transformComponent.LocalRotation = Angle.op_Subtraction(transformComponent.LocalRotation, Angle.FromDegrees(90.0));
    });
    alternativeVerb1.Text = this.Loc.GetString("guidebook-monkey-unspin");
    alternativeVerb1.Priority = -9999;
    verbs1.Add(alternativeVerb1);
    SortedSet<AlternativeVerb> verbs2 = args.Verbs;
    AlternativeVerb alternativeVerb2 = new AlternativeVerb();
    alternativeVerb2.Act = (Action) (() =>
    {
      this.EnsureComp<PointLightComponent>(uid);
      this._pointLightSystem.SetEnabled(uid, false, (SharedPointLightComponent) null, (MetaDataComponent) null);
      RgbLightControllerComponent rgb = this.EnsureComp<RgbLightControllerComponent>(uid);
      SpriteComponent spriteComponent = this.EnsureComp<SpriteComponent>(uid);
      List<int> layers = new List<int>();
      for (int index = 0; index < spriteComponent.AllLayers.Count<ISpriteLayer>(); ++index)
        layers.Add(index);
      this._rgbLightControllerSystem.SetLayers(uid, layers, rgb);
    });
    alternativeVerb2.Text = this.Loc.GetString("guidebook-monkey-disco");
    alternativeVerb2.Priority = -9998;
    verbs2.Add(alternativeVerb2);
  }

  private void OnGuidebookControlsTestActivateInWorld(
    EntityUid uid,
    GuidebookControlsTestComponent component,
    ActivateInWorldEvent args)
  {
    TransformComponent transformComponent = this.Transform(uid);
    transformComponent.LocalRotation = Angle.op_Addition(transformComponent.LocalRotation, Angle.FromDegrees(90.0));
  }

  private void OnGuidebookControlsTestInteractHand(
    EntityUid uid,
    GuidebookControlsTestComponent component,
    InteractHandEvent args)
  {
    SpeechComponent speechComponent;
    if (!this.TryComp<SpeechComponent>(uid, ref speechComponent))
      return;
    int num = speechComponent.SpeechSounds.HasValue ? 1 : 0;
  }

  public void FakeClientActivateInWorld(EntityUid activated)
  {
    ActivateInWorldEvent activateInWorldEvent = new ActivateInWorldEvent(this.GetGuidebookUser(), activated, true);
    this.RaiseLocalEvent<ActivateInWorldEvent>(activated, activateInWorldEvent, false);
  }

  public void FakeClientAltActivateInWorld(EntityUid activated)
  {
    SortedSet<Verb> localVerbs = this._verbSystem.GetLocalVerbs(activated, this.GetGuidebookUser(), typeof (AlternativeVerb), true);
    if (!localVerbs.Any<Verb>())
      return;
    this._verbSystem.ExecuteVerb(localVerbs.First<Verb>(), this.GetGuidebookUser(), activated);
  }

  public void FakeClientUse(EntityUid activated)
  {
    InteractHandEvent interactHandEvent = new InteractHandEvent(this.GetGuidebookUser(), activated);
    this.RaiseLocalEvent<InteractHandEvent>(activated, interactHandEvent, false);
  }
}
