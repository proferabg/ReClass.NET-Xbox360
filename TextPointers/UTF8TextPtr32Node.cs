using System.Drawing;
using System.Text;
using ReClassNET.Controls;

namespace Xbox360Plugin.TextPointers
{
	public class Utf8TextPtr32Node : BaseTextPtr32Node
	{
		public override Encoding Encoding => Encoding.UTF8;

		public override void GetUserInterfaceInfo(out string name, out Image icon)
		{
			name = "UTF8 / ASCII Text Pointer32";
			icon = Properties.Resources.B16x16_Button_Text_Pointer;
		}

		public override Size Draw(DrawContext context, int x, int y)
		{
			return DrawText(context, x, y, "Text8Ptr32");
		}
	}
}
