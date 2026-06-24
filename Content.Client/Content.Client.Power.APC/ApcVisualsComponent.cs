using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Power.APC;

[RegisterComponent]
[Access(new Type[] { typeof(ApcVisualizerSystem) })]
public sealed class ApcVisualsComponent : Component, ISerializationGenerated<ApcVisualsComponent>, ISerializationGenerated
{
	[DataField("numLockIndicators", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public byte LockIndicators = 2;

	[DataField("lockIndicatorPrefix", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LockPrefix = "lock";

	[DataField("lockIndicatorSuffixes", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string[] LockSuffixes = new string[2] { "unlocked", "locked" };

	[DataField("numChannelIndicators", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public byte ChannelIndicators = 3;

	[DataField("channelIndicatorPrefix", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ChannelPrefix = "channel";

	[DataField("channelIndicatorSuffixes", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string[] ChannelSuffixes = new string[4] { "auto_off", "manual_off", "auto_on", "manual_on" };

	[DataField("screenStatePrefix", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ScreenPrefix = "display";

	[DataField("screenStateSuffixes", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string[] ScreenSuffixes = new string[4] { "lack", "charging", "full", "remote" };

	[DataField("screenColors", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Color[] ScreenColors = (Color[])(object)new Color[4]
	{
		Color.FromHex((ReadOnlySpan<char>)"#d1332e", (Color?)null),
		Color.FromHex((ReadOnlySpan<char>)"#dcdc28", (Color?)null),
		Color.FromHex((ReadOnlySpan<char>)"#82ff4c", (Color?)null),
		Color.FromHex((ReadOnlySpan<char>)"#ffac1c", (Color?)null)
	};

	[DataField("emaggedScreenState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string EmaggedScreenState = "emag-unlit";

	[DataField("emaggedScreenColor", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Color EmaggedScreenColor = Color.FromHex((ReadOnlySpan<char>)"#1f48d6", (Color?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ApcVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (ApcVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<ApcVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			byte lockIndicators = 0;
			if (!serialization.TryCustomCopy<byte>(LockIndicators, ref lockIndicators, hookCtx, false, context))
			{
				lockIndicators = LockIndicators;
			}
			target.LockIndicators = lockIndicators;
			string lockPrefix = null;
			if (LockPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(LockPrefix, ref lockPrefix, hookCtx, false, context))
			{
				lockPrefix = LockPrefix;
			}
			target.LockPrefix = lockPrefix;
			string[] lockSuffixes = null;
			if (LockSuffixes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string[]>(LockSuffixes, ref lockSuffixes, hookCtx, true, context))
			{
				lockSuffixes = serialization.CreateCopy<string[]>(LockSuffixes, hookCtx, context, false);
			}
			target.LockSuffixes = lockSuffixes;
			byte channelIndicators = 0;
			if (!serialization.TryCustomCopy<byte>(ChannelIndicators, ref channelIndicators, hookCtx, false, context))
			{
				channelIndicators = ChannelIndicators;
			}
			target.ChannelIndicators = channelIndicators;
			string channelPrefix = null;
			if (ChannelPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ChannelPrefix, ref channelPrefix, hookCtx, false, context))
			{
				channelPrefix = ChannelPrefix;
			}
			target.ChannelPrefix = channelPrefix;
			string[] channelSuffixes = null;
			if (ChannelSuffixes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string[]>(ChannelSuffixes, ref channelSuffixes, hookCtx, true, context))
			{
				channelSuffixes = serialization.CreateCopy<string[]>(ChannelSuffixes, hookCtx, context, false);
			}
			target.ChannelSuffixes = channelSuffixes;
			string screenPrefix = null;
			if (ScreenPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ScreenPrefix, ref screenPrefix, hookCtx, false, context))
			{
				screenPrefix = ScreenPrefix;
			}
			target.ScreenPrefix = screenPrefix;
			string[] screenSuffixes = null;
			if (ScreenSuffixes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string[]>(ScreenSuffixes, ref screenSuffixes, hookCtx, true, context))
			{
				screenSuffixes = serialization.CreateCopy<string[]>(ScreenSuffixes, hookCtx, context, false);
			}
			target.ScreenSuffixes = screenSuffixes;
			Color[] screenColors = null;
			if (ScreenColors == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Color[]>(ScreenColors, ref screenColors, hookCtx, true, context))
			{
				screenColors = serialization.CreateCopy<Color[]>(ScreenColors, hookCtx, context, false);
			}
			target.ScreenColors = screenColors;
			string emaggedScreenState = null;
			if (EmaggedScreenState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(EmaggedScreenState, ref emaggedScreenState, hookCtx, false, context))
			{
				emaggedScreenState = EmaggedScreenState;
			}
			target.EmaggedScreenState = emaggedScreenState;
			Color emaggedScreenColor = default(Color);
			if (!serialization.TryCustomCopy<Color>(EmaggedScreenColor, ref emaggedScreenColor, hookCtx, false, context))
			{
				emaggedScreenColor = serialization.CreateCopy<Color>(EmaggedScreenColor, hookCtx, context, false);
			}
			target.EmaggedScreenColor = emaggedScreenColor;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ApcVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ApcVisualsComponent target2 = (ApcVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ApcVisualsComponent target2 = (ApcVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ApcVisualsComponent target2 = (ApcVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ApcVisualsComponent Instantiate()
	{
		return new ApcVisualsComponent();
	}
}
