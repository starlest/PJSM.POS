namespace PUJASM.POS.Utilities
{
    using System.Drawing;
    using System.Drawing.Printing;
    using Models.Sales;

    public class ReceiptPrinter
    {
        private readonly SalesTransaction _salesTranscation;

        private readonly StringFormat drawFormatCenter = new StringFormat {Alignment = StringAlignment.Center};
        private readonly StringFormat drawFormatLeft = new StringFormat {Alignment = StringAlignment.Near};
        private readonly StringFormat drawFormatRight = new StringFormat {Alignment = StringAlignment.Far};

        private readonly Font drawFontArial12Bold = new Font("Arial", 12, FontStyle.Bold);
        private readonly Font drawFontArial6Bold = new Font("Arial", 6, FontStyle.Bold);
        private readonly Font drawFontArial6Regular = new Font("Arial", 6, FontStyle.Regular);
        private readonly SolidBrush drawBrush = new SolidBrush(Color.Black);

        private const float x = 0f;
        private float y;
        private const float width = 270.0F; // max width I found through trial and error
        private const float height = 0F;

        public ReceiptPrinter(SalesTransaction salesTransaction)
        {
            _salesTranscation = salesTransaction;
        }

        public void PrintReceiptPage(object sender, PrintPageEventArgs e)
        {
            y = 0;
            PrintTitle(e);
            PrintHeaders(e);
            PrintLines(e);
            PrintFooter(e);
        }

        private void PrintTitle(PrintPageEventArgs e)
        {
            var text = "Puja Supermarket";
            e.Graphics.DrawString(text, drawFontArial12Bold, drawBrush, new RectangleF(x, y, width, height),
                drawFormatCenter);
            y += e.Graphics.MeasureString(text, drawFontArial12Bold).Height;

            text = "Jln. Telaga Biru";
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x, y, width, height),
                drawFormatCenter);

            y += 40;
        }

        private void PrintHeaders(PrintPageEventArgs e)
        {
            var text = "Barang";
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x, y, 80, height),
                drawFormatLeft);

            text = "Qty";
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x + 80, y, 30, height),
                drawFormatCenter);

            text = "Hrg";
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x + 110, y, 50, height),
                drawFormatCenter);

            text = "Dis";
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x + 160, y, 50, height),
                drawFormatCenter);

            text = "Tot";
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x + 210, y, 50, height),
                drawFormatCenter);

            y += 20;
        }

        private void PrintLines(PrintPageEventArgs e)
        {
            foreach (var line in _salesTranscation.SalesTransactionLines)
            {
                var text = line.Item.Name;
                var textArea = new RectangleF(x, y, 80, height);
                e.Graphics.DrawString(text, drawFontArial6Regular, drawBrush, textArea,
                    drawFormatLeft);
                var lineHeight = e.Graphics.MeasureString(text, drawFontArial6Regular, textArea.Size).Height;

                text = line.Quantity.ToString();
                textArea = new RectangleF(x + 80, y, 30, height);
                e.Graphics.DrawString(text, drawFontArial6Regular, drawBrush, textArea,
                    drawFormatCenter);
                if (e.Graphics.MeasureString(text, drawFontArial6Regular).Height > lineHeight)
                    lineHeight = e.Graphics.MeasureString(text, drawFontArial6Regular, textArea.Size).Height;

                text = line.SalesPrice.ToString("#");
                textArea = new RectangleF(x + 110, y, 50, height);
                e.Graphics.DrawString(text, drawFontArial6Regular, drawBrush, textArea,
                    drawFormatCenter);
                if (e.Graphics.MeasureString(text, drawFontArial6Regular).Height > lineHeight)
                    lineHeight = e.Graphics.MeasureString(text, drawFontArial6Regular, textArea.Size).Height;

                text = line.Discount.ToString("#");
                textArea = new RectangleF(x + 160, y, 50, height);
                e.Graphics.DrawString(text, drawFontArial6Regular, drawBrush, textArea,
                    drawFormatCenter);
                if (e.Graphics.MeasureString(text, drawFontArial6Regular).Height > lineHeight)
                    lineHeight = e.Graphics.MeasureString(text, drawFontArial6Regular, textArea.Size).Height;

                text = line.Total.ToString("#");
                textArea = new RectangleF(x + 210, y, 50, height);
                e.Graphics.DrawString(text, drawFontArial6Regular, drawBrush, textArea,
                    drawFormatCenter);
                if (e.Graphics.MeasureString(text, drawFontArial6Regular).Height > lineHeight)
                    lineHeight = e.Graphics.MeasureString(text, drawFontArial6Regular, textArea.Size).Height;

                y += lineHeight + 10;
            }

            y += 20;
        }

        private void PrintFooter(PrintPageEventArgs e)
        {
            var text = "Gross Total: " + _salesTranscation.GrossTotal.ToString("#");
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x, y, width, height),
                drawFormatLeft);
            y += e.Graphics.MeasureString(text, drawFontArial6Regular).Height;

            text = "Discount: " + _salesTranscation.Discount.ToString("#");
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x, y, width, height),
                drawFormatLeft);
            y += e.Graphics.MeasureString(text, drawFontArial6Regular).Height;

            text = "Jumlah: " + _salesTranscation.Total.ToString("#");
            e.Graphics.DrawString(text, drawFontArial6Bold, drawBrush, new RectangleF(x, y, width, height),
                drawFormatLeft);
            y += e.Graphics.MeasureString(text, drawFontArial6Regular).Height;
        }

    }
}
