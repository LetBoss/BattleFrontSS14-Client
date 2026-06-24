using System;
using System.IO;
using System.Threading.Tasks;
using Content.Client.Viewport;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Shared.ContentPack;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Screenshot;

internal sealed class ScreenshotHook : IScreenshotHook
{
	private static readonly ResPath BaseScreenshotPath = new ResPath("/Screenshots");

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IResourceManager _resourceManager;

	[Dependency]
	private IStateManager _stateManager;

	private ISawmill _sawmill;

	public void Initialize()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		_sawmill = Logger.GetSawmill("screenshot");
		_sawmill.Level = (LogLevel)2;
		_inputManager.SetInputCommand(ContentKeyFunctions.TakeScreenshot, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			_clyde.Screenshot((ScreenshotType)0, (CopyPixelsDelegate<Rgb24>)Take<Rgb24>, (UIBox2i?)null);
		}, (StateInputCmdDelegate)null, true, true));
		_inputManager.SetInputCommand(ContentKeyFunctions.TakeScreenshotNoUI, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			if (_stateManager.CurrentState is IMainViewportState mainViewportState)
			{
				mainViewportState.Viewport.Viewport.Screenshot(Take<Rgba32>);
			}
			else
			{
				_sawmill.Info("Can't take no-UI screenshot: current state is not GameScreen");
			}
		}, (StateInputCmdDelegate)null, true, true));
	}

	private async void Take<T>(Image<T> screenshot) where T : unmanaged, IPixel<T>
	{
		string time = DateTime.Now.ToString("yyyy-M-dd_HH.mm.ss");
		if (!_resourceManager.UserData.IsDir(BaseScreenshotPath))
		{
			_resourceManager.UserData.CreateDir(BaseScreenshotPath);
		}
		for (int i = 0; i < 5; i++)
		{
			try
			{
				string filename = time;
				if (i != 0)
				{
					filename = $"{filename}-{i}";
				}
				Stream file = _resourceManager.UserData.Open(BaseScreenshotPath / (filename + ".png"), FileMode.CreateNew, FileAccess.Write, FileShare.None);
				try
				{
					await Task.Run(delegate
					{
						ImageExtensions.SaveAsPng((Image)(object)screenshot, file);
					});
					_sawmill.Info("Screenshot taken as {0}.png", new object[1] { filename });
					return;
				}
				finally
				{
					if (file != null)
					{
						await file.DisposeAsync();
					}
				}
			}
			catch (IOException ex)
			{
				_sawmill.Warning("Failed to save screenshot, retrying?:\n{0}", new object[1] { ex });
			}
		}
		_sawmill.Error("Unable to save screenshot.");
	}
}
