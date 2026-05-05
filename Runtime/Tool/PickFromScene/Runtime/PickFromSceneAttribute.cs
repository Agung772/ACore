using System;
namespace ACore
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PickFromSceneAttribute : Attribute{
        /// <param name="label"></param>
        /// <param name="usePathAsLabel">path of property in serialize tree</param>
        /// <param name="handleColor">Supports a variety of color formats, including named colors (e.g. "red", "orange", "green", "blue"), hex codes (e.g. "#FF0000" and "#FF0000FF"), and RGBA (e.g. "RGBA(1,1,1,1)") or RGB (e.g. "RGB(1,1,1)"), including Odin attribute expressions (e.g "@this.MyColor"). Here are the available named colors: black, blue, clear, cyan, gray, green, grey, magenta, orange, purple, red, transparent, transparentBlack, transparentWhite, white, yellow, lightblue, lightcyan, lightgray, lightgreen, lightgrey, lightmagenta, lightorange, lightpurple, lightred, lightyellow, darkblue, darkcyan, darkgray, darkgreen, darkgrey, darkmagenta, darkorange, darkpurple, darkred, darkyellow.</param>
        public PickFromSceneAttribute(string label = "", bool usePathAsLabel = false, string handleColor = "white") {
            Label = label;
            UsePathAsAsLabel = usePathAsLabel;
            HandleColor = handleColor;
        }
        public string Label { get; }
        public bool UsePathAsAsLabel { get; }
        public string HandleColor { get; }
    }
}