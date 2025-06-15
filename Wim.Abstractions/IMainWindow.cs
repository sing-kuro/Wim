using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wim.Abstractions
{
    public interface IMainWindow
    {
		/// <summary>
		/// Displays a text block with the specified ID and text at the given coordinates.
		/// Allows specifying foreground and background colors as brushes.
		/// If colors are not provided, default colors will be used.
		/// </summary>
		void ShowText(string id, string text, double x, double y,
			System.Windows.Media.Brush? foregroundColor = null,
			System.Windows.Media.Brush? backgroundColor = null,
			int size = 24, double opacity = 0.8, double padding = 5);

		/// <summary>
		/// Displays a text block with the specified ID and text at the given coordinates.
		/// Allows specifying foreground and background colors as strings.
		/// If colors are not provided, default colors will be used.
		/// </summary>
		void ShowText(string id, string text, double x, double y,
			string? foregroundColor = null, string? backgroundColor = null,
			int size = 24, double opacity = 0.8, double padding = 5);

		/// <summary>
		/// Removes a text block with the specified ID from the overlay canvas.
		/// If the text block does not exist, it does nothing.
		/// </summary>
		/// <param name="id">The ID of the text block to remove.</param>
		void RemoveText(string id);

		/// <summary>
		/// Removes all text blocks from the overlay canvas.
		/// </summary>
		void ClearTexts();
    }
}
