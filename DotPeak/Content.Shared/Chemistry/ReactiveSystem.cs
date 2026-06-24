// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.ReactiveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

#nullable enable
namespace Content.Shared.Chemistry;

public sealed class ReactiveSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private RMCReagentSystem _reagents;
  [Dependency]
  private IRobustRandom _robustRandom;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;

  public void DoEntityReaction(EntityUid uid, Solution solution, ReactionMethod method)
  {
    foreach (ReagentQuantity reagentQuantity in solution.Contents.ToArray())
      this.ReactionEntity(uid, method, reagentQuantity, solution);
  }

  public void ReactionEntity(
    EntityUid uid,
    ReactionMethod method,
    ReagentQuantity reagentQuantity,
    Solution? source)
  {
    Content.Shared._RMC14.Chemistry.Reagent.Reagent proto = this._reagents.Index(ProtoId<ReagentPrototype>.op_Implicit(reagentQuantity.Reagent.Prototype));
    this.ReactionEntity(uid, method, (ReagentPrototype) proto, reagentQuantity, source);
  }

  public void ReactionEntity(
    EntityUid uid,
    ReactionMethod method,
    ReagentPrototype proto,
    ReagentQuantity reagentQuantity,
    Solution? source)
  {
    ReactiveComponent reactiveComponent;
    if (!this.TryComp<ReactiveComponent>(uid, ref reactiveComponent))
      return;
    ReactionEntityEvent reactionEntityEvent = new ReactionEntityEvent(method, proto, reagentQuantity, source);
    this.RaiseLocalEvent<ReactionEntityEvent>(uid, ref reactionEntityEvent, false);
    EntityEffectReagentArgs args = new EntityEffectReagentArgs(uid, (IEntityManager) this.EntityManager, new EntityUid?(), source, source != null ? source.GetReagentQuantity(reagentQuantity.Reagent) : reagentQuantity.Quantity, proto, new ReactionMethod?(method), (FixedPoint2) 1f);
    if (proto.ReactiveEffects != null && reactiveComponent.ReactiveGroups != null)
    {
      foreach ((ProtoId<ReactiveGroupPrototype> key, Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry reagentEffectEntry) in proto.ReactiveEffects)
      {
        if (reagentEffectEntry.Methods.Contains(method) && reactiveComponent.ReactiveGroups.ContainsKey(ProtoId<ReactiveGroupPrototype>.op_Implicit(key)) && reactiveComponent.ReactiveGroups[ProtoId<ReactiveGroupPrototype>.op_Implicit(key)].Contains(method))
        {
          foreach (EntityEffect effect in reagentEffectEntry.Effects)
          {
            if (effect.ShouldApply((EntityEffectBaseArgs) args, this._robustRandom))
            {
              if (effect.ShouldLog)
              {
                EntityUid targetEntity = args.TargetEntity;
                ISharedAdminLogManager adminLogger = this._adminLogger;
                int logImpact = (int) effect.LogImpact;
                LogStringHandler logStringHandler = new LogStringHandler(64 /*0x40*/, 5);
                logStringHandler.AppendLiteral("Reactive effect ");
                logStringHandler.AppendFormatted(effect.GetType().Name, format: "effect");
                logStringHandler.AppendLiteral(" of reagent ");
                logStringHandler.AppendFormatted(proto.ID, format: "reagent");
                logStringHandler.AppendLiteral(" with method ");
                logStringHandler.AppendFormatted<ReactionMethod>(method, nameof (method));
                logStringHandler.AppendLiteral(" applied on entity ");
                logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(targetEntity)), "entity", "ToPrettyString(entity)");
                logStringHandler.AppendLiteral(" at ");
                logStringHandler.AppendFormatted<EntityCoordinates>(this.Transform(targetEntity).Coordinates, "coordinates", "Transform(entity).Coordinates");
                ref LogStringHandler local = ref logStringHandler;
                adminLogger.Add(LogType.ReagentEffect, (LogImpact) logImpact, ref local);
              }
              effect.Effect((EntityEffectBaseArgs) args);
            }
          }
        }
      }
    }
    if (reactiveComponent.Reactions == null)
      return;
    foreach (Content.Shared.Chemistry.Reaction.ReactiveReagentEffectEntry reaction in reactiveComponent.Reactions)
    {
      if (reaction.Methods.Contains(method) && (reaction.Reagents == null || reaction.Reagents.Contains(proto.ID)))
      {
        foreach (EntityEffect effect in reaction.Effects)
        {
          if (effect.ShouldApply((EntityEffectBaseArgs) args, this._robustRandom))
          {
            if (effect.ShouldLog)
            {
              EntityUid targetEntity = args.TargetEntity;
              ISharedAdminLogManager adminLogger = this._adminLogger;
              int logImpact = (int) effect.LogImpact;
              LogStringHandler logStringHandler = new LogStringHandler(52, 5);
              logStringHandler.AppendLiteral("Reactive effect ");
              logStringHandler.AppendFormatted(effect.GetType().Name, format: "effect");
              logStringHandler.AppendLiteral(" of ");
              logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(targetEntity)), "entity", "ToPrettyString(entity)");
              logStringHandler.AppendLiteral(" using reagent ");
              logStringHandler.AppendFormatted(proto.ID, format: "reagent");
              logStringHandler.AppendLiteral(" with method ");
              logStringHandler.AppendFormatted<ReactionMethod>(method, nameof (method));
              logStringHandler.AppendLiteral(" at ");
              logStringHandler.AppendFormatted<EntityCoordinates>(this.Transform(targetEntity).Coordinates, "coordinates", "Transform(entity).Coordinates");
              ref LogStringHandler local = ref logStringHandler;
              adminLogger.Add(LogType.ReagentEffect, (LogImpact) logImpact, ref local);
            }
            effect.Effect((EntityEffectBaseArgs) args);
          }
        }
      }
    }
  }
}
