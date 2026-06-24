// Decompiled with JetBrains decompiler
// Type: Content.Client.NPC.NPCSteeringOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.PhysicsSystem.Controllers;
using Content.Shared.Movement.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.NPC;

public sealed class NPCSteeringOverlay : Overlay
{
  private readonly IEntityManager _entManager;
  private readonly SharedTransformSystem _transformSystem;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public NPCSteeringOverlay(IEntityManager entManager)
  {
    this._entManager = entManager;
    this._transformSystem = this._entManager.System<SharedTransformSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    foreach ((NPCSteeringComponent steeringComponent, InputMoverComponent mover, TransformComponent transformComponent) in this._entManager.EntityQuery<NPCSteeringComponent, InputMoverComponent, TransformComponent>(true))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId))
      {
        Vector2 vector2_1 = this._transformSystem.GetWorldPositionRotation(transformComponent).Item1;
        if (((Box2) ref args.WorldAABB).Contains(vector2_1, true))
        {
          ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawCircle(vector2_1, 1f, Color.Green, false);
          Angle parentGridAngle = this._entManager.System<MoverController>().GetParentGridAngle(mover);
          foreach (Vector2 dangerPoint in steeringComponent.DangerPoints)
          {
            DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
            Vector2 vector2_2 = dangerPoint;
            Color red = Color.Red;
            Color color = ((Color) ref red).WithAlpha(0.6f);
            ((DrawingHandleBase) worldHandle).DrawCircle(vector2_2, 0.1f, color, true);
          }
          for (int index = 0; index < 12; ++index)
          {
            float danger = steeringComponent.DangerMap[index];
            float interest = steeringComponent.InterestMap[index];
            Angle angle1 = Angle.FromDegrees((double) (index * 30));
            DrawingHandleWorld worldHandle1 = ((OverlayDrawArgs) ref args).WorldHandle;
            Vector2 vector2_3 = vector2_1;
            Vector2 vector2_4 = vector2_1;
            Angle angle2 = Angle.op_Addition(parentGridAngle, angle1);
            ref Angle local1 = ref angle2;
            Vector2 vector2_5 = new Vector2(interest, 0.0f);
            ref Vector2 local2 = ref vector2_5;
            Vector2 vector2_6 = ((Angle) ref local1).RotateVec(ref local2);
            Vector2 vector2_7 = vector2_4 + vector2_6;
            Color limeGreen = Color.LimeGreen;
            ((DrawingHandleBase) worldHandle1).DrawLine(vector2_3, vector2_7, limeGreen);
            DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs) ref args).WorldHandle;
            Vector2 vector2_8 = vector2_1;
            Vector2 vector2_9 = vector2_1;
            Angle angle3 = Angle.op_Addition(parentGridAngle, angle1);
            ref Angle local3 = ref angle3;
            Vector2 vector2_10 = new Vector2(danger, 0.0f);
            ref Vector2 local4 = ref vector2_10;
            Vector2 vector2_11 = ((Angle) ref local3).RotateVec(ref local4);
            Vector2 vector2_12 = vector2_9 + vector2_11;
            Color red = Color.Red;
            ((DrawingHandleBase) worldHandle2).DrawLine(vector2_8, vector2_12, red);
          }
          ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawLine(vector2_1, vector2_1 + ((Angle) ref parentGridAngle).RotateVec(ref steeringComponent.Direction), Color.Cyan);
        }
      }
    }
  }
}
