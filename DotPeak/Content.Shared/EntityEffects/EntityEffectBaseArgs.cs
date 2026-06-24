// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EntityEffectBaseArgs
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Content.Shared.EntityEffects;

public record EntityEffectBaseArgs
{
  public EntityUid TargetEntity;
  public IEntityManager EntityManager;

  public EntityEffectBaseArgs(EntityUid targetEntity, IEntityManager entityManager)
  {
    this.TargetEntity = targetEntity;
    this.EntityManager = entityManager;
  }

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("TargetEntity = ");
    builder.Append(this.TargetEntity.ToString());
    builder.Append(", EntityManager = ");
    builder.Append((object) this.EntityManager);
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.TargetEntity)) * -1521134295 + EqualityComparer<IEntityManager>.Default.GetHashCode(this.EntityManager);
  }

  [CompilerGenerated]
  public virtual bool Equals(EntityEffectBaseArgs? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<EntityUid>.Default.Equals(this.TargetEntity, other.TargetEntity) && EqualityComparer<IEntityManager>.Default.Equals(this.EntityManager, other.EntityManager);
  }

  [CompilerGenerated]
  protected EntityEffectBaseArgs(EntityEffectBaseArgs original)
  {
    this.TargetEntity = original.TargetEntity;
    this.EntityManager = original.EntityManager;
  }
}
