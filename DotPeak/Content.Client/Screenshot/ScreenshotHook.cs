// Decompiled with JetBrains decompiler
// Type: Content.Client.Screenshot.ScreenshotHook
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Shared.ContentPack;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

#nullable enable
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
    this._sawmill = Logger.GetSawmill("screenshot");
    this._sawmill.Level = new LogLevel?((LogLevel) 2);
    // ISSUE: method pointer
    this._inputManager.SetInputCommand(ContentKeyFunctions.TakeScreenshot, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__6_0)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._inputManager.SetInputCommand(ContentKeyFunctions.TakeScreenshotNoUI, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__6_1)), (StateInputCmdDelegate) null, true, true));
  }

  private async void Take<T>(Image<T> screenshot) where T : unmanaged, IPixel<T>
  {
    string time = DateTime.Now.ToString("yyyy-M-dd_HH.mm.ss");
    if (!this._resourceManager.UserData.IsDir(ScreenshotHook.BaseScreenshotPath))
      this._resourceManager.UserData.CreateDir(ScreenshotHook.BaseScreenshotPath);
    for (int i = 0; i < 5; ++i)
    {
      try
      {
        string filename = time;
        if (i != 0)
          filename = $"{filename}-{i}";
        Stream file = this._resourceManager.UserData.Open(ResPath.op_Division(ScreenshotHook.BaseScreenshotPath, filename + ".png"), FileMode.CreateNew, FileAccess.Write, FileShare.None);
        object obj = (object) null;
        int num = 0;
        try
        {
          await Task.Run((Action) (() => ImageExtensions.SaveAsPng((Image) screenshot, file)));
          this._sawmill.Info("Screenshot taken as {0}.png", new object[1]
          {
            (object) filename
          });
          num = 1;
        }
        catch (object ex)
        {
          obj = ex;
        }
        if (file != null)
          await file.DisposeAsync();
        object obj1 = obj;
        if (obj1 != null)
        {
          if (!(obj1 is Exception source))
            throw obj1;
          ExceptionDispatchInfo.Capture(source).Throw();
        }
        if (num == 1)
          return;
        obj = (object) null;
        filename = (string) null;
      }
      catch (IOException ex)
      {
        this._sawmill.Warning("Failed to save screenshot, retrying?:\n{0}", new object[1]
        {
          (object) ex
        });
      }
    }
    this._sawmill.Error("Unable to save screenshot.");
  }
}
