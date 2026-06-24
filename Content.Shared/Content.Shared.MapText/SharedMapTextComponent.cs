using System;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.MapText;

[NetworkedComponent]
[Access(new Type[] { typeof(SharedMapTextSystem) })]
public abstract class SharedMapTextComponent : Component, ISerializationGenerated<SharedMapTextComponent>, ISerializationGenerated
{
	public const string DefaultFont = "Default";

	[DataField(null, false, 1, false, false, null)]
	public string? Text;

	[DataField(null, false, 1, false, false, null)]
	public LocId LocText = LocId.op_Implicit("map-text-default");

	[DataField(null, false, 1, false, false, null)]
	public Color Color = Color.White;

	[DataField(null, false, 1, false, false, null)]
	public string FontId = "Default";

	[DataField(null, false, 1, false, false, null)]
	public int FontSize = 12;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 Offset = Vector2.Zero;

	public SharedMapTextComponent()
	{
	}//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	//IL_000b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	//IL_0016: Unknown result type (might be due to invalid IL or missing references)


	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedMapTextComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedMapTextComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SharedMapTextComponent>(this, ref target, hookCtx, false, context))
		{
			string TextTemp = null;
			if (!serialization.TryCustomCopy<string>(Text, ref TextTemp, hookCtx, false, context))
			{
				TextTemp = Text;
			}
			target.Text = TextTemp;
			LocId LocTextTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(LocText, ref LocTextTemp, hookCtx, false, context))
			{
				LocTextTemp = serialization.CreateCopy<LocId>(LocText, hookCtx, context, false);
			}
			target.LocText = LocTextTemp;
			Color ColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
			}
			target.Color = ColorTemp;
			string FontIdTemp = null;
			if (FontId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FontId, ref FontIdTemp, hookCtx, false, context))
			{
				FontIdTemp = FontId;
			}
			target.FontId = FontIdTemp;
			int FontSizeTemp = 0;
			if (!serialization.TryCustomCopy<int>(FontSize, ref FontSizeTemp, hookCtx, false, context))
			{
				FontSizeTemp = FontSize;
			}
			target.FontSize = FontSizeTemp;
			Vector2 OffsetTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Offset, ref OffsetTemp, hookCtx, false, context))
			{
				OffsetTemp = serialization.CreateCopy<Vector2>(Offset, hookCtx, context, false);
			}
			target.Offset = OffsetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedMapTextComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedMapTextComponent cast = (SharedMapTextComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedMapTextComponent cast = (SharedMapTextComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedMapTextComponent def = (SharedMapTextComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedMapTextComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
