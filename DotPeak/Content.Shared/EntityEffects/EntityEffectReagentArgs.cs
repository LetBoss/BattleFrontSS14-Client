// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EntityEffectReagentArgs
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Content.Shared.EntityEffects;

public record EntityEffectReagentArgs : EntityEffectBaseArgs
{
  public EntityUid? OrganEntity;
  public Solution? Source;
  public FixedPoint2 Quantity;
  public ReagentPrototype? Reagent;
  public ReactionMethod? Method;
  public FixedPoint2 Scale;

  public EntityEffectReagentArgs(
    EntityUid targetEntity,
    IEntityManager entityManager,
    EntityUid? organEntity,
    Solution? source,
    FixedPoint2 quantity,
    ReagentPrototype? reagent,
    ReactionMethod? method,
    FixedPoint2 scale)
    : base(targetEntity, entityManager)
  {
    this.OrganEntity = organEntity;
    this.Source = source;
    this.Quantity = quantity;
    this.Reagent = reagent;
    this.Method = method;
    this.Scale = scale;
  }

  [CompilerGenerated]
  protected override bool PrintMembers(StringBuilder builder)
  {
    if (base.PrintMembers(builder))
      builder.Append(", ");
    builder.Append("OrganEntity = ");
    builder.Append(this.OrganEntity.ToString());
    builder.Append(", Source = ");
    builder.Append((object) this.Source);
    builder.Append(", Quantity = ");
    builder.Append(this.Quantity.ToString());
    builder.Append(", Reagent = ");
    builder.Append((object) this.Reagent);
    builder.Append(", Method = ");
    builder.Append(this.Method.ToString());
    builder.Append(", Scale = ");
    builder.Append(this.Scale.ToString());
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (((((base.GetHashCode() * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.OrganEntity)) * -1521134295 + EqualityComparer<Solution>.Default.GetHashCode(this.Source)) * -1521134295 + EqualityComparer<FixedPoint2>.Default.GetHashCode(this.Quantity)) * -1521134295 + EqualityComparer<ReagentPrototype>.Default.GetHashCode(this.Reagent)) * -1521134295 + EqualityComparer<ReactionMethod?>.Default.GetHashCode(this.Method)) * -1521134295 + EqualityComparer<FixedPoint2>.Default.GetHashCode(this.Scale);
  }

  [CompilerGenerated]
  public sealed override bool Equals(EntityEffectBaseArgs? other) => this.Equals((object) other);

  [CompilerGenerated]
  public virtual bool Equals(EntityEffectReagentArgs? other)
  {
    if ((object) this == (object) other)
      return true;
    return base.Equals((EntityEffectBaseArgs) other) && EqualityComparer<EntityUid?>.Default.Equals(this.OrganEntity, other.OrganEntity) && EqualityComparer<Solution>.Default.Equals(this.Source, other.Source) && EqualityComparer<FixedPoint2>.Default.Equals(this.Quantity, other.Quantity) && EqualityComparer<ReagentPrototype>.Default.Equals(this.Reagent, other.Reagent) && EqualityComparer<ReactionMethod?>.Default.Equals(this.Method, other.Method) && EqualityComparer<FixedPoint2>.Default.Equals(this.Scale, other.Scale);
  }

  [CompilerGenerated]
  protected EntityEffectReagentArgs(EntityEffectReagentArgs original)
    : base((EntityEffectBaseArgs) original)
  {
    this.OrganEntity = original.OrganEntity;
    this.Source = original.Source;
    this.Quantity = original.Quantity;
    this.Reagent = original.Reagent;
    this.Method = original.Method;
    this.Scale = original.Scale;
  }
}
