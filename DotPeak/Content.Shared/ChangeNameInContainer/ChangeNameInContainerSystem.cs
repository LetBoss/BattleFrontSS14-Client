// Decompiled with JetBrains decompiler
// Type: Content.Shared.ChangeNameInContainer.ChangeNameInContainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat;
using Content.Shared.Speech;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.ChangeNameInContainer;

public sealed class ChangeNameInContainerSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private EntityWhitelistSystem _whitelist;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChangeVoiceInContainerComponent, TransformSpeakerNameEvent>(new EntityEventRefHandler<ChangeVoiceInContainerComponent, TransformSpeakerNameEvent>((object) this, __methodptr(OnTransformSpeakerName)), (Type[]) null, (Type[]) null);
  }

  private void OnTransformSpeakerName(
    Entity<ChangeVoiceInContainerComponent> ent,
    ref TransformSpeakerNameEvent args)
  {
    BaseContainer baseContainer;
    if (!this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((Entity<ChangeVoiceInContainerComponent>.op_Implicit(ent), (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer) || this._whitelist.IsWhitelistFail(ent.Comp.Whitelist, baseContainer.Owner))
      return;
    args.VoiceName = this.Name(baseContainer.Owner, (MetaDataComponent) null);
    SpeechComponent speechComponent;
    if (!this.TryComp<SpeechComponent>(baseContainer.Owner, ref speechComponent))
      return;
    args.SpeechVerb = new ProtoId<SpeechVerbPrototype>?(speechComponent.SpeechVerb);
  }
}
