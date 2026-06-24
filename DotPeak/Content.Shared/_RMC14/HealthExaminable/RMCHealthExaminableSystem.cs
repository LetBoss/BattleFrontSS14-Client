// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.HealthExaminable.RMCHealthExaminableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System.Collections.Immutable;

#nullable enable
namespace Content.Shared._RMC14.HealthExaminable;

public sealed class RMCHealthExaminableSystem : EntitySystem
{
  private readonly ImmutableArray<FixedPoint2> _thresholds = ImmutableArray.Create<FixedPoint2>((FixedPoint2) 25, (FixedPoint2) 50, (FixedPoint2) 75);

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCHealthExaminableComponent, ExaminedEvent>(new EntityEventRefHandler<RMCHealthExaminableComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(Entity<RMCHealthExaminableComponent> ent, ref ExaminedEvent args)
  {
    DamageableComponent comp;
    if (!this.TryComp<DamageableComponent>((EntityUid) ent, out comp))
      return;
    using (args.PushGroup(nameof (RMCHealthExaminableSystem), -1))
    {
      foreach (ProtoId<DamageGroupPrototype> group in ent.Comp.Groups)
      {
        FixedPoint2 fixedPoint2;
        if (comp.DamagePerGroup.TryGetValue((string) group, out fixedPoint2))
        {
          for (int index = this._thresholds.Length - 1; index >= 0; --index)
          {
            FixedPoint2 threshold = this._thresholds[index];
            if (!(fixedPoint2 < threshold))
            {
              string markup;
              if (this.Loc.TryGetString($"rmc-health-examinable-{group}-{threshold.Int()}", out markup, ("target", (object) Identity.Entity((EntityUid) ent, (IEntityManager) this.EntityManager, new EntityUid?(args.Examiner)))))
              {
                args.PushMarkup(markup);
                break;
              }
            }
          }
        }
      }
    }
  }
}
