using iTextSharp.text;

namespace ExportReport.Models
{
    public class PageSizePDF
    {
        public static iTextSharp.text.Rectangle GetPageSize(string tamañoPagina)
        {
            switch (tamañoPagina.ToLower())
            {
                case "a5-vertical":
                    return PageSize.A5;
                case "a4-vertical":
                    return PageSize.A4;
                case "a3-vertical":
                    return PageSize.A3;
                case "a2-vertical":
                    return PageSize.A2;
                case "a1-vertical":
                    return PageSize.A1;
                case "letter-vertical":
                    return PageSize.LETTER;
                case "tabloid-vertical":
                    return PageSize.TABLOID;
                case "a5-horizontal":
                    return PageSize.A5.Rotate();
                case "a4-horizontal":
                    return PageSize.A4.Rotate();
                case "a3-horizontal":
                    return PageSize.A3.Rotate();
                case "a2-horizontal":
                    return PageSize.A2.Rotate();
                case "a1-horizontal":
                    return PageSize.A1.Rotate();
                case "letter-horizontal":
                    return PageSize.LETTER.Rotate();
                case "tabloid-horizontal":
                    return PageSize.TABLOID.Rotate();
                default:
                    return PageSize.A4;
            }
        }
    }
}
