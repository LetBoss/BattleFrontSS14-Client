using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Content.Client.Administration.Managers;
using Content.Shared._RMC14.Figurines;
using Content.Shared.Administration;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client._RMC14.Figurines;

public sealed class FigurineSystem : EntitySystem
{
	private sealed class ContentSpriteControl : Control
	{
		[Dependency]
		private IEntityManager _entManager;

		internal readonly Queue<(IRenderTexture Texture, Direction Direction, EntityUid Entity)> QueuedTextures = new Queue<(IRenderTexture, Direction, EntityUid)>();

		public ContentSpriteControl()
		{
			IoCManager.InjectDependencies<ContentSpriteControl>(this);
		}

		protected override void Draw(DrawingHandleScreen handle)
		{
			((Control)this).Draw(handle);
			ISawmill log = ((EntitySystem)_entManager.System<FigurineSystem>()).Log;
			(IRenderTexture, Direction, EntityUid) result;
			while (QueuedTextures.TryDequeue(out result))
			{
				try
				{
					(IRenderTexture Texture, Direction Direction, EntityUid Entity) result2 = result;
					((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)result.Item1, (Action)delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0021: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						//IL_0036: Unknown result type (might be due to invalid IL or missing references)
						//IL_0048: Unknown result type (might be due to invalid IL or missing references)
						//IL_0054: Unknown result type (might be due to invalid IL or missing references)
						//IL_005a: Unknown result type (might be due to invalid IL or missing references)
						DrawingHandleScreen obj = handle;
						EntityUid item = result2.Entity;
						Vector2 vector = Vector2i.op_Implicit(((IRenderTarget)result2.Texture).Size / 2);
						Vector2 one = Vector2.One;
						Angle? val = Angle.Zero;
						Direction? val2 = result2.Direction;
						obj.DrawEntity(item, vector, one, val, default(Angle), val2, (SpriteComponent)null, (TransformComponent)null, (SharedTransformSystem)null);
					}, (Color?)null);
					((IRenderTarget)result.Item1).CopyPixelsToMemory<Rgba32>((CopyPixelsDelegate<Rgba32>)delegate(Image<Rgba32> image)
					{
						MemoryStream memoryStream = new MemoryStream();
						ImageExtensions.SaveAsPng((Image)(object)image, (Stream)memoryStream);
						FigurineImageEvent figurineImageEvent = new FigurineImageEvent(memoryStream.ToArray());
						_entManager.EntityNetManager.SendSystemNetworkMessage((EntityEventArgs)(object)figurineImageEvent, true);
					}, (UIBox2i?)null);
				}
				catch (Exception ex)
				{
					((IDisposable)result.Item1).Dispose();
					if (!string.IsNullOrEmpty(ex.StackTrace))
					{
						log.Fatal(ex.StackTrace);
					}
				}
			}
		}
	}

	[Dependency]
	private IClientAdminManager _adminManager;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private SpriteSystem _sprite;

	private readonly ContentSpriteControl _control = new ContentSpriteControl();

	public override void Initialize()
	{
	}

	public override void Shutdown()
	{
	}

	private void OnFigurineRequest(FigurineRequestEvent ev)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted || !_adminManager.HasFlag(AdminFlags.Host))
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(valueOrDefault, ref val))
		{
			return;
		}
		_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((valueOrDefault, val)), Vector2.One);
		Vector2i val2 = Vector2i.Zero;
		foreach (ISpriteLayer allLayer in val.AllLayers)
		{
			if (allLayer.Visible)
			{
				val2 = Vector2i.ComponentMax(val2, allLayer.PixelSize);
			}
		}
		if (!((Vector2i)(ref val2)).Equals(Vector2i.Zero))
		{
			IRenderTexture item = _clyde.CreateRenderTarget(new Vector2i(val2.X, val2.Y), new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "export");
			_control.QueuedTextures.Enqueue((item, (Direction)0, valueOrDefault));
		}
	}
}
