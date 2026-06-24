// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAE.XAERandomTeleportInvokerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Popups;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Numerics;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAERandomTeleportInvokerSystem : BaseXAESystem<XAERandomTeleportInvokerComponent>
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  private IGameTiming _timing;

  protected override void OnActivated(
    Entity<XAERandomTeleportInvokerComponent> ent,
    ref XenoArtifactNodeActivatedEvent args)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    XAERandomTeleportInvokerComponent comp = ent.Comp;
    TransformComponent xform = this.Transform(ent.Owner);
    this._popup.PopupCoordinates(this.Loc.GetString("blink-artifact-popup"), xform.Coordinates, PopupType.Medium);
    Vector2 position = this._random.NextVector2(comp.MinRange, comp.MaxRange);
    this._xform.SetCoordinates(ent.Owner, xform, xform.Coordinates.Offset(position));
  }
}
