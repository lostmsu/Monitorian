using System;
using System.Diagnostics;
using System.Windows.Input;

using Gma.System.MouseKeyHook;

using Monitorian.Core.Collections;
using Monitorian.Core.Models.Monitor;

namespace Monitorian.Core.Models;

public class HotkeyService
{
	readonly IValueProvider<IMonitor> _currentMonitor;
	readonly IKeyboardEvents _keyboardEvents;

	public HotkeyService(IValueProvider<IMonitor> currentMonitor)
	{
		_currentMonitor = currentMonitor ?? throw new ArgumentNullException(nameof(currentMonitor));

		_keyboardEvents = Hook.GlobalEvents();
		_keyboardEvents.KeyDown += OnKeyDown;
	}

	private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	{
		ModifierKeys modifiers = GetKeyboardModifiers();
		Key key = KeyInterop.KeyFromVirtualKey((int)e.KeyData);
		if (key == Key.None)
			key = KeyInterop.KeyFromVirtualKey((int)e.KeyCode);

		switch (key)
		{
			case Key.PageUp when modifiers == ModifierKeys.Windows:
			case Key.PageDown when modifiers == ModifierKeys.Windows:
				{
					bool increase = key == Key.PageUp;
					if (_currentMonitor.Value is { } monitor)
					{
						int brightness = monitor.Brightness;
						int newBrightness = increase ? brightness + 1 : brightness - 1;
						newBrightness = Math.Max(0, Math.Min(100, newBrightness));
						monitor.SetBrightness(newBrightness);
						Debug.WriteLine($"{monitor.Description} brightness: {brightness} -> {newBrightness}");
						e.Handled = true;
					}
					else
					{
						Debug.WriteLine("ignoring brightness change (no monitor)");
					}
				}
				break;

			case Key.PageUp when modifiers == (ModifierKeys.Windows | ModifierKeys.Control):
			case Key.PageDown when modifiers == (ModifierKeys.Windows | ModifierKeys.Control):
				{
					bool increase = key == Key.PageUp;
					if (_currentMonitor.Value is { } monitor)
					{
						int contrast = monitor.Contrast;
						if (contrast < 0)
						{
							monitor.UpdateContrast();
							contrast = monitor.Contrast;
						}
						int newContrast = increase ? contrast + 1 : contrast - 1;
						newContrast = Math.Max(0, Math.Min(100, newContrast));
						monitor.SetContrast(newContrast);
						Debug.WriteLine($"{monitor.Description} contrast: {contrast} -> {newContrast}");
						e.Handled = true;
					}
					else
					{
						Debug.WriteLine("ignoring contrast change (no monitor)");
					}
				}
				break;
		}
	}

	static ModifierKeys GetKeyboardModifiers()
			=> Keyboard.Modifiers | (IsWinDown() ? ModifierKeys.Windows : ModifierKeys.None);

	static bool IsWinDown() => Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin);
}
