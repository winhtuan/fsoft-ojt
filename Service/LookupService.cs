using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Repository;
using Plantpedia.ViewModel;

namespace Plantpedia.Service
{
    public class LookupService : ILookupService
    {
        private readonly ILookupRepository _lookupRepository;

        public LookupService(ILookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public async Task<IEnumerable<LookupItemDto>> GetPlantTypesAsync()
        {
            LoggerHelper.Info("Attempting to retrieve all plant types.");
            try
            {
                var plantTypes = await _lookupRepository.GetAllPlantTypesAsync();
                var dtos = plantTypes
                    .Select(pt => new LookupItemDto { Id = pt.PlantTypeId, Name = pt.TypeName })
                    .ToList();

                LoggerHelper.Info($"Successfully retrieved {dtos.Count} plant types.");
                return dtos;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "An error occurred while retrieving plant types.");
                throw;
            }
        }

        public async Task<IEnumerable<LookupItemDto>> GetRegionsAsync()
        {
            LoggerHelper.Info("Attempting to retrieve all regions.");
            try
            {
                var regions = await _lookupRepository.GetAllRegionsAsync();
                var dtos = regions
                    .Select(r => new LookupItemDto { Id = r.RegionId, Name = r.Name })
                    .ToList();
                LoggerHelper.Info($"Successfully retrieved {dtos.Count} regions.");
                return dtos;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "An error occurred while retrieving regions.");
                throw;
            }
        }

        public async Task<IEnumerable<LookupItemDto>> GetSoilTypesAsync()
        {
            LoggerHelper.Info("Attempting to retrieve all soil types.");
            try
            {
                var soilTypes = await _lookupRepository.GetAllSoilTypesAsync();
                var dtos = soilTypes
                    .Select(s => new LookupItemDto { Id = s.SoilTypeId, Name = s.Name })
                    .ToList();
                LoggerHelper.Info($"Successfully retrieved {dtos.Count} soil types.");
                return dtos;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "An error occurred while retrieving soil types.");
                throw;
            }
        }

        public async Task<IEnumerable<LookupItemDto>> GetClimatesAsync()
        {
            LoggerHelper.Info("Attempting to retrieve all climates.");
            try
            {
                var climates = await _lookupRepository.GetAllClimatesAsync();
                var dtos = climates
                    .Select(c => new LookupItemDto { Id = c.ClimateId, Name = c.Name })
                    .ToList();
                LoggerHelper.Info($"Successfully retrieved {dtos.Count} climates.");
                return dtos;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "An error occurred while retrieving climates.");
                throw;
            }
        }

        public async Task<IEnumerable<LookupItemDto>> GetUsagesAsync()
        {
            LoggerHelper.Info("Attempting to retrieve all usages.");
            try
            {
                var usages = await _lookupRepository.GetAllUsagesAsync();
                var dtos = usages
                    .Select(u => new LookupItemDto { Id = u.UsageId, Name = u.Name })
                    .ToList();
                LoggerHelper.Info($"Successfully retrieved {dtos.Count} usages.");
                return dtos;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "An error occurred while retrieving usages.");
                throw;
            }
        }

        public async Task<List<DropdownViewModel>> GetAllFilterDropdownsAsync()
        {
            LoggerHelper.Info("Bắt đầu chuẩn bị tất cả filter dropdowns (chạy tuần tự).");
            try
            {
                var plantTypes = await GetPlantTypesAsync();
                var soilTypes = await GetSoilTypesAsync();
                var regions = await GetRegionsAsync();
                var climates = await GetClimatesAsync();
                var usages = await GetUsagesAsync();

                var dropdowns = new List<DropdownViewModel>
                {
                    new()
                    {
                        Name = "plantTypeId",
                        Placeholder = "Chọn loại cây",
                        Options = plantTypes.ToSelectListItem(dto => dto.Id!, dto => dto.Name!),
                    },
                    new()
                    {
                        Name = "soilTypeId",
                        Placeholder = "Chọn loại đất",
                        Options = soilTypes.ToSelectListItem(dto => dto.Id!, dto => dto.Name!),
                    },
                    new()
                    {
                        Name = "regionId",
                        Placeholder = "Chọn khu vực",
                        Options = regions.ToSelectListItem(dto => dto.Id!, dto => dto.Name!),
                    },
                    new()
                    {
                        Name = "climateId",
                        Placeholder = "Chọn khí hậu",
                        Options = climates.ToSelectListItem(dto => dto.Id!, dto => dto.Name!),
                    },
                    new()
                    {
                        Name = "usageId",
                        Placeholder = "Chọn công dụng",
                        Options = usages.ToSelectListItem(dto => dto.Id!, dto => dto.Name!),
                    },
                };

                LoggerHelper.Info("Chuẩn bị filter dropdowns thành công.");
                return dropdowns;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Xảy ra lỗi khi chuẩn bị filter dropdowns.");
                throw;
            }
        }
    }
}
