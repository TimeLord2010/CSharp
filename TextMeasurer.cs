using System.Drawing;
using System.Windows.Forms;

class TextMeasurer {

    public string FontName;
    public float FontSize;

    public TextMeasurer (string fontName, float fontSize) {
        FontName = fontName;
        FontSize = fontSize;
    }

    public Size Measure (string text) {
        using (var font = new Font(FontName, FontSize)) {
            return Measure(text, font);
        }
    }

    public static Size Measure (string text, Font font) {

        return TextRenderer.MeasureText(text, font);
    }

}