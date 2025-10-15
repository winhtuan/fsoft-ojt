using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Repository;
using Plantpedia.ViewModel;

namespace Plantpedia.Service
{
    public class PlantService : IPlantService
    {
        private readonly IPlantRepository _plantRepository;
        private readonly IMapper _mapper;

        public PlantService(IPlantRepository plantRepository, IMapper mapper)
        {
            _plantRepository = plantRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlantDto>> GetAllAsync()
        {
            LoggerHelper.Info("Bắt đầu lấy tất cả cây trồng từ service.");
            try
            {
                var plants = await _plantRepository.GetAllAsync();
                LoggerHelper.Info($"Lấy thành công {plants.Count()} cây trồng từ service.");
                return plants;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Đã xảy ra lỗi trong service khi lấy tất cả cây trồng.");
                throw;
            }
        }

        public async Task<List<PlantDto>> GetFilteredDtosAsync(
            string? plantTypeId,
            string? regionId,
            string? climateId,
            string? usageId,
            string? soilTypeId,
            string? keywords
        )
        {
            LoggerHelper.Info($"Service bắt đầu lọc cây trồng với từ khóa: '{keywords}'.");
            try
            {
                var result = await _plantRepository.GetFilteredDtosAsync(
                    plantTypeId,
                    regionId,
                    climateId,
                    usageId,
                    soilTypeId,
                    keywords
                );
                LoggerHelper.Info($"Service tìm thấy {result.Count} cây trồng phù hợp với bộ lọc.");
                return result;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Đã xảy ra lỗi trong service khi lọc cây trồng.");
                throw;
            }
        }

        public async Task<PlantDto> GetPlantById(string plantId)
        {
            LoggerHelper.Info($"Service bắt đầu lấy cây trồng theo ID: {plantId}.");
            try
            {
                var plant = await _plantRepository.GetPlantDetailAsync(plantId);
                if (plant == null || string.IsNullOrEmpty(plant.PlantId))
                {
                    LoggerHelper.Warn($"Service không tìm thấy cây trồng với ID: {plantId}.");
                }
                else
                {
                    LoggerHelper.Info($"Service đã lấy thành công cây trồng với ID: {plantId}.");
                }
                return plant!;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"Đã xảy ra lỗi trong service khi lấy cây trồng theo ID: {plantId}."
                );
                throw;
            }
        }

        public async Task CreatePlantAsync(UpdatePlantViewModel viewModel)
        {
            LoggerHelper.Info($"Bắt đầu tạo cây trồng mới với tên: '{viewModel.CommonName}'.");
            try
            {
                var plantEntity = _mapper.Map<PlantInfo>(viewModel);
                plantEntity.PlantId = Guid.NewGuid().ToString("N").Substring(0, 10);
                plantEntity.CreatedDate = DateTime.UtcNow;

                UpdateCollectionsFromViewModel(plantEntity, viewModel);

                await _plantRepository.CreateAsync(plantEntity);
                LoggerHelper.Info($"Tạo thành công cây trồng mới với ID: {plantEntity.PlantId}.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"Đã xảy ra lỗi khi tạo cây trồng '{viewModel.CommonName}'."
                );
                throw;
            }
        }

        public async Task UpdatePlantAsync(UpdatePlantViewModel viewModel)
        {
            LoggerHelper.Info($"Bắt đầu cập nhật cây trồng với ID: {viewModel.PlantId}.");
            try
            {
                var existingPlant = await _plantRepository.GetByIdForUpdateAsync(
                    viewModel.PlantId!
                );
                if (existingPlant == null)
                {
                    LoggerHelper.Error(
                        $"Cập nhật thất bại. Không tìm thấy cây trồng với ID: {viewModel.PlantId}."
                    );
                    throw new KeyNotFoundException(
                        $"Không tìm thấy cây trồng với ID: {viewModel.PlantId}"
                    );
                }

                _mapper.Map(viewModel, existingPlant);
                existingPlant.UpdatedDate = DateTime.UtcNow;

                UpdateCollectionsFromViewModel(existingPlant, viewModel);

                await _plantRepository.UpdateAsync(existingPlant);
                LoggerHelper.Info($"Cập nhật thành công cây trồng với ID: {viewModel.PlantId}.");
            }
            catch (KeyNotFoundException) // Bắt lại lỗi cụ thể để không log 2 lần
            {
                throw;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"Đã xảy ra lỗi không mong muốn khi cập nhật cây trồng ID: {viewModel.PlantId}."
                );
                throw;
            }
        }

        private void UpdateCollectionsFromViewModel(PlantInfo plant, UpdatePlantViewModel viewModel)
        {
            plant.PlantImages?.Clear();
            plant.PlantImages ??= new List<PlantImg>();

            if (!string.IsNullOrWhiteSpace(viewModel.ImageUrl))
            {
                plant.PlantImages.Add(
                    new PlantImg
                    {
                        ImageId = Guid.NewGuid().ToString().Substring(0, 10),
                        ImageUrl = viewModel.ImageUrl.Trim(),
                        Caption = $"Ảnh của {plant.CommonName}",
                    }
                );
            }

            plant.PlantRegions?.Clear();
            plant.PlantSoils?.Clear();
            plant.PlantClimates?.Clear();
            plant.PlantUsages?.Clear();

            plant.PlantRegions = viewModel
                .RegionIds.Select(id => new PlantRegion { RegionId = id })
                .ToList();
            plant.PlantSoils = viewModel
                .SoilTypeIds.Select(id => new PlantSoil { SoilTypeId = id })
                .ToList();
            plant.PlantClimates = viewModel
                .ClimateIds.Select(id => new PlantClimate { ClimateId = id })
                .ToList();
            plant.PlantUsages = viewModel
                .UsageIds.Select(id => new PlantUsage { UsageId = id })
                .ToList();
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            LoggerHelper.Info($"Bắt đầu xóa cây trồng với ID: {id}.");
            try
            {
                var plantToDelete = await _plantRepository.GetByIdAsync(id);

                if (plantToDelete == null)
                {
                    LoggerHelper.Warn($"Xóa thất bại. Không tìm thấy cây trồng với ID: {id}.");
                    return false;
                }

                await _plantRepository.DeleteAsync(plantToDelete);

                LoggerHelper.Info($"Đã xóa thành công cây trồng với ID: {id}.");
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"Đã xảy ra lỗi khi xóa cây trồng với ID: {id}.");
                throw;
            }
        }

        public async Task<int> GetPlantCountAsync()
        {
            return await _plantRepository.GetPlantCountAsync();
        }
    }
}
