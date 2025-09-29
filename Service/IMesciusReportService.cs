using System.Collections.Generic;
using Plantpedia.DTO;
using Plantpedia.Enum;

namespace Plantpedia.Service
{
    public interface IMesciusReportService
    {
        byte[] ExportPlantReport(IEnumerable<PlantDto> data);
    }
}
