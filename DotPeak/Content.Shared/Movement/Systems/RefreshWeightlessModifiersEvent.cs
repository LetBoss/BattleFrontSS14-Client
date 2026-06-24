// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.RefreshWeightlessModifiersEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Movement.Systems;

[ByRefEvent]
public record struct RefreshWeightlessModifiersEvent
{
  public float WeightlessAcceleration;
  public float WeightlessAccelerationMod;
  public float WeightlessModifier;
  public float WeightlessFriction;
  public float WeightlessFrictionMod;
  public float WeightlessFrictionNoInput;
  public float WeightlessFrictionNoInputMod;

  public void ModifyFriction(float friction, float noInput)
  {
    this.WeightlessFrictionMod *= friction;
    this.WeightlessFrictionNoInput *= noInput;
  }

  public void ModifyFriction(float friction) => this.ModifyFriction(friction, friction);

  public void ModifyAcceleration(float acceleration, float modifier)
  {
    this.WeightlessAcceleration *= acceleration;
    this.WeightlessModifier *= modifier;
  }

  public void ModifyAcceleration(float modifier) => this.ModifyAcceleration(modifier, modifier);

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (((((EqualityComparer<float>.Default.GetHashCode(this.WeightlessAcceleration) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.WeightlessAccelerationMod)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.WeightlessModifier)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.WeightlessFriction)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.WeightlessFrictionMod)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.WeightlessFrictionNoInput)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.WeightlessFrictionNoInputMod);
  }

  [CompilerGenerated]
  public readonly bool Equals(RefreshWeightlessModifiersEvent other)
  {
    return EqualityComparer<float>.Default.Equals(this.WeightlessAcceleration, other.WeightlessAcceleration) && EqualityComparer<float>.Default.Equals(this.WeightlessAccelerationMod, other.WeightlessAccelerationMod) && EqualityComparer<float>.Default.Equals(this.WeightlessModifier, other.WeightlessModifier) && EqualityComparer<float>.Default.Equals(this.WeightlessFriction, other.WeightlessFriction) && EqualityComparer<float>.Default.Equals(this.WeightlessFrictionMod, other.WeightlessFrictionMod) && EqualityComparer<float>.Default.Equals(this.WeightlessFrictionNoInput, other.WeightlessFrictionNoInput) && EqualityComparer<float>.Default.Equals(this.WeightlessFrictionNoInputMod, other.WeightlessFrictionNoInputMod);
  }
}
