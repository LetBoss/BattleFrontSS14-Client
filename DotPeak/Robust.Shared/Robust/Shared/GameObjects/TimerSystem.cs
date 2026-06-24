// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TimerSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;

#nullable disable
namespace Robust.Shared.GameObjects;

public sealed class TimerSystem : EntitySystem
{
  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<TimerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TimerComponent>();
    ValueList<(EntityUid, TimerComponent)> valueList = new ValueList<(EntityUid, TimerComponent)>();
    EntityUid uid1;
    TimerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1))
      valueList.Add((uid1, comp1));
    foreach ((EntityUid, TimerComponent) valueTuple in valueList)
      valueTuple.Item2.Update(frameTime);
    foreach ((EntityUid uid2, TimerComponent timerComponent) in valueList)
    {
      if (!timerComponent.Deleted && !this.EntityManager.Deleted(uid2) && timerComponent.RemoveOnEmpty && timerComponent.TimerCount == 0)
        this.RemComp<TimerComponent>(uid2);
    }
  }
}
