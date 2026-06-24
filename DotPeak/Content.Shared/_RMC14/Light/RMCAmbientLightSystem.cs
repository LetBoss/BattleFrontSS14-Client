// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Light.RMCAmbientLightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Dataset;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Light;

public sealed class RMCAmbientLightSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IPrototypeManager _prototype;

  public Color GetColor(Entity<RMCAmbientLightComponent> ent, TimeSpan curTime)
  {
    if (ent.Comp.Colors.Count == 0 || ent.Comp.Duration <= TimeSpan.Zero)
      return Color.Black;
    if (ent.Comp.Colors.Count == 1)
      return ent.Comp.Colors[0];
    double num1 = Math.Clamp((curTime - ent.Comp.StartTime) / ent.Comp.Duration, 0.0, 1.0);
    int num2 = ent.Comp.Colors.Count - 1;
    double num3 = 1.0 / (double) num2;
    int index1 = Math.Min((int) (num1 / num3), num2 - 1);
    int index2 = index1 + 1;
    double num4 = Math.Clamp((num1 - (double) index1 * num3) / num3, 0.0, 1.0);
    return Color.InterpolateBetween(ent.Comp.Colors[index1], ent.Comp.Colors[index2], (float) num4);
  }

  public List<Color> ProcessPrototype(ProtoId<DatasetPrototype> protoId)
  {
    List<Color> colorList = new List<Color>();
    DatasetPrototype prototype;
    return !this._prototype.TryIndex<DatasetPrototype>(protoId, out prototype) ? colorList : prototype.Values.Select<string, Color>((Func<string, Color>) (hex => Color.FromHex((ReadOnlySpan<char>) hex, new Color?(Color.Black)))).ToList<Color>();
  }

  public void SetColor(Entity<RMCAmbientLightComponent> ent, Color colorHex, TimeSpan duration)
  {
    if (this._net.IsClient)
      return;
    MapLightComponent mapLightComponent = this.EnsureComp<MapLightComponent>((EntityUid) ent);
    ent.Comp.Colors.Clear();
    // ISSUE: object of a compiler-generated type is created
    ent.Comp.Colors.AddRange((IEnumerable<Color>) new \u003C\u003Ez__ReadOnlyArray<Color>(new Color[2]
    {
      mapLightComponent.AmbientLightColor,
      colorHex
    }));
    ent.Comp.Duration = duration;
    ent.Comp.StartTime = this._timing.CurTime;
    ent.Comp.IsAnimating = true;
    this.Dirty<RMCAmbientLightComponent>(ent);
  }

  public void SetColor(
    Entity<RMCAmbientLightComponent> ent,
    List<Color> colorList,
    TimeSpan duration)
  {
    if (this._net.IsClient || colorList.Count == 0 || duration <= TimeSpan.Zero)
      return;
    this.EnsureComp<MapLightComponent>((EntityUid) ent);
    ent.Comp.Colors.Clear();
    ent.Comp.Colors.AddRange((IEnumerable<Color>) colorList);
    ent.Comp.Duration = duration;
    ent.Comp.StartTime = this._timing.CurTime;
    ent.Comp.IsAnimating = true;
    this.Dirty<RMCAmbientLightComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCAmbientLightComponent, MapLightComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCAmbientLightComponent, MapLightComponent>();
    TimeSpan curTime = this._timing.CurTime;
    EntityUid uid;
    RMCAmbientLightComponent comp1;
    MapLightComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.IsAnimating)
      {
        if (curTime >= comp1.EndTime)
        {
          MapLightComponent mapLightComponent = comp2;
          List<Color> colors = comp1.Colors;
          Color color = colors[colors.Count - 1];
          mapLightComponent.AmbientLightColor = color;
          comp1.IsAnimating = false;
          this.Dirty(uid, (IComponent) comp1);
          this.Dirty(uid, (IComponent) comp2);
        }
        else
        {
          Color color = this.GetColor((Entity<RMCAmbientLightComponent>) (uid, comp1), curTime);
          comp2.AmbientLightColor = color;
          this.Dirty(uid, (IComponent) comp2);
        }
      }
    }
  }
}
