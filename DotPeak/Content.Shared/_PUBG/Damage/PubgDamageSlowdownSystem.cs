// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Damage.PubgDamageSlowdownSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared._PUBG.Damage;

public sealed class PubgDamageSlowdownSystem : EntitySystem
{
  private const float SlowdownFactor = 0.5f;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PubgCharacterComponent, ModifySlowOnDamageSpeedEvent>(new EntityEventRefHandler<PubgCharacterComponent, ModifySlowOnDamageSpeedEvent>(this.OnModifySlowOnDamageSpeed), after: new Type[1]
    {
      typeof (InventorySystem)
    });
  }

  private void OnModifySlowOnDamageSpeed(
    Entity<PubgCharacterComponent> ent,
    ref ModifySlowOnDamageSpeedEvent args)
  {
    if ((double) args.Speed <= 0.0 || (double) args.Speed >= 1.0)
      return;
    args.Speed = (float) (1.0 - (1.0 - (double) args.Speed) * 0.5);
  }
}
