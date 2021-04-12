using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetBarcode;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Blueprint.GenerateBarCodeInExcel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private const int _Width = 290;
        private const int _Height = 120;
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult GetBarCodeWithNetBarcode(string input)
        {
            // Using NetBarcode
            _logger.LogInformation($"Call to {nameof(GetBarCodeWithNetBarcode)}");

            var barcode = new Barcode(input, Type.Code128, true);
            var bytes = barcode.GetByteArray(ImageFormat.Png);
            return File(bytes, "image/png");
        }

        /// <summary>
        /// Document: https://github.com/barnhill/barcodelib
        /// BarCode: https://en.wikipedia.org/wiki/Barcode
        /// Returns image: https://stackoverflow.com/questions/39177576/how-to-to-return-an-image-with-web-api-get-method/39177684
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet("barcodelib")]
        public IActionResult GetBarCodeWithBarcodeLib(string input)
        {
            // Using NetBarcode
            _logger.LogInformation($"Call to {nameof(GetBarCodeWithBarcodeLib)}");

            if (input.Length < 11 || input.Length > 12)
            {
                input = "ProductUpc";
            }

            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
            barcode.IncludeLabel = true;
            Image img = barcode.Encode(BarcodeLib.TYPE.UPCA, input, Color.Black, Color.White, 290, 120);
            return File(ImageToByteArray(img), "image/png");
        }

        /// <summary>
        /// Export excel: https://www.infoworld.com/article/3538413/how-to-export-data-to-excel-in-aspnet-core-30.html
        /// </summary>
        /// <returns></returns>
        [HttpGet("excel-with-barcode")]
        public IActionResult GetExcelWithBarCode()
        {
            // Using NetBarcode
            _logger.LogInformation($"Call to {nameof(GetExcelWithBarCode)}");

            // Exports excel
            string fileName = "OrderDetail.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                ExportOrderDetailToExcel(GetOrderSummaryReport(), workbook);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }

        public byte[] ImageToByteArray(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private void ExportOrderDetailToExcel(List<OrderDetailReport> report, XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add(OrderExportConstants.OrderDetailExcelName);
            var currentRow = 1;

            //Add header
            worksheet.Cell("A1").Value = OrderExportConstants.ProductUpc;
            worksheet.Cell("B1").Value = OrderExportConstants.ProductCategory;
            worksheet.Cell("C1").Value = OrderExportConstants.ProductTitle;
            worksheet.Cell("D1").Value = string.Empty;
            worksheet.Cell("E1").Value = OrderExportConstants.Dept;
            worksheet.Cell("F1").Value = OrderExportConstants.OrderNumber;
            worksheet.Cell("G1").Value = OrderExportConstants.StoreNumber;
            worksheet.Cell("H1").Value = OrderExportConstants.Options;
            worksheet.Cell("I1").Value = OrderExportConstants.Price;

            foreach (var item in report)
            {
                currentRow++;
                worksheet.Row(currentRow).Style.Alignment
                    .SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Column(1).Width = 50;
                worksheet.Row(currentRow).Height = _Height;
                // Adds image to ClosedXML: https://stackoverflow.com/questions/18661239/closedxml-add-image
                worksheet.AddPicture(GetBarCodeStream())
                         .MoveTo(worksheet.Cell(currentRow, 1), 10, 10);

                worksheet.Cell(currentRow, 2).Value = item.ProductCategory;
                worksheet.Cell(currentRow, 3).Value = item.ProductTitle;
                worksheet.Cell(currentRow, 4).Value = string.Empty;
                worksheet.Cell(currentRow, 5).Value = item.Dept;
                worksheet.Cell(currentRow, 6).SetDataType(XLDataType.Text);
                worksheet.Cell(currentRow, 6).SetValue(item.OrderNumber);
                worksheet.Cell(currentRow, 7).SetDataType(XLDataType.Text);
                worksheet.Cell(currentRow, 7).SetValue(item.StoreNumber);
                worksheet.Cell(currentRow, 8).Value = string.Empty;
                worksheet.Cell(currentRow, 9).Style.NumberFormat.SetNumberFormatId(4);
                worksheet.Cell(currentRow, 9).Value = item.Price;
            }
        }

        private List<OrderDetailReport> GetOrderSummaryReport()
        {
            return new List<OrderDetailReport>()
            {
                new OrderDetailReport()
                {
                    ProductUpc= "ProductUpc",
                    ProductCategory = "Product 001",
                    ProductTitle = "Product 001",
                    OrderNumber="Order Number 001",
                    StoreNumber = "001",
                    Price = 127
                },
                new OrderDetailReport()
                {
                    ProductUpc= "ProductUpc",
                    ProductCategory = "Product 002",
                    ProductTitle = "Product 002",
                    OrderNumber="Order Number 002",
                    StoreNumber = "002",
                    Price = 127
                },
                new OrderDetailReport()
                {
                    ProductUpc= "ProductUpc",
                    ProductCategory = "Product 003",
                    ProductTitle = "Product 003",
                    OrderNumber="Order Number 003",
                    StoreNumber = "003",
                    Price = 127
                }
            };
        }

        private Stream GetBarCodeStream()
        {
            string barCode = "ProductUpc";
            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
            barcode.IncludeLabel = true;
            Image image = barcode.Encode(BarcodeLib.TYPE.UPCA, barCode, Color.Black, Color.White, _Width, _Height);

            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            return stream;
        }
    }
}
