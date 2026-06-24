// Decompiled with JetBrains decompiler
// Type: Content.Shared.GPS.Systems.HandheldGpsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.GPS.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared.GPS.Systems;

public sealed class HandheldGpsSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HandheldGPSComponent, ExaminedEvent>(new EntityEventRefHandler<HandheldGPSComponent, ExaminedEvent>(this.OnExamine));
  }

  private void OnExamine(Entity<HandheldGPSComponent> ent, ref ExaminedEvent args)
  {
    string str = "Error";
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) ent);
    if (mapCoordinates.MapId != MapId.Nullspace)
      str = $"({(int) mapCoordinates.Position.X}, {(int) mapCoordinates.Position.Y})";
    args.PushMarkup(this.Loc.GetString("handheld-gps-coordinates-title", ("coordinates", (object) str)));
  }
}
