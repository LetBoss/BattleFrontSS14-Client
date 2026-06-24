// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Spawners.SharedTimedDespawnSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Spawners;

public abstract class SharedTimedDespawnSystem : EntitySystem
{
  [Dependency]
  private readonly IGameTiming _timing;
  private readonly HashSet<EntityUid> _queuedDespawnEntities = new HashSet<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    this._queuedDespawnEntities.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<TimedDespawnComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TimedDespawnComponent>();
    EntityUid uid;
    TimedDespawnComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Lifetime -= frameTime;
      if (this.CanDelete(uid) && (double) comp1.Lifetime <= 0.0)
        this._queuedDespawnEntities.Add(uid);
    }
    foreach (EntityUid queuedDespawnEntity in this._queuedDespawnEntities)
    {
      TimedDespawnEvent args = new TimedDespawnEvent();
      this.RaiseLocalEvent<TimedDespawnEvent>(queuedDespawnEntity, ref args);
      this.QueueDel(new EntityUid?(queuedDespawnEntity));
    }
  }

  protected abstract bool CanDelete(EntityUid uid);
}
