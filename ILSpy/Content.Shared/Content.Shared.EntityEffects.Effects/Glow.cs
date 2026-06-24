using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class Glow : EntityEffect, ISerializationGenerated<Glow>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Radius = 2f;

	[DataField(null, false, 1, false, false, null)]
	public Color Color = Color.Black;

	private static readonly List<Color> Colors = new List<Color>
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Color == Color.Black)
		{
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			Color = RandomExtensions.Pick<Color>(random, (IReadOnlyList<Color>)Colors);
		}
		SharedPointLightSystem obj = args.EntityManager.System<SharedPointLightSystem>();
		SharedPointLightComponent light = obj.EnsureLight(args.TargetEntity);
		obj.SetRadius(args.TargetEntity, Radius, light, (MetaDataComponent)null);
		obj.SetColor(args.TargetEntity, Color, light);
		obj.SetCastShadows(args.TargetEntity, false, light, (MetaDataComponent)null);
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return "TODO";
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Glow target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Glow)definitionCast;
		if (!serialization.TryCustomCopy<Glow>(this, ref target, hookCtx, false, context))
		{
			float RadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Radius, ref RadiusTemp, hookCtx, false, context))
			{
				RadiusTemp = Radius;
			}
			target.Radius = RadiusTemp;
			Color ColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
			}
			target.Color = ColorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Glow target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Glow cast = (Glow)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Glow cast = (Glow)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Glow Instantiate()
	{
		return new Glow();
	}
}
