// Decompiled with JetBrains decompiler
// Type: Content.Client.Access.AccessOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.StationRecords;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

#nullable enable
namespace Content.Client.Access;

public sealed class AccessOverlay : Overlay
{
  private const string TextFontPath = "/Fonts/NotoSans/NotoSans-Regular.ttf";
  private const int TextFontSize = 12;
  private readonly IEntityManager _entityManager;
  private readonly SharedTransformSystem _transformSystem;
  private readonly Font _font;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public AccessOverlay(
    IEntityManager entityManager,
    IResourceCache resourceCache,
    SharedTransformSystem transformSystem)
  {
    this._entityManager = entityManager;
    this._transformSystem = transformSystem;
    this._font = resourceCache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.ViewportControl == null)
      return;
    StringBuilder stringBuilder = new StringBuilder();
    EntityQueryEnumerator<AccessReaderComponent, TransformComponent> entityQueryEnumerator = this._entityManager.EntityQueryEnumerator<AccessReaderComponent, TransformComponent>();
    EntityUid entityUid;
    AccessReaderComponent accessReaderComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref accessReaderComponent, ref transformComponent))
    {
      stringBuilder.Clear();
      EntityStringRepresentation prettyString = this._entityManager.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entityUid));
      stringBuilder.AppendLine(((EntityStringRepresentation) ref prettyString).Prototype);
      stringBuilder.Append("UID: ");
      stringBuilder.Append(((EntityStringRepresentation) ref prettyString).Uid.Id);
      stringBuilder.Append(", NUID: ");
      stringBuilder.Append(((EntityStringRepresentation) ref prettyString).Nuid.Id);
      stringBuilder.AppendLine();
      if (!accessReaderComponent.Enabled)
      {
        stringBuilder.AppendLine("-Disabled");
      }
      else
      {
        if (accessReaderComponent.AccessLists.Count > 0)
        {
          int num = 0;
          foreach (HashSet<ProtoId<AccessLevelPrototype>> accessList in accessReaderComponent.AccessLists)
          {
            ++num;
            foreach (ProtoId<AccessLevelPrototype> protoId in accessList)
            {
              stringBuilder.Append("+Set ");
              stringBuilder.Append(num);
              stringBuilder.Append(": ");
              stringBuilder.Append(protoId.Id);
              stringBuilder.AppendLine();
            }
          }
        }
        else
          stringBuilder.AppendLine("+Unrestricted");
        foreach (StationRecordKey accessKey in accessReaderComponent.AccessKeys)
        {
          stringBuilder.Append("+Key ");
          stringBuilder.Append((object) accessKey.OriginStation);
          stringBuilder.Append(": ");
          stringBuilder.Append(accessKey.Id);
          stringBuilder.AppendLine();
        }
        foreach (ProtoId<AccessLevelPrototype> denyTag in accessReaderComponent.DenyTags)
        {
          stringBuilder.Append("-Tag ");
          stringBuilder.AppendLine(denyTag.Id);
        }
        string str = stringBuilder.ToString();
        Vector2 screen = args.ViewportControl.WorldToScreen(this._transformSystem.GetWorldPosition(transformComponent));
        ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, screen, str, Color.Gold);
      }
    }
  }
}
