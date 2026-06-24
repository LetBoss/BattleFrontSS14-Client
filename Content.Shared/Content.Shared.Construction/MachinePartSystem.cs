using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Construction.Components;
using Content.Shared.Examine;
using Content.Shared.Lathe;
using Content.Shared.Materials;
using Content.Shared.Research.Prototypes;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Construction;

public sealed class MachinePartSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SharedLatheSystem _lathe;

	[Dependency]
	private SharedConstructionSystem _construction;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MachineBoardComponent, ExaminedEvent>((ComponentEventHandler<MachineBoardComponent, ExaminedEvent>)OnMachineBoardExamined, (Type[])null, (Type[])null);
	}

	private void OnMachineBoardExamined(EntityUid uid, MachineBoardComponent component, ExaminedEvent args)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsInDetailsRange)
		{
			return;
		}
		using (args.PushGroup("MachineBoardComponent"))
		{
			args.PushMarkup(base.Loc.GetString("machine-board-component-on-examine-label"));
			foreach (KeyValuePair<ProtoId<StackPrototype>, int> stackRequirement in component.StackRequirements)
			{
				stackRequirement.Deconstruct(out var key, out var value);
				ProtoId<StackPrototype> material = key;
				int amount = value;
				StackPrototype stack = _prototype.Index<StackPrototype>(material);
				string name = _prototype.Index(stack.Spawn).Name;
				args.PushMarkup(base.Loc.GetString("machine-board-component-required-element-entry-text", (ValueTuple<string, object>)("amount", amount), (ValueTuple<string, object>)("requiredElement", base.Loc.GetString(name))));
			}
			GenericPartInfo value2;
			foreach (KeyValuePair<string, GenericPartInfo> componentRequirement in component.ComponentRequirements)
			{
				componentRequirement.Deconstruct(out var _, out value2);
				GenericPartInfo info = value2;
				string examineName = _construction.GetExamineName(info);
				args.PushMarkup(base.Loc.GetString("machine-board-component-required-element-entry-text", (ValueTuple<string, object>)("amount", info.Amount), (ValueTuple<string, object>)("requiredElement", examineName)));
			}
			foreach (KeyValuePair<ProtoId<TagPrototype>, GenericPartInfo> tagRequirement in component.TagRequirements)
			{
				tagRequirement.Deconstruct(out var _, out value2);
				GenericPartInfo info2 = value2;
				string examineName2 = _construction.GetExamineName(info2);
				args.PushMarkup(base.Loc.GetString("machine-board-component-required-element-entry-text", (ValueTuple<string, object>)("amount", info2.Amount), (ValueTuple<string, object>)("requiredElement", examineName2)));
			}
		}
	}

	public Dictionary<string, int> GetMachineBoardMaterialCost(Entity<MachineBoardComponent> entity, int coefficient = 1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		Entity<MachineBoardComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		MachineBoardComponent machineBoardComponent = default(MachineBoardComponent);
		val.Deconstruct(ref val2, ref machineBoardComponent);
		MachineBoardComponent comp = machineBoardComponent;
		Dictionary<string, int> materials = new Dictionary<string, int>();
		int value;
		PhysicalCompositionComponent physComp = default(PhysicalCompositionComponent);
		string key2;
		ProtoId<MaterialPrototype> key3;
		foreach (KeyValuePair<ProtoId<StackPrototype>, int> stackRequirement in comp.StackRequirements)
		{
			stackRequirement.Deconstruct(out var key, out value);
			ProtoId<StackPrototype> stackId = key;
			int amount = value;
			StackPrototype stackProto = _prototype.Index<StackPrototype>(stackId);
			if (_prototype.Index(stackProto.Spawn).TryGetComponent<PhysicalCompositionComponent>(ref physComp, base.EntityManager.ComponentFactory))
			{
				foreach (KeyValuePair<string, int> item in physComp.MaterialComposition)
				{
					item.Deconstruct(out key2, out value);
					string mat = key2;
					int matAmount = value;
					materials.TryAdd(mat, 0);
					Dictionary<string, int> dictionary = materials;
					key2 = mat;
					dictionary[key2] += matAmount * amount * coefficient;
				}
			}
			else
			{
				if (!_lathe.TryGetRecipesFromEntity(EntProtoId.op_Implicit(stackProto.Spawn), out List<LatheRecipePrototype> recipes))
				{
					continue;
				}
				LatheRecipePrototype partRecipe = recipes[0];
				if (recipes.Count > 1)
				{
					partRecipe = recipes.MinBy((LatheRecipePrototype p) => p.Materials.Values.Sum());
				}
				foreach (KeyValuePair<ProtoId<MaterialPrototype>, int> material in partRecipe.Materials)
				{
					material.Deconstruct(out key3, out value);
					ProtoId<MaterialPrototype> mat2 = key3;
					int matAmount2 = value;
					materials.TryAdd(ProtoId<MaterialPrototype>.op_Implicit(mat2), 0);
					Dictionary<string, int> dictionary = materials;
					key2 = ProtoId<MaterialPrototype>.op_Implicit(mat2);
					dictionary[key2] += matAmount2 * amount * coefficient;
				}
			}
		}
		EntityPrototype defaultProto = default(EntityPrototype);
		PhysicalCompositionComponent physComp2 = default(PhysicalCompositionComponent);
		foreach (GenericPartInfo item2 in comp.ComponentRequirements.Values.Concat(comp.ComponentRequirements.Values))
		{
			int amount2 = item2.Amount;
			EntProtoId defaultProtoId = item2.DefaultPrototype;
			if (_lathe.TryGetRecipesFromEntity(EntProtoId.op_Implicit(defaultProtoId), out List<LatheRecipePrototype> recipes2))
			{
				LatheRecipePrototype partRecipe2 = recipes2[0];
				if (recipes2.Count > 1)
				{
					partRecipe2 = recipes2.MinBy((LatheRecipePrototype p) => p.Materials.Values.Sum());
				}
				foreach (KeyValuePair<ProtoId<MaterialPrototype>, int> material2 in partRecipe2.Materials)
				{
					material2.Deconstruct(out key3, out value);
					ProtoId<MaterialPrototype> mat3 = key3;
					int matAmount3 = value;
					materials.TryAdd(ProtoId<MaterialPrototype>.op_Implicit(mat3), 0);
					Dictionary<string, int> dictionary = materials;
					key2 = ProtoId<MaterialPrototype>.op_Implicit(mat3);
					dictionary[key2] += matAmount3 * amount2 * coefficient;
				}
			}
			else
			{
				if (!_prototype.TryIndex(defaultProtoId, ref defaultProto) || !defaultProto.TryGetComponent<PhysicalCompositionComponent>(ref physComp2, base.EntityManager.ComponentFactory))
				{
					continue;
				}
				foreach (KeyValuePair<string, int> item3 in physComp2.MaterialComposition)
				{
					item3.Deconstruct(out key2, out value);
					string mat4 = key2;
					int matAmount4 = value;
					materials.TryAdd(mat4, 0);
					Dictionary<string, int> dictionary = materials;
					key2 = mat4;
					dictionary[key2] += matAmount4 * amount2 * coefficient;
				}
			}
		}
		return materials;
	}
}
