using Wim.Abstractions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Wim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
		private readonly Dictionary<string, TextBlock> textBlocks = [];

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// Sets the window to be click-through and non-activating.
		/// </summary>
        public MainWindow()
        {
            InitializeComponent();
			MakeClickThrough();
        }

		/// <summary>
		/// Sets the window style to be click-through and non-activating.
		/// This allows the window to be transparent to mouse clicks,
		/// allowing interaction with underlying windows.
		/// </summary>
		private void MakeClickThrough()
		{
			var hwnd = new WindowInteropHelper(this).EnsureHandle();
			int exStyle = (int)GetWindowLong(hwnd, GWL_EXSTYLE);
			SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE);
		}

		/// <summary>
		/// Shows a text block with the specified ID and text at the given coordinates.
		/// Allows specifying foreground and background colors as brushes.
		/// If colors are not provided, default colors will be used.
		/// The text block is added to the overlay canvas,
		/// and if a text block with the same ID already exists,
		/// it is removed before adding the new one.
		/// </summary>
        public void ShowText(string id, string text, double x, double y,
            System.Windows.Media.Brush? foregroundColor = null,
            System.Windows.Media.Brush? backgroundColor = null,
            int size = 24, double opacity = 0.8, double padding = 5)
        {
            Dispatcher.Invoke(() =>
            {
				if (textBlocks.TryGetValue(id, out var existing))
				{
					OverlayCanvas.Children.Remove(existing);
					textBlocks.Remove(id);
				}

                var block = new TextBlock
                {
                    Text = text,
                    FontSize = size,
                    Foreground = foregroundColor ?? System.Windows.Media.Brushes.White,
                    Background = backgroundColor ?? System.Windows.Media.Brushes.Black,
                    Opacity = opacity,
                    Padding = new Thickness(padding),
                };

                Canvas.SetLeft(block, x);
                Canvas.SetTop(block, y);
				textBlocks[id] = block;
                OverlayCanvas.Children.Add(block);
            });
        }

		/// <summary>
		/// Shows a text block with the specified ID and text at the given coordinates.
		/// Allows specifying foreground and background colors as strings.
		/// If colors are not provided, default colors will be used.
		/// </summary>
		public void ShowText(string id, string text, double x, double y,
			string? foregroundColor = null, string? backgroundColor = null,
			int size = 24, double opacity = 0.8, double padding = 5)
		{
			Dispatcher.Invoke(() =>
			{
				var fgBrush = foregroundColor != null
					? (System.Windows.Media.Brush)new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(foregroundColor))
					: System.Windows.Media.Brushes.White;
				var bgBrush = backgroundColor != null
					? (System.Windows.Media.Brush)new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(backgroundColor))
					: System.Windows.Media.Brushes.Black;
				ShowText(id, text, x, y, fgBrush, bgBrush, size, opacity, padding);
			});
		}

		/// <summary>
		/// Removes a text block with the specified ID from the overlay canvas.
		/// If the text block does not exist, it does nothing.
		/// </summary>
		public void RemoveText(string id)
		{
			Dispatcher.Invoke(() =>
			{
				if (textBlocks.TryGetValue(id, out var existing))
				{
					OverlayCanvas.Children.Remove(existing);
					textBlocks.Remove(id);
				}
			});
		}

		/// <summary>
		/// Clears all text blocks from the overlay canvas.
		/// Removes all entries from the textBlocks dictionary.
		/// </summary>
		public void ClearTexts()
		{
			Dispatcher.Invoke(() =>
			{
				foreach (var block in textBlocks.Values)
				{
					OverlayCanvas.Children.Remove(block);
				}
				textBlocks.Clear();
			});
		}

#region Win32 Interop
		const int GWL_EXSTYLE = -20;
		const int WS_EX_TRANSPARENT = 0x00000020;
		const int WS_EX_NOACTIVATE = 0x08000000;

		[DllImport("user32.dll")]
		static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
#endregion
    }
}
