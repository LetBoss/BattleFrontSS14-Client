// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Crate.CrateOpenableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Random;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Crate;

public sealed class CrateOpenableSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<CrateOpenableComponent, InteractUsingEvent>(new EntityEventRefHandler<CrateOpenableComponent, InteractUsingEvent>(this.OnInteractUsing));
  }

  private void OnInteractUsing(Entity<CrateOpenableComponent> ent, ref InteractUsingEvent args)
  {
    if (this.EntityManager.IsQueuedForDeletion((EntityUid) ent))
      return;
    if (!this._tool.HasQuality(args.Used, (string) ent.Comp.Tool))
    {
      this._popup.PopupClient(this.Loc.GetString((string) ent.Comp.WrongToolPopup), (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      args.Handled = true;
      this._audio.PlayPredicted(ent.Comp.Sound, this._transform.GetMoverCoordinates((EntityUid) ent), new EntityUid?(args.User));
      if (this._net.IsClient)
        return;
      this.QueueDel(new EntityUid?((EntityUid) ent));
      foreach (string spawn in EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) ent.Comp.Spawn, this._random))
        this.TrySpawnNextTo(spawn, (EntityUid) ent, out EntityUid? _);
    }
  }
}
