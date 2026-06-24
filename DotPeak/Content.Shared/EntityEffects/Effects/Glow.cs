// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Glow
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class Glow : EntityEffect, ISerializationGenerated<Glow>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Radius = 2f;
  [DataField(null, false, 1, false, false, null)]
  public Color Color = Color.Black;
  private static readonly List<Color> Colors = new List<Color>()
  {
    Color.White,
    Color.Red,
    Color.Yellow,
    Color.Green,
    Color.Blue,
    Color.Purple,
    Color.Pink
  };

  public override void Effect(EntityEffectBaseArgs args)
  {
    if (Color.op_Equality(this.Color, Color.Black))
      this.Color = RandomExtensions.Pick<Color>(IoCManager.Resolve<IRobustRandom>(), (IReadOnlyList<Color>) Glow.Colors);
    SharedPointLightSystem pointLightSystem = args.EntityManager.System<SharedPointLightSystem>();
    SharedPointLightComponent comp = pointLightSystem.EnsureLight(args.TargetEntity);
    pointLightSystem.SetRadius(args.TargetEntity, this.Radius, comp);
    pointLightSystem.SetColor(args.TargetEntity, this.Color, comp);
    pointLightSystem.SetCastShadows(args.TargetEntity, false, comp);
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return "TODO";
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Glow target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Glow) target1;
    if (serialization.TryCustomCopy<Glow>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target2, hookCtx, false, context))
      target2 = this.Radius;
    target.Radius = target2;
    Color target3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Glow target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Glow target1 = (Glow) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Glow target1 = (Glow) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Glow EntityEffect.Instantiate() => new Glow();
}
