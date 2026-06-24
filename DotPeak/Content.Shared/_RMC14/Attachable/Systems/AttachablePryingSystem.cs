// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachablePryingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared.Prying.Components;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachablePryingSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachablePryingComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachablePryingComponent, AttachableAlteredEvent>(this.OnAttachableAltered));
  }

  private void OnAttachableAltered(
    Entity<AttachablePryingComponent> ent,
    ref AttachableAlteredEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    switch (args.Alteration)
    {
      case AttachableAlteredType.Attached:
        PryingComponent pryingComponent = this.EnsureComp<PryingComponent>(args.Holder);
        ToolComponent toolComponent = this.EnsureComp<ToolComponent>(args.Holder);
        pryingComponent.SpeedModifier = 0.5f;
        toolComponent.Qualities.Add("Prying", this._prototype);
        toolComponent.UseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/crowbar.ogg");
        this.Dirty(args.Holder, (IComponent) pryingComponent);
        this.Dirty(args.Holder, (IComponent) toolComponent);
        break;
      case AttachableAlteredType.Detached:
        this.RemCompDeferred<PryingComponent>(args.Holder);
        this.RemCompDeferred<ToolComponent>(args.Holder);
        break;
    }
  }
}
