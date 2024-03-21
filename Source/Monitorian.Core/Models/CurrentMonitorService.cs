#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Monitorian.Core.Collections;
using Monitorian.Core.Models.Monitor;

using PInvoke;

using static PInvoke.User32;

namespace Monitorian.Core.Models;

internal class CurrentMonitorService(IEnumerable<IMonitor> monitors) : IValueProvider<IMonitor?>
{
	readonly IEnumerable<IMonitor> _monitors = monitors ?? throw new ArgumentNullException(nameof(monitors));

	public IMonitor? Value
	{
		get
		{
			var foreground = GetForegroundWindow();
			if (foreground != IntPtr.Zero)
			{
				if (foreground == GetDesktopWindow() || foreground == GetShellWindow())
					foreground = IntPtr.Zero;
			}

			POINT point;
			var info = new WINDOWINFO();
			if (GetWindowInfo(foreground, ref info)
				&& (info.rcWindow.right - info.rcWindow.left > 4)
				&& (info.rcWindow.bottom - info.rcWindow.top > 4))
			{
				point = new()
				{
					x = (info.rcWindow.left + info.rcWindow.right) / 2,
					y = (info.rcWindow.top + info.rcWindow.bottom) / 2,
				};
			}
			else
			{
				GetCursorPos(out point);
			}

			foreach (var monitor in _monitors)
			{
				if (monitor.IsReachable && monitor.MonitorRect.Contains(point.x, point.y))
					return monitor;
			}

			return _monitors.FirstOrDefault();
		}
	}
}
