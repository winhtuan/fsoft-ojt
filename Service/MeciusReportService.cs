using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Document.Drawing;
using GrapeCity.ActiveReports.Document.Section;
using GrapeCity.ActiveReports.Export.Pdf.Section;
using GrapeCity.ActiveReports.SectionReportModel;
using Plantpedia.DTO;

namespace Plantpedia.Service
{
    public class MeciusReportService : IMesciusReportService
    {
        public byte[] ExportPlantReport(IEnumerable<PlantDto> data)
        {
            var dt = CreateGroupablePlantDataTable(data);
            dt.DefaultView.Sort = "Khu Vực ASC, [Loại cây] ASC";

            var report = new SectionReport();
            report.DataSource = dt.DefaultView;

            var pageHeader = new PageHeader();
            var ghRegion = new GroupHeader { DataField = "Khu Vực", Height = 0.3f };
            var ghPlantType = new GroupHeader { DataField = "Loại cây", Height = 0.3f };
            var detail = new Detail { Height = 0.25f };

            var gfPlantType = new GroupFooter();
            var gfRegion = new GroupFooter();
            var pageFooter = new PageFooter();

            var lblHeader = new Label
            {
                Text = "BÁO CÁO CÂY TRỒNG",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Width = 7f,
                Alignment = TextAlignment.Center,
            };
            pageHeader.Controls.Add(lblHeader);

            var txtRegion = new TextBox
            {
                DataField = "Khu Vực",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Width = 7.5f,
                Left = 0f,
                Top = 0.05f,
            };
            ghRegion.Controls.Add(txtRegion);

            var txtPlantType = new TextBox
            {
                DataField = "Loại cây",
                Font = new Font("Arial", 10, FontStyle.Italic),
                Width = 7f,
                Left = 0.3f,
                Top = 0.05f,
            };
            ghPlantType.Controls.Add(txtPlantType);

            var txtPlantDetail = new TextBox
            {
                DataField = "LeafDisplay",
                Font = new Font("Arial", 10),
                Width = 6.5f,
                Left = 0.6f,
                Top = 0,
            };
            detail.Controls.Add(txtPlantDetail);

            report.Sections.Add(pageHeader);
            report.Sections.Add(ghRegion);
            report.Sections.Add(ghPlantType);
            report.Sections.Add(detail);
            report.Sections.Add(gfPlantType);
            report.Sections.Add(gfRegion);
            report.Sections.Add(pageFooter);

            report.Run();

            using var ms = new MemoryStream();
            var pdfExport = new PdfExport();
            pdfExport.Export(report.Document, ms);
            return ms.ToArray();
        }

        private DataTable CreateGroupablePlantDataTable(IEnumerable<PlantDto> plants)
        {
            var dt = new DataTable();
            dt.Columns.Add("Khu Vực");
            dt.Columns.Add("Loại cây");
            dt.Columns.Add("LeafDisplay");

            foreach (var p in plants)
            {
                var regions = p.RegionNames?.ToList();
                if (regions == null || !regions.Any())
                {
                    regions = new List<string> { "(Không xác định)" };
                }

                foreach (var region in regions)
                {
                    string leafInfo =
                        $"- {p.CommonName} (ID: {p.PlantId}) - Mục đích: {string.Join(", ", p.UsageNames ?? Enumerable.Empty<string>())}";
                    dt.Rows.Add(region, p.PlantTypeName ?? "(Chưa phân loại)", leafInfo);
                }
            }
            return dt;
        }
    }
}
