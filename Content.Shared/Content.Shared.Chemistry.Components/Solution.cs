using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Components;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class Solution : IEnumerable<ReagentQuantity>, IEnumerable, ISerializationHooks, IRobustCloneable<Solution>, ISerializationGenerated<Solution>, ISerializationGenerated
{
	[DataField("reagents", false, 1, false, false, null)]
	public List<ReagentQuantity> Contents;

	[DataField(null, false, 1, false, false, null)]
	public string? Name;

	[ViewVariables]
	private float _heatCapacity;

	[ViewVariables]
	private bool _heatCapacityDirty = true;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	private int _heatCapacityUpdateCounter;

	private const int HeatCapacityUpdateInterval = 15;

	[ViewVariables]
	public FixedPoint2 Volume { get; set; }

	[DataField("maxVol", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public FixedPoint2 MaxVolume { get; set; } = FixedPoint2.Zero;

	public float FillFraction
	{
		get
		{
			if (!(MaxVolume == 0))
			{
				return Volume.Float() / MaxVolume.Float();
			}
			return 1f;
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("canReact", false, 1, false, false, null)]
	public bool CanReact { get; set; } = true;

	[ViewVariables]
	public FixedPoint2 AvailableVolume => MaxVolume - Volume;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("temperature", false, 1, false, false, null)]
	public float Temperature { get; set; } = 293.15f;

	public ReagentQuantity this[ReagentId id]
	{
		get
		{
			if (!TryGetReagent(id, out var quantity))
			{
				throw new KeyNotFoundException(id.ToString());
			}
			return quantity;
		}
	}

	public bool CanAddSolution(Solution solution)
	{
		return solution.Volume <= AvailableVolume;
	}

	public void UpdateHeatCapacity(IPrototypeManager? protoMan)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IPrototypeManager>(ref protoMan);
		_heatCapacityDirty = false;
		_heatCapacity = 0f;
		if (Contents.Count > 0)
		{
			RMCReagentSystem reagentSys = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
			foreach (var (reagent, quantity) in Contents)
			{
				_heatCapacity += (float)quantity * reagentSys.Index(ProtoId<ReagentPrototype>.op_Implicit(reagent.Prototype)).SpecificHeat;
			}
		}
		_heatCapacityUpdateCounter = 0;
	}

	public float GetHeatCapacity(IPrototypeManager? protoMan)
	{
		if (_heatCapacityDirty)
		{
			UpdateHeatCapacity(protoMan);
		}
		return _heatCapacity;
	}

	public void CheckRecalculateHeatCapacity()
	{
		if (++_heatCapacityUpdateCounter >= 15)
		{
			_heatCapacityDirty = true;
		}
	}

	public float GetThermalEnergy(IPrototypeManager? protoMan)
	{
		return GetHeatCapacity(protoMan) * Temperature;
	}

	public Solution()
		: this(2)
	{
	}

	public Solution(int capacity)
	{
		Contents = new List<ReagentQuantity>(capacity);
	}

	public Solution(string prototype, FixedPoint2 quantity, List<ReagentData>? data = null)
		: this()
	{
		AddReagent(new ReagentId(prototype, data), quantity);
	}

	public Solution(IEnumerable<ReagentQuantity> reagents, bool setMaxVol = true)
	{
		Contents = new List<ReagentQuantity>(reagents);
		Volume = FixedPoint2.Zero;
		foreach (ReagentQuantity reagent in Contents)
		{
			Volume += reagent.Quantity;
		}
		if (setMaxVol)
		{
			MaxVolume = Volume;
		}
		ValidateSolution();
	}

	public Solution(Solution solution, IPrototypeManager? prototypes = null)
	{
		Contents = Extensions.ShallowClone<ReagentQuantity>(solution.Contents);
		Volume = solution.Volume;
		MaxVolume = solution.MaxVolume;
		Temperature = solution.Temperature;
		CanReact = solution.CanReact;
		_heatCapacity = solution._heatCapacity;
		_heatCapacityDirty = solution._heatCapacityDirty;
		_heatCapacityUpdateCounter = solution._heatCapacityUpdateCounter;
		ValidateSolution(prototypes);
	}

	public Solution Clone()
	{
		return new Solution(this);
	}

	public void ValidateSolution(IPrototypeManager? prototypes = null)
	{
	}

	void ISerializationHooks.AfterDeserialization()
	{
		Volume = FixedPoint2.Zero;
		foreach (ReagentQuantity reagent in Contents)
		{
			Volume += reagent.Quantity;
		}
		if (MaxVolume == FixedPoint2.Zero)
		{
			MaxVolume = Volume;
		}
	}

	public bool ContainsPrototype(string prototype)
	{
		foreach (var (reagent, _) in Contents)
		{
			if (reagent.Prototype == prototype)
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsReagent(ReagentId id)
	{
		foreach (ReagentQuantity content in Contents)
		{
			content.Deconstruct(out var id2, out var _);
			if (id2 == id)
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsReagent(string reagentId, List<ReagentData>? data)
	{
		return ContainsReagent(new ReagentId(reagentId, data));
	}

	public bool TryGetReagent(ReagentId id, out ReagentQuantity quantity)
	{
		foreach (ReagentQuantity tuple in Contents)
		{
			if (!(tuple.Reagent != id))
			{
				quantity = tuple;
				return true;
			}
		}
		quantity = new ReagentQuantity(id, FixedPoint2.Zero);
		return false;
	}

	public bool TryGetReagentQuantity(ReagentId id, out FixedPoint2 volume)
	{
		volume = FixedPoint2.Zero;
		if (!TryGetReagent(id, out var quant))
		{
			return false;
		}
		volume = quant.Quantity;
		return true;
	}

	public ReagentQuantity GetReagent(ReagentId id)
	{
		TryGetReagent(id, out var quantity);
		return quantity;
	}

	public FixedPoint2 GetReagentQuantity(ReagentId id)
	{
		return GetReagent(id).Quantity;
	}

	public FixedPoint2 GetTotalPrototypeQuantity(params string[] prototypes)
	{
		FixedPoint2 total = FixedPoint2.Zero;
		foreach (var (reagent, quantity) in Contents)
		{
			if (Enumerable.Contains(prototypes, reagent.Prototype))
			{
				total += quantity;
			}
		}
		return total;
	}

	public FixedPoint2 GetTotalPrototypeQuantity(string id)
	{
		FixedPoint2 total = FixedPoint2.Zero;
		foreach (var (reagent, quantity) in Contents)
		{
			if (id == reagent.Prototype)
			{
				total += quantity;
			}
		}
		return total;
	}

	public ReagentId? GetPrimaryReagentId()
	{
		if (Contents.Count == 0)
		{
			return null;
		}
		ReagentQuantity max = default(ReagentQuantity);
		foreach (ReagentQuantity reagent in Contents)
		{
			if (reagent.Quantity >= max.Quantity)
			{
				max = reagent;
			}
		}
		return max.Reagent;
	}

	public void AddReagent(string prototype, FixedPoint2 quantity, bool dirtyHeatCap = true)
	{
		AddReagent(new ReagentId(prototype, null), quantity, dirtyHeatCap);
	}

	public void AddReagent(ReagentId id, FixedPoint2 quantity, bool dirtyHeatCap = true)
	{
		if (quantity <= 0)
		{
			return;
		}
		Volume += quantity;
		_heatCapacityDirty |= dirtyHeatCap;
		for (int i = 0; i < Contents.Count; i++)
		{
			var (reagentId2, existingQuantity) = (ReagentQuantity)(ref Contents[i]);
			if (!(reagentId2 != id))
			{
				Contents[i] = new ReagentQuantity(id, existingQuantity + quantity);
				ValidateSolution();
				return;
			}
		}
		Contents.Add(new ReagentQuantity(id, quantity));
		ValidateSolution();
	}

	public void AddReagent(ReagentPrototype proto, ReagentId reagentId, FixedPoint2 quantity)
	{
		AddReagent(reagentId, quantity, dirtyHeatCap: false);
		_heatCapacity += quantity.Float() * proto.SpecificHeat;
		CheckRecalculateHeatCapacity();
	}

	public void AddReagent(ReagentQuantity reagentQuantity)
	{
		AddReagent(reagentQuantity.Reagent, reagentQuantity.Quantity);
	}

	public void AddReagent(ReagentPrototype proto, FixedPoint2 quantity, float temperature, IPrototypeManager? protoMan, List<ReagentData>? data = null)
	{
		if (_heatCapacityDirty)
		{
			UpdateHeatCapacity(protoMan);
		}
		float totalThermalEnergy = Temperature * _heatCapacity + temperature * proto.SpecificHeat;
		AddReagent(new ReagentId(proto.ID, data), quantity);
		Temperature = ((_heatCapacity == 0f) ? 0f : (totalThermalEnergy / _heatCapacity));
	}

	public void ScaleSolution(int scale)
	{
		if (scale == 1)
		{
			return;
		}
		if (scale <= 0)
		{
			RemoveAllSolution();
			return;
		}
		_heatCapacity *= scale;
		Volume *= scale;
		CheckRecalculateHeatCapacity();
		for (int i = 0; i < Contents.Count; i++)
		{
			ReagentQuantity old = Contents[i];
			Contents[i] = new ReagentQuantity(old.Reagent, old.Quantity * scale);
		}
		ValidateSolution();
	}

	public void ScaleSolution(float scale)
	{
		if (scale == 1f)
		{
			return;
		}
		if (scale == 0f)
		{
			RemoveAllSolution();
			return;
		}
		Volume = FixedPoint2.Zero;
		for (int i = Contents.Count - 1; i >= 0; i--)
		{
			ReagentQuantity old = Contents[i];
			FixedPoint2 newQuantity = old.Quantity * scale;
			if (newQuantity == FixedPoint2.Zero)
			{
				Extensions.RemoveSwap<ReagentQuantity>((IList<ReagentQuantity>)Contents, i);
			}
			else
			{
				Contents[i] = new ReagentQuantity(old.Reagent, newQuantity);
				Volume += newQuantity;
			}
		}
		_heatCapacityDirty = true;
		ValidateSolution();
	}

	public FixedPoint2 RemoveReagent(ReagentQuantity toRemove, bool preserveOrder = false, bool ignoreReagentData = false)
	{
		if (toRemove.Quantity <= FixedPoint2.Zero)
		{
			return FixedPoint2.Zero;
		}
		List<int> reagentIndices = new List<int>();
		int totalRemoveVolume = 0;
		ReagentId id;
		FixedPoint2 quantity;
		for (int i = 0; i < Contents.Count; i++)
		{
			Contents[i].Deconstruct(out id, out quantity);
			ReagentId reagent = id;
			FixedPoint2 quantity2 = quantity;
			if (ignoreReagentData)
			{
				string prototype = reagent.Prototype;
				id = toRemove.Reagent;
				if (prototype != id.Prototype)
				{
					continue;
				}
			}
			else if (reagent != toRemove.Reagent)
			{
				continue;
			}
			totalRemoveVolume += quantity2.Value;
			reagentIndices.Insert(0, i);
		}
		if (totalRemoveVolume <= 0)
		{
			return FixedPoint2.Zero;
		}
		FixedPoint2 removedQuantity = 0;
		for (int j = 0; j < reagentIndices.Count; j++)
		{
			Contents[reagentIndices[j]].Deconstruct(out id, out quantity);
			ReagentId reagent2 = id;
			FixedPoint2 curQuantity = quantity;
			quantity = toRemove.Quantity;
			FixedPoint2 splitQuantity = FixedPoint2.FromCents((int)((long)quantity.Value * (long)curQuantity.Value / totalRemoveVolume));
			FixedPoint2 newQuantity = curQuantity - splitQuantity;
			_heatCapacityDirty = true;
			if (newQuantity <= 0)
			{
				if (!preserveOrder)
				{
					Extensions.RemoveSwap<ReagentQuantity>((IList<ReagentQuantity>)Contents, reagentIndices[j]);
				}
				else
				{
					Contents.RemoveAt(reagentIndices[j]);
				}
				Volume -= curQuantity;
				removedQuantity += curQuantity;
			}
			else
			{
				Contents[reagentIndices[j]] = new ReagentQuantity(reagent2, newQuantity);
				Volume -= splitQuantity;
				removedQuantity += splitQuantity;
			}
		}
		ValidateSolution();
		return removedQuantity;
	}

	public FixedPoint2 RemoveReagent(string prototype, FixedPoint2 quantity, List<ReagentData>? data = null, bool ignoreReagentData = false)
	{
		return RemoveReagent(new ReagentQuantity(prototype, quantity, data), preserveOrder: false, ignoreReagentData);
	}

	public FixedPoint2 RemoveReagent(ReagentId reagentId, FixedPoint2 quantity, bool preserveOrder = false, bool ignoreReagentData = false)
	{
		return RemoveReagent(new ReagentQuantity(reagentId, quantity), preserveOrder, ignoreReagentData);
	}

	public void RemoveAllSolution()
	{
		Contents.Clear();
		Volume = FixedPoint2.Zero;
		_heatCapacityDirty = false;
		_heatCapacity = 0f;
	}

	public Solution SplitSolutionWithout(FixedPoint2 toTake, params string[] excludedPrototypes)
	{
		List<ReagentQuantity> excluded = new List<ReagentQuantity>();
		foreach (string id in excludedPrototypes)
		{
			foreach (ReagentQuantity tuple in Contents)
			{
				if (!(tuple.Reagent.Prototype != id))
				{
					excluded.Add(tuple);
					RemoveReagent(tuple);
					break;
				}
			}
		}
		Solution sol = SplitSolution(toTake);
		foreach (ReagentQuantity reagent in excluded)
		{
			AddReagent(reagent);
		}
		return sol;
	}

	public Solution SplitSolutionWithOnly(FixedPoint2 toTake, params string[] includedPrototypes)
	{
		List<ReagentQuantity> excluded = new List<ReagentQuantity>();
		for (int i = Contents.Count - 1; i >= 0; i--)
		{
			if (!Enumerable.Contains(includedPrototypes, Contents[i].Reagent.Prototype))
			{
				excluded.Add(Contents[i]);
				RemoveReagent(Contents[i]);
			}
		}
		Solution sol = SplitSolution(toTake);
		foreach (ReagentQuantity reagent in excluded)
		{
			AddReagent(reagent);
		}
		return sol;
	}

	public Solution SplitSolution(FixedPoint2 toTake)
	{
		if (toTake <= FixedPoint2.Zero)
		{
			return new Solution();
		}
		Solution newSolution;
		if (toTake >= Volume)
		{
			newSolution = Clone();
			RemoveAllSolution();
			return newSolution;
		}
		FixedPoint2 origVol = Volume;
		FixedPoint2 quantity = Volume;
		int effVol = quantity.Value;
		newSolution = new Solution(Contents.Count)
		{
			Temperature = Temperature
		};
		long remaining = toTake.Value;
		for (int i = Contents.Count - 1; i >= 0; i--)
		{
			Contents[i].Deconstruct(out var id, out quantity);
			ReagentId reagent = id;
			FixedPoint2 quantity2 = quantity;
			long split = remaining * quantity2.Value / effVol;
			if (split <= 0)
			{
				effVol -= quantity2.Value;
			}
			else
			{
				FixedPoint2 splitQuantity = FixedPoint2.FromCents((int)split);
				FixedPoint2 newQuantity = quantity2 - splitQuantity;
				if (newQuantity > FixedPoint2.Zero)
				{
					Contents[i] = new ReagentQuantity(reagent, newQuantity);
				}
				else
				{
					Extensions.RemoveSwap<ReagentQuantity>((IList<ReagentQuantity>)Contents, i);
				}
				newSolution.Contents.Add(new ReagentQuantity(reagent, splitQuantity));
				Volume -= splitQuantity;
				remaining -= split;
				effVol -= quantity2.Value;
			}
		}
		newSolution.Volume = origVol - Volume;
		_heatCapacityDirty = true;
		newSolution._heatCapacityDirty = true;
		ValidateSolution();
		newSolution.ValidateSolution();
		return newSolution;
	}

	public void RemoveSolution(FixedPoint2 toTake)
	{
		if (toTake <= FixedPoint2.Zero)
		{
			return;
		}
		if (toTake >= Volume)
		{
			RemoveAllSolution();
			return;
		}
		FixedPoint2 quantity = Volume;
		int effVol = quantity.Value;
		Volume -= toTake;
		long remaining = toTake.Value;
		for (int i = Contents.Count - 1; i >= 0; i--)
		{
			Contents[i].Deconstruct(out var id, out quantity);
			ReagentId reagent = id;
			FixedPoint2 quantity2 = quantity;
			long split = remaining * quantity2.Value / effVol;
			if (split <= 0)
			{
				effVol -= quantity2.Value;
			}
			else
			{
				FixedPoint2 splitQuantity = FixedPoint2.FromCents((int)split);
				FixedPoint2 newQuantity = quantity2 - splitQuantity;
				if (newQuantity > FixedPoint2.Zero)
				{
					Contents[i] = new ReagentQuantity(reagent, newQuantity);
				}
				else
				{
					Extensions.RemoveSwap<ReagentQuantity>((IList<ReagentQuantity>)Contents, i);
				}
				remaining -= split;
				effVol -= quantity2.Value;
			}
		}
		_heatCapacityDirty = true;
		ValidateSolution();
	}

	public void AddSolution(Solution otherSolution, IPrototypeManager? protoMan)
	{
		if (otherSolution.Volume <= FixedPoint2.Zero)
		{
			return;
		}
		Volume += otherSolution.Volume;
		bool closeTemps = MathHelper.CloseTo(otherSolution.Temperature, Temperature, 1E-07f);
		float totalThermalEnergy = 0f;
		if (!closeTemps)
		{
			IoCManager.Resolve<IPrototypeManager>(ref protoMan);
			if (_heatCapacityDirty)
			{
				UpdateHeatCapacity(protoMan);
			}
			if (otherSolution._heatCapacityDirty)
			{
				otherSolution.UpdateHeatCapacity(protoMan);
			}
			totalThermalEnergy = _heatCapacity * Temperature + otherSolution._heatCapacity * otherSolution.Temperature;
		}
		for (int i = 0; i < otherSolution.Contents.Count; i++)
		{
			otherSolution.Contents[i].Deconstruct(out var id, out var quantity);
			ReagentId otherReagent = id;
			FixedPoint2 otherQuantity = quantity;
			bool found = false;
			for (int j = 0; j < Contents.Count; j++)
			{
				Contents[j].Deconstruct(out id, out quantity);
				ReagentId reagent = id;
				FixedPoint2 quantity2 = quantity;
				if (reagent == otherReagent)
				{
					found = true;
					Contents[j] = new ReagentQuantity(reagent, quantity2 + otherQuantity);
					break;
				}
			}
			if (!found)
			{
				Contents.Add(new ReagentQuantity(otherReagent, otherQuantity));
			}
		}
		_heatCapacity += otherSolution._heatCapacity;
		CheckRecalculateHeatCapacity();
		if (closeTemps)
		{
			_heatCapacityDirty |= otherSolution._heatCapacityDirty;
		}
		else
		{
			Temperature = ((_heatCapacity == 0f) ? 0f : (totalThermalEnergy / _heatCapacity));
		}
		ValidateSolution();
	}

	public Color GetColorWithout(IPrototypeManager? protoMan, params string[] without)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (Volume == FixedPoint2.Zero)
		{
			return Color.Transparent;
		}
		IoCManager.Resolve<IPrototypeManager>(ref protoMan);
		RMCReagentSystem reagentSys = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
		Color mixColor = default(Color);
		FixedPoint2 runningTotalQuantity = FixedPoint2.New(0);
		bool first = true;
		foreach (var (reagent, quantity) in Contents)
		{
			if (Enumerable.Contains(without, reagent.Prototype))
			{
				continue;
			}
			runningTotalQuantity += quantity;
			if (reagentSys.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(reagent.Prototype), out Content.Shared._RMC14.Chemistry.Reagent.Reagent proto))
			{
				if (first)
				{
					first = false;
					mixColor = proto.SubstanceColor;
				}
				else
				{
					float interpolateValue = quantity.Float() / runningTotalQuantity.Float();
					mixColor = Color.InterpolateBetween(mixColor, proto.SubstanceColor, interpolateValue);
				}
			}
		}
		return mixColor;
	}

	public Color GetColor(IPrototypeManager? protoMan)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetColorWithout(protoMan);
	}

	public Color GetColorWithOnly(IPrototypeManager? protoMan, params string[] included)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (Volume == FixedPoint2.Zero)
		{
			return Color.Transparent;
		}
		IoCManager.Resolve<IPrototypeManager>(ref protoMan);
		RMCReagentSystem reagentSys = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
		Color mixColor = default(Color);
		FixedPoint2 runningTotalQuantity = FixedPoint2.New(0);
		bool first = true;
		foreach (var (reagent, quantity) in Contents)
		{
			if (!Enumerable.Contains(included, reagent.Prototype))
			{
				continue;
			}
			runningTotalQuantity += quantity;
			if (reagentSys.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(reagent.Prototype), out Content.Shared._RMC14.Chemistry.Reagent.Reagent proto))
			{
				if (first)
				{
					first = false;
					mixColor = proto.SubstanceColor;
				}
				else
				{
					float interpolateValue = quantity.Float() / runningTotalQuantity.Float();
					mixColor = Color.InterpolateBetween(mixColor, proto.SubstanceColor, interpolateValue);
				}
			}
		}
		return mixColor;
	}

	public IEnumerator<ReagentQuantity> GetEnumerator()
	{
		return Contents.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void SetContents(IEnumerable<ReagentQuantity> reagents, bool setMaxVol = false)
	{
		Volume = 0;
		RemoveAllSolution();
		_heatCapacityDirty = true;
		Contents = new List<ReagentQuantity>(reagents);
		foreach (ReagentQuantity reagent in Contents)
		{
			Volume += reagent.Quantity;
		}
		if (setMaxVol)
		{
			MaxVolume = Volume;
		}
		ValidateSolution();
	}

	public Dictionary<ReagentPrototype, FixedPoint2> GetReagentPrototypes(IPrototypeManager protoMan)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ReagentPrototype, FixedPoint2> dict = new Dictionary<ReagentPrototype, FixedPoint2>(Contents.Count);
		RMCReagentSystem reagentSys = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
		foreach (ReagentQuantity content in Contents)
		{
			content.Deconstruct(out var id, out var quantity);
			ReagentId reagent = id;
			FixedPoint2 quantity2 = quantity;
			Content.Shared._RMC14.Chemistry.Reagent.Reagent proto = reagentSys.Index(ProtoId<ReagentPrototype>.op_Implicit(reagent.Prototype));
			dict[proto] = quantity2 + dict.GetValueOrDefault(proto);
		}
		return dict;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Solution target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<Solution>(this, ref target, hookCtx, true, context))
		{
			List<ReagentQuantity> ContentsTemp = null;
			if (Contents == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ReagentQuantity>>(Contents, ref ContentsTemp, hookCtx, true, context))
			{
				ContentsTemp = serialization.CreateCopy<List<ReagentQuantity>>(Contents, hookCtx, context, false);
			}
			target.Contents = ContentsTemp;
			FixedPoint2 MaxVolumeTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(MaxVolume, ref MaxVolumeTemp, hookCtx, false, context))
			{
				MaxVolumeTemp = serialization.CreateCopy<FixedPoint2>(MaxVolume, hookCtx, context, false);
			}
			target.MaxVolume = MaxVolumeTemp;
			bool CanReactTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanReact, ref CanReactTemp, hookCtx, false, context))
			{
				CanReactTemp = CanReact;
			}
			target.CanReact = CanReactTemp;
			float TemperatureTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Temperature, ref TemperatureTemp, hookCtx, false, context))
			{
				TemperatureTemp = Temperature;
			}
			target.Temperature = TemperatureTemp;
			string NameTemp = null;
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Solution target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Solution cast = (Solution)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public Solution Instantiate()
	{
		return new Solution();
	}
}
