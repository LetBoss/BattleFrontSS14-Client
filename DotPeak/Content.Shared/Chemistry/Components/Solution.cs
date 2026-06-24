// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.Solution
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class Solution : 
  IEnumerable<ReagentQuantity>,
  IEnumerable,
  ISerializationHooks,
  IRobustCloneable<Solution>,
  ISerializationGenerated<Solution>,
  ISerializationGenerated
{
  [DataField("reagents", false, 1, false, false, null)]
  public List<ReagentQuantity> Contents;
  [DataField(null, false, 1, false, false, null)]
  public string? Name;
  [Robust.Shared.ViewVariables.ViewVariables]
  private float _heatCapacity;
  [Robust.Shared.ViewVariables.ViewVariables]
  private bool _heatCapacityDirty = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  private int _heatCapacityUpdateCounter;
  private const int HeatCapacityUpdateInterval = 15;

  [Robust.Shared.ViewVariables.ViewVariables]
  public FixedPoint2 Volume { get; set; }

  [DataField("maxVol", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public FixedPoint2 MaxVolume { get; set; } = FixedPoint2.Zero;

  public float FillFraction
  {
    get => !(this.MaxVolume == 0) ? this.Volume.Float() / this.MaxVolume.Float() : 1f;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("canReact", false, 1, false, false, null)]
  public bool CanReact { get; set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  public FixedPoint2 AvailableVolume => this.MaxVolume - this.Volume;

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("temperature", false, 1, false, false, null)]
  public float Temperature { get; set; } = 293.15f;

  public bool CanAddSolution(Solution solution) => solution.Volume <= this.AvailableVolume;

  public void UpdateHeatCapacity(IPrototypeManager? protoMan)
  {
    IoCManager.Resolve<IPrototypeManager>(ref protoMan);
    this._heatCapacityDirty = false;
    this._heatCapacity = 0.0f;
    if (this.Contents.Count > 0)
    {
      RMCReagentSystem rmcReagentSystem = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
      foreach ((ReagentId id, FixedPoint2 quantity) in this.Contents)
        this._heatCapacity += (float) quantity * rmcReagentSystem.Index(ProtoId<ReagentPrototype>.op_Implicit(id.Prototype)).SpecificHeat;
    }
    this._heatCapacityUpdateCounter = 0;
  }

  public float GetHeatCapacity(IPrototypeManager? protoMan)
  {
    if (this._heatCapacityDirty)
      this.UpdateHeatCapacity(protoMan);
    return this._heatCapacity;
  }

  public void CheckRecalculateHeatCapacity()
  {
    if (++this._heatCapacityUpdateCounter < 15)
      return;
    this._heatCapacityDirty = true;
  }

  public float GetThermalEnergy(IPrototypeManager? protoMan)
  {
    return this.GetHeatCapacity(protoMan) * this.Temperature;
  }

  public Solution()
    : this(2)
  {
  }

  public Solution(int capacity) => this.Contents = new List<ReagentQuantity>(capacity);

  public Solution(string prototype, FixedPoint2 quantity, List<ReagentData>? data = null)
    : this()
  {
    this.AddReagent(new ReagentId(prototype, data), quantity);
  }

  public Solution(IEnumerable<ReagentQuantity> reagents, bool setMaxVol = true)
  {
    this.Contents = new List<ReagentQuantity>(reagents);
    this.Volume = FixedPoint2.Zero;
    foreach (ReagentQuantity content in this.Contents)
      this.Volume += content.Quantity;
    if (setMaxVol)
      this.MaxVolume = this.Volume;
    this.ValidateSolution();
  }

  public Solution(Solution solution, IPrototypeManager? prototypes = null)
  {
    this.Contents = Extensions.ShallowClone<ReagentQuantity>(solution.Contents);
    this.Volume = solution.Volume;
    this.MaxVolume = solution.MaxVolume;
    this.Temperature = solution.Temperature;
    this.CanReact = solution.CanReact;
    this._heatCapacity = solution._heatCapacity;
    this._heatCapacityDirty = solution._heatCapacityDirty;
    this._heatCapacityUpdateCounter = solution._heatCapacityUpdateCounter;
    this.ValidateSolution(prototypes);
  }

  public Solution Clone() => new Solution(this);

  public void ValidateSolution(IPrototypeManager? prototypes = null)
  {
  }

  void ISerializationHooks.AfterDeserialization()
  {
    this.Volume = FixedPoint2.Zero;
    foreach (ReagentQuantity content in this.Contents)
      this.Volume += content.Quantity;
    if (!(this.MaxVolume == FixedPoint2.Zero))
      return;
    this.MaxVolume = this.Volume;
  }

  public bool ContainsPrototype(string prototype)
  {
    foreach ((ReagentId id, FixedPoint2 _) in this.Contents)
    {
      if (id.Prototype == prototype)
        return true;
    }
    return false;
  }

  public bool ContainsReagent(ReagentId id)
  {
    foreach ((ReagentId id1, FixedPoint2 _) in this.Contents)
    {
      if (id1 == id)
        return true;
    }
    return false;
  }

  public bool ContainsReagent(string reagentId, List<ReagentData>? data)
  {
    return this.ContainsReagent(new ReagentId(reagentId, data));
  }

  public bool TryGetReagent(ReagentId id, out ReagentQuantity quantity)
  {
    foreach (ReagentQuantity content in this.Contents)
    {
      if (!(content.Reagent != id))
      {
        quantity = content;
        return true;
      }
    }
    quantity = new ReagentQuantity(id, FixedPoint2.Zero);
    return false;
  }

  public bool TryGetReagentQuantity(ReagentId id, out FixedPoint2 volume)
  {
    volume = FixedPoint2.Zero;
    ReagentQuantity quantity;
    if (!this.TryGetReagent(id, out quantity))
      return false;
    volume = quantity.Quantity;
    return true;
  }

  public ReagentQuantity GetReagent(ReagentId id)
  {
    ReagentQuantity quantity;
    this.TryGetReagent(id, out quantity);
    return quantity;
  }

  public ReagentQuantity this[ReagentId id]
  {
    get
    {
      ReagentQuantity quantity;
      if (!this.TryGetReagent(id, out quantity))
        throw new KeyNotFoundException(id.ToString());
      return quantity;
    }
  }

  public FixedPoint2 GetReagentQuantity(ReagentId id) => this.GetReagent(id).Quantity;

  public FixedPoint2 GetTotalPrototypeQuantity(params string[] prototypes)
  {
    FixedPoint2 zero = FixedPoint2.Zero;
    foreach ((ReagentId id, FixedPoint2 quantity) in this.Contents)
    {
      if (((IEnumerable<string>) prototypes).Contains<string>(id.Prototype))
        zero += quantity;
    }
    return zero;
  }

  public FixedPoint2 GetTotalPrototypeQuantity(string id)
  {
    FixedPoint2 zero = FixedPoint2.Zero;
    foreach ((ReagentId id1, FixedPoint2 quantity) in this.Contents)
    {
      if (id == id1.Prototype)
        zero += quantity;
    }
    return zero;
  }

  public ReagentId? GetPrimaryReagentId()
  {
    if (this.Contents.Count == 0)
      return new ReagentId?();
    ReagentQuantity reagentQuantity = new ReagentQuantity();
    foreach (ReagentQuantity content in this.Contents)
    {
      if (content.Quantity >= reagentQuantity.Quantity)
        reagentQuantity = content;
    }
    return new ReagentId?(reagentQuantity.Reagent);
  }

  public void AddReagent(string prototype, FixedPoint2 quantity, bool dirtyHeatCap = true)
  {
    this.AddReagent(new ReagentId(prototype, (List<ReagentData>) null), quantity, dirtyHeatCap);
  }

  public void AddReagent(ReagentId id, FixedPoint2 quantity, bool dirtyHeatCap = true)
  {
    if (quantity <= 0)
      return;
    this.Volume += quantity;
    this._heatCapacityDirty |= dirtyHeatCap;
    for (int index = 0; index < this.Contents.Count; ++index)
    {
      (ReagentId id1, FixedPoint2 quantity1) = this.Contents[index];
      ReagentId reagentId = id;
      if (!(id1 != reagentId))
      {
        this.Contents[index] = new ReagentQuantity(id, quantity1 + quantity);
        this.ValidateSolution();
        return;
      }
    }
    this.Contents.Add(new ReagentQuantity(id, quantity));
    this.ValidateSolution();
  }

  public void AddReagent(ReagentPrototype proto, ReagentId reagentId, FixedPoint2 quantity)
  {
    this.AddReagent(reagentId, quantity, false);
    this._heatCapacity += quantity.Float() * proto.SpecificHeat;
    this.CheckRecalculateHeatCapacity();
  }

  public void AddReagent(ReagentQuantity reagentQuantity)
  {
    this.AddReagent(reagentQuantity.Reagent, reagentQuantity.Quantity);
  }

  public void AddReagent(
    ReagentPrototype proto,
    FixedPoint2 quantity,
    float temperature,
    IPrototypeManager? protoMan,
    List<ReagentData>? data = null)
  {
    if (this._heatCapacityDirty)
      this.UpdateHeatCapacity(protoMan);
    float num = (float) ((double) this.Temperature * (double) this._heatCapacity + (double) temperature * (double) proto.SpecificHeat);
    this.AddReagent(new ReagentId(proto.ID, data), quantity);
    this.Temperature = (double) this._heatCapacity == 0.0 ? 0.0f : num / this._heatCapacity;
  }

  public void ScaleSolution(int scale)
  {
    if (scale == 1)
      return;
    if (scale <= 0)
    {
      this.RemoveAllSolution();
    }
    else
    {
      this._heatCapacity *= (float) scale;
      this.Volume *= scale;
      this.CheckRecalculateHeatCapacity();
      for (int index = 0; index < this.Contents.Count; ++index)
      {
        ReagentQuantity content = this.Contents[index];
        this.Contents[index] = new ReagentQuantity(content.Reagent, content.Quantity * scale);
      }
      this.ValidateSolution();
    }
  }

  public void ScaleSolution(float scale)
  {
    if ((double) scale == 1.0)
      return;
    if ((double) scale == 0.0)
    {
      this.RemoveAllSolution();
    }
    else
    {
      this.Volume = FixedPoint2.Zero;
      for (int index = this.Contents.Count - 1; index >= 0; --index)
      {
        ReagentQuantity content = this.Contents[index];
        FixedPoint2 quantity = content.Quantity * scale;
        if (quantity == FixedPoint2.Zero)
        {
          Extensions.RemoveSwap<ReagentQuantity>((IList<ReagentQuantity>) this.Contents, index);
        }
        else
        {
          this.Contents[index] = new ReagentQuantity(content.Reagent, quantity);
          this.Volume += quantity;
        }
      }
      this._heatCapacityDirty = true;
      this.ValidateSolution();
    }
  }

  public FixedPoint2 RemoveReagent(
    ReagentQuantity toRemove,
    bool preserveOrder = false,
    bool ignoreReagentData = false)
  {
    if (toRemove.Quantity <= FixedPoint2.Zero)
      return FixedPoint2.Zero;
    List<int> intList = new List<int>();
    int num = 0;
    ReagentId reagent1;
    FixedPoint2 quantity1;
    for (int index = 0; index < this.Contents.Count; ++index)
    {
      (reagent1, quantity1) = this.Contents[index];
      ReagentId reagentId = reagent1;
      FixedPoint2 fixedPoint2 = quantity1;
      if (ignoreReagentData)
      {
        string prototype1 = reagentId.Prototype;
        reagent1 = toRemove.Reagent;
        string prototype2 = reagent1.Prototype;
        if (prototype1 != prototype2)
          continue;
      }
      else if (reagentId != toRemove.Reagent)
        continue;
      num += fixedPoint2.Value;
      intList.Insert(0, index);
    }
    if (num <= 0)
      return FixedPoint2.Zero;
    FixedPoint2 fixedPoint2_1 = (FixedPoint2) 0;
    for (int index = 0; index < intList.Count; ++index)
    {
      (reagent1, quantity1) = this.Contents[intList[index]];
      ReagentId reagent2 = reagent1;
      FixedPoint2 fixedPoint2_2 = quantity1;
      quantity1 = toRemove.Quantity;
      FixedPoint2 fixedPoint2_3 = FixedPoint2.FromCents((int) ((long) quantity1.Value * (long) fixedPoint2_2.Value / (long) num));
      FixedPoint2 quantity2 = fixedPoint2_2 - fixedPoint2_3;
      this._heatCapacityDirty = true;
      if (quantity2 <= 0)
      {
        if (!preserveOrder)
          Extensions.RemoveSwap<ReagentQuantity>((IList<ReagentQuantity>) this.Contents, intList[index]);
        else
          this.Contents.RemoveAt(intList[index]);
        this.Volume -= fixedPoint2_2;
        fixedPoint2_1 += fixedPoint2_2;
      }
      else
      {
        this.Contents[intList[index]] = new ReagentQuantity(reagent2, quantity2);
        this.Volume -= fixedPoint2_3;
        fixedPoint2_1 += fixedPoint2_3;
      }
    }
    this.ValidateSolution();
    return fixedPoint2_1;
  }

  public FixedPoint2 RemoveReagent(
    string prototype,
    FixedPoint2 quantity,
    List<ReagentData>? data = null,
    bool ignoreReagentData = false)
  {
    return this.RemoveReagent(new ReagentQuantity(prototype, quantity, data), ignoreReagentData: ignoreReagentData);
  }

  public FixedPoint2 RemoveReagent(
    ReagentId reagentId,
    FixedPoint2 quantity,
    bool preserveOrder = false,
    bool ignoreReagentData = false)
  {
    return this.RemoveReagent(new ReagentQuantity(reagentId, quantity), preserveOrder, ignoreReagentData);
  }

  public void RemoveAllSolution()
  {
    this.Contents.Clear();
    this.Volume = FixedPoint2.Zero;
    this._heatCapacityDirty = false;
    this._heatCapacity = 0.0f;
  }

  public Solution SplitSolutionWithout(FixedPoint2 toTake, params string[] excludedPrototypes)
  {
    List<ReagentQuantity> reagentQuantityList = new List<ReagentQuantity>();
    foreach (string excludedPrototype in excludedPrototypes)
    {
      foreach (ReagentQuantity content in this.Contents)
      {
        if (!(content.Reagent.Prototype != excludedPrototype))
        {
          reagentQuantityList.Add(content);
          this.RemoveReagent(content);
          break;
        }
      }
    }
    Solution solution = this.SplitSolution(toTake);
    foreach (ReagentQuantity reagentQuantity in reagentQuantityList)
      this.AddReagent(reagentQuantity);
    return solution;
  }

  public Solution SplitSolutionWithOnly(FixedPoint2 toTake, params string[] includedPrototypes)
  {
    List<ReagentQuantity> reagentQuantityList = new List<ReagentQuantity>();
    for (int index = this.Contents.Count - 1; index >= 0; --index)
    {
      if (!((IEnumerable<string>) includedPrototypes).Contains<string>(this.Contents[index].Reagent.Prototype))
      {
        reagentQuantityList.Add(this.Contents[index]);
        this.RemoveReagent(this.Contents[index]);
      }
    }
    Solution solution = this.SplitSolution(toTake);
    foreach (ReagentQuantity reagentQuantity in reagentQuantityList)
      this.AddReagent(reagentQuantity);
    return solution;
  }

  public Solution SplitSolution(FixedPoint2 toTake)
  {
    if (toTake <= FixedPoint2.Zero)
      return new Solution();
    if (toTake >= this.Volume)
    {
      Solution solution = this.Clone();
      this.RemoveAllSolution();
      return solution;
    }
    FixedPoint2 volume1 = this.Volume;
    FixedPoint2 volume2 = this.Volume;
    int num1 = volume2.Value;
    Solution solution1 = new Solution(this.Contents.Count)
    {
      Temperature = this.Temperature
    };
    long num2 = (long) toTake.Value;
    for (int index = this.Contents.Count - 1; index >= 0; --index)
    {
      ReagentId reagentId;
      (reagentId, volume2) = this.Contents[index];
      FixedPoint2 fixedPoint2 = volume2;
      long num3 = num2 * (long) fixedPoint2.Value / (long) num1;
      if (num3 <= 0L)
      {
        num1 -= fixedPoint2.Value;
      }
      else
      {
        FixedPoint2 quantity1 = FixedPoint2.FromCents((int) num3);
        FixedPoint2 quantity2 = fixedPoint2 - quantity1;
        if (quantity2 > FixedPoint2.Zero)
          this.Contents[index] = new ReagentQuantity(reagentId, quantity2);
        else
          Extensions.RemoveSwap<ReagentQuantity>((IList<ReagentQuantity>) this.Contents, index);
        solution1.Contents.Add(new ReagentQuantity(reagentId, quantity1));
        this.Volume -= quantity1;
        num2 -= num3;
        num1 -= fixedPoint2.Value;
      }
    }
    solution1.Volume = volume1 - this.Volume;
    this._heatCapacityDirty = true;
    solution1._heatCapacityDirty = true;
    this.ValidateSolution();
    solution1.ValidateSolution();
    return solution1;
  }

  public void RemoveSolution(FixedPoint2 toTake)
  {
    if (toTake <= FixedPoint2.Zero)
      return;
    if (toTake >= this.Volume)
    {
      this.RemoveAllSolution();
    }
    else
    {
      FixedPoint2 volume = this.Volume;
      int num1 = volume.Value;
      this.Volume -= toTake;
      long num2 = (long) toTake.Value;
      for (int index = this.Contents.Count - 1; index >= 0; --index)
      {
        ReagentId reagentId;
        (reagentId, volume) = this.Contents[index];
        FixedPoint2 fixedPoint2_1 = volume;
        long num3 = num2 * (long) fixedPoint2_1.Value / (long) num1;
        if (num3 <= 0L)
        {
          num1 -= fixedPoint2_1.Value;
        }
        else
        {
          FixedPoint2 fixedPoint2_2 = FixedPoint2.FromCents((int) num3);
          FixedPoint2 quantity = fixedPoint2_1 - fixedPoint2_2;
          if (quantity > FixedPoint2.Zero)
            this.Contents[index] = new ReagentQuantity(reagentId, quantity);
          else
            Extensions.RemoveSwap<ReagentQuantity>((IList<ReagentQuantity>) this.Contents, index);
          num2 -= num3;
          num1 -= fixedPoint2_1.Value;
        }
      }
      this._heatCapacityDirty = true;
      this.ValidateSolution();
    }
  }

  public void AddSolution(Solution otherSolution, IPrototypeManager? protoMan)
  {
    if (otherSolution.Volume <= FixedPoint2.Zero)
      return;
    this.Volume += otherSolution.Volume;
    bool flag1 = MathHelper.CloseTo(otherSolution.Temperature, this.Temperature, 1E-07f);
    float num = 0.0f;
    if (!flag1)
    {
      IoCManager.Resolve<IPrototypeManager>(ref protoMan);
      if (this._heatCapacityDirty)
        this.UpdateHeatCapacity(protoMan);
      if (otherSolution._heatCapacityDirty)
        otherSolution.UpdateHeatCapacity(protoMan);
      num = (float) ((double) this._heatCapacity * (double) this.Temperature + (double) otherSolution._heatCapacity * (double) otherSolution.Temperature);
    }
    for (int index1 = 0; index1 < otherSolution.Contents.Count; ++index1)
    {
      ReagentQuantity content = otherSolution.Contents[index1];
      (ReagentId id, FixedPoint2 quantity1) = content;
      ReagentId reagent1 = id;
      FixedPoint2 quantity2 = quantity1;
      bool flag2 = false;
      for (int index2 = 0; index2 < this.Contents.Count; ++index2)
      {
        content = this.Contents[index2];
        (id, quantity1) = content;
        ReagentId reagent2 = id;
        FixedPoint2 fixedPoint2 = quantity1;
        if (reagent2 == reagent1)
        {
          flag2 = true;
          this.Contents[index2] = new ReagentQuantity(reagent2, fixedPoint2 + quantity2);
          break;
        }
      }
      if (!flag2)
        this.Contents.Add(new ReagentQuantity(reagent1, quantity2));
    }
    this._heatCapacity += otherSolution._heatCapacity;
    this.CheckRecalculateHeatCapacity();
    if (flag1)
      this._heatCapacityDirty |= otherSolution._heatCapacityDirty;
    else
      this.Temperature = (double) this._heatCapacity == 0.0 ? 0.0f : num / this._heatCapacity;
    this.ValidateSolution();
  }

  public Color GetColorWithout(IPrototypeManager? protoMan, params string[] without)
  {
    if (this.Volume == FixedPoint2.Zero)
      return Color.Transparent;
    IoCManager.Resolve<IPrototypeManager>(ref protoMan);
    RMCReagentSystem rmcReagentSystem = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
    Color colorWithout = new Color();
    FixedPoint2 fixedPoint2 = FixedPoint2.New(0);
    bool flag = true;
    foreach ((ReagentId id, FixedPoint2 quantity) in this.Contents)
    {
      if (!((IEnumerable<string>) without).Contains<string>(id.Prototype))
      {
        fixedPoint2 += quantity;
        Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
        if (rmcReagentSystem.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(id.Prototype), out reagent))
        {
          if (flag)
          {
            flag = false;
            colorWithout = reagent.SubstanceColor;
          }
          else
          {
            float num = quantity.Float() / fixedPoint2.Float();
            colorWithout = Color.InterpolateBetween(colorWithout, reagent.SubstanceColor, num);
          }
        }
      }
    }
    return colorWithout;
  }

  public Color GetColor(IPrototypeManager? protoMan) => this.GetColorWithout(protoMan);

  public Color GetColorWithOnly(IPrototypeManager? protoMan, params string[] included)
  {
    if (this.Volume == FixedPoint2.Zero)
      return Color.Transparent;
    IoCManager.Resolve<IPrototypeManager>(ref protoMan);
    RMCReagentSystem rmcReagentSystem = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
    Color colorWithOnly = new Color();
    FixedPoint2 fixedPoint2 = FixedPoint2.New(0);
    bool flag = true;
    foreach ((ReagentId id, FixedPoint2 quantity) in this.Contents)
    {
      if (((IEnumerable<string>) included).Contains<string>(id.Prototype))
      {
        fixedPoint2 += quantity;
        Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
        if (rmcReagentSystem.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(id.Prototype), out reagent))
        {
          if (flag)
          {
            flag = false;
            colorWithOnly = reagent.SubstanceColor;
          }
          else
          {
            float num = quantity.Float() / fixedPoint2.Float();
            colorWithOnly = Color.InterpolateBetween(colorWithOnly, reagent.SubstanceColor, num);
          }
        }
      }
    }
    return colorWithOnly;
  }

  public IEnumerator<ReagentQuantity> GetEnumerator()
  {
    return (IEnumerator<ReagentQuantity>) this.Contents.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void SetContents(IEnumerable<ReagentQuantity> reagents, bool setMaxVol = false)
  {
    this.Volume = (FixedPoint2) 0;
    this.RemoveAllSolution();
    this._heatCapacityDirty = true;
    this.Contents = new List<ReagentQuantity>(reagents);
    foreach (ReagentQuantity content in this.Contents)
      this.Volume += content.Quantity;
    if (setMaxVol)
      this.MaxVolume = this.Volume;
    this.ValidateSolution();
  }

  public Dictionary<ReagentPrototype, FixedPoint2> GetReagentPrototypes(IPrototypeManager protoMan)
  {
    Dictionary<ReagentPrototype, FixedPoint2> dictionary = new Dictionary<ReagentPrototype, FixedPoint2>(this.Contents.Count);
    RMCReagentSystem rmcReagentSystem = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
    foreach ((ReagentId id, FixedPoint2 quantity) in this.Contents)
    {
      Content.Shared._RMC14.Chemistry.Reagent.Reagent key = rmcReagentSystem.Index(ProtoId<ReagentPrototype>.op_Implicit(id.Prototype));
      dictionary[(ReagentPrototype) key] = quantity + dictionary.GetValueOrDefault<ReagentPrototype, FixedPoint2>((ReagentPrototype) key);
    }
    return dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Solution target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Solution>(this, ref target, hookCtx, true, context))
      return;
    List<ReagentQuantity> reagentQuantityList = (List<ReagentQuantity>) null;
    if (this.Contents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ReagentQuantity>>(this.Contents, ref reagentQuantityList, hookCtx, true, context))
      reagentQuantityList = serialization.CreateCopy<List<ReagentQuantity>>(this.Contents, hookCtx, context, false);
    target.Contents = reagentQuantityList;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxVolume, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.MaxVolume, hookCtx, context, false);
    target.MaxVolume = fixedPoint2;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.CanReact, ref flag, hookCtx, false, context))
      flag = this.CanReact;
    target.CanReact = flag;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Temperature, ref num, hookCtx, false, context))
      num = this.Temperature;
    target.Temperature = num;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Name, ref str, hookCtx, false, context))
      str = this.Name;
    target.Name = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Solution target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Solution target1 = (Solution) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public Solution Instantiate() => new Solution();
}
