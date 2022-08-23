using System.Drawing;
using System.Text;
using ReClassNET.Controls;

namespace Xbox360Plugin.TextPointers
{
	public class Utf16TextPtr32Node : BaseTextPtr32Node
	{
		public override Encoding Encoding => Encoding.Unicode;

		public override void GetUserInterfaceInfo(out string name, out Image icon)
		{
			name = "UTF16 / Unicode Text Pointer32";
			icon = Properties.Resources.B16x16_Button_UText_Pointer;
		}

		public override Size Draw(DrawContext context, int x, int y)
		{
			return DrawText(context, x, y, "Text16Ptr32");
		}
	}
}
