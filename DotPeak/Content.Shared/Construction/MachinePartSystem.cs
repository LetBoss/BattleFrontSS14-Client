// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.MachinePartSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.Components;
using Content.Shared.Examine;
using Content.Shared.Lathe;
using Content.Shared.Materials;
using Content.Shared.Research.Prototypes;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Construction;

public sealed class MachinePartSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SharedLatheSystem _lathe;
  [Dependency]
  private SharedConstructionSystem _construction;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MachineBoardComponent, ExaminedEvent>(new ComponentEventHandler<MachineBoardComponent, ExaminedEvent>((object) this, __methodptr(OnMachineBoardExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnMachineBoardExamined(
    EntityUid uid,
    MachineBoardComponent component,
    ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    using (args.PushGroup("MachineBoardComponent"))
    {
      args.PushMarkup(this.Loc.GetString("machine-board-component-on-examine-label"));
      foreach ((ProtoId<StackPrototype> key, int num) in component.StackRequirements)
      {
        string name = this._prototype.Index(this._prototype.Index<StackPrototype>(key).Spawn).Name;
        args.PushMarkup(this.Loc.GetString("machine-board-component-required-element-entry-text", ("amount", (object) num), ("requiredElement", (object) this.Loc.GetString(name))));
      }
      foreach ((string _, GenericPartInfo genericPartInfo) in component.ComponentRequirements)
      {
        GenericPartInfo info = genericPartInfo;
        string examineName = this._construction.GetExamineName(info);
        args.PushMarkup(this.Loc.GetString("machine-board-component-required-element-entry-text", ("amount", (object) info.Amount), ("requiredElement", (object) examineName)));
      }
      foreach ((_, genericPartInfo) in component.TagRequirements)
      {
        GenericPartInfo info = genericPartInfo;
        string examineName = this._construction.GetExamineName(info);
        args.PushMarkup(this.Loc.GetString("machine-board-component-required-element-entry-text", ("amount", (object) info.Amount), ("requiredElement", (object) examineName)));
      }
    }
  }

  public Dictionary<string, int> GetMachineBoardMaterialCost(
    Entity<MachineBoardComponent> entity,
    int coefficient = 1)
  {
    EntityUid entityUid;
    MachineBoardComponent machineBoardComponent1;
    entity.Deconstruct(ref entityUid, ref machineBoardComponent1);
    MachineBoardComponent machineBoardComponent2 = machineBoardComponent1;
    Dictionary<string, int> boardMaterialCost = new Dictionary<string, int>();
    string key8;
    ProtoId<MaterialPrototype> key7;
    foreach ((ProtoId<StackPrototype> key3, int num9) in machineBoardComponent2.StackRequirements)
    {
      int num2 = num9;
      StackPrototype stackPrototype = this._prototype.Index<StackPrototype>(key3);
      PhysicalCompositionComponent compositionComponent;
      if (this._prototype.Index(stackPrototype.Spawn).TryGetComponent<PhysicalCompositionComponent>(ref compositionComponent, this.EntityManager.ComponentFactory))
      {
        foreach ((key8, num9) in compositionComponent.MaterialComposition)
        {
          string key5 = key8;
          int num4 = num9;
          boardMaterialCost.TryAdd(key5, 0);
          Dictionary<string, int> dictionary = boardMaterialCost;
          key8 = key5;
          dictionary[key8] += num4 * num2 * coefficient;
        }
      }
      else
      {
        List<LatheRecipePrototype> recipes;
        if (this._lathe.TryGetRecipesFromEntity(EntProtoId.op_Implicit(stackPrototype.Spawn), out recipes))
        {
          LatheRecipePrototype latheRecipePrototype = recipes[0];
          if (recipes.Count > 1)
            latheRecipePrototype = recipes.MinBy<LatheRecipePrototype, int>((Func<LatheRecipePrototype, int>) (p => p.Materials.Values.Sum()));
          foreach ((key7, num9) in latheRecipePrototype.Materials)
          {
            ProtoId<MaterialPrototype> protoId = key7;
            int num6 = num9;
            boardMaterialCost.TryAdd(ProtoId<MaterialPrototype>.op_Implicit(protoId), 0);
            Dictionary<string, int> dictionary = boardMaterialCost;
            key8 = ProtoId<MaterialPrototype>.op_Implicit(protoId);
            dictionary[key8] += num6 * num2 * coefficient;
          }
        }
      }
    }
    foreach (GenericPartInfo genericPartInfo in machineBoardComponent2.ComponentRequirements.Values.Concat<GenericPartInfo>((IEnumerable<GenericPartInfo>) machineBoardComponent2.ComponentRequirements.Values))
    {
      int amount = genericPartInfo.Amount;
      EntProtoId defaultPrototype = genericPartInfo.DefaultPrototype;
      List<LatheRecipePrototype> recipes;
      if (this._lathe.TryGetRecipesFromEntity(EntProtoId.op_Implicit(defaultPrototype), out recipes))
      {
        LatheRecipePrototype latheRecipePrototype = recipes[0];
        if (recipes.Count > 1)
          latheRecipePrototype = recipes.MinBy<LatheRecipePrototype, int>((Func<LatheRecipePrototype, int>) (p => p.Materials.Values.Sum()));
        foreach ((key7, num9) in latheRecipePrototype.Materials)
        {
          ProtoId<MaterialPrototype> protoId = key7;
          int num8 = num9;
          boardMaterialCost.TryAdd(ProtoId<MaterialPrototype>.op_Implicit(protoId), 0);
          Dictionary<string, int> dictionary = boardMaterialCost;
          key8 = ProtoId<MaterialPrototype>.op_Implicit(protoId);
          dictionary[key8] += num8 * amount * coefficient;
        }
      }
      else
      {
        EntityPrototype entityPrototype;
        PhysicalCompositionComponent compositionComponent;
        if (this._prototype.TryIndex(defaultPrototype, ref entityPrototype) && entityPrototype.TryGetComponent<PhysicalCompositionComponent>(ref compositionComponent, this.EntityManager.ComponentFactory))
        {
          foreach ((key8, num9) in compositionComponent.MaterialComposition)
          {
            string key9 = key8;
            int num10 = num9;
            boardMaterialCost.TryAdd(key9, 0);
            Dictionary<string, int> dictionary = boardMaterialCost;
            key8 = key9;
            dictionary[key8] += num10 * amount * coefficient;
          }
        }
      }
    }
    return boardMaterialCost;
  }
}
