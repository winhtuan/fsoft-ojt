using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.DTO;
using Plantpedia.Extensions;
using Plantpedia.Helper;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public class PlantRepository : IPlantRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PlantRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlantDto>> GetAllAsync()
        {
            LoggerHelper.Info("Bắt đầu lấy tất cả cây trồng.");
            try
            {
                var plants = await _context
                    .PlantInfos.ProjectTo<PlantDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                LoggerHelper.Info($"Lấy thành công {plants.Count} cây trồng.");
                return plants;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Đã xảy ra lỗi khi lấy tất cả cây trồng.");
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
            LoggerHelper.Info($"Bắt đầu lọc cây trồng với từ khóa: '{keywords}'.");
            try
            {
                var query = _context.PlantInfos.AsQueryable();

                query = query
                    .WhereIf(!string.IsNullOrEmpty(plantTypeId), p => p.PlantTypeId == plantTypeId)
                    .WhereIf(
                        !string.IsNullOrEmpty(regionId),
                        p => p.PlantRegions.Any(pr => pr.RegionId == regionId)
                    )
                    .WhereIf(
                        !string.IsNullOrEmpty(climateId),
                        p => p.PlantClimates.Any(pc => pc.ClimateId == climateId)
                    )
                    .WhereIf(
                        !string.IsNullOrEmpty(usageId),
                        p => p.PlantUsages.Any(pu => pu.UsageId == usageId)
                    )
                    .WhereIf(
                        !string.IsNullOrEmpty(soilTypeId),
                        p => p.PlantSoils.Any(ps => ps.SoilTypeId == soilTypeId)
                    );

                query = ApplyKeywordSearch(query, keywords);

                var result = await query
                    .ProjectTo<PlantDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                LoggerHelper.Info($"Tìm thấy {result.Count} cây trồng phù hợp với bộ lọc.");
                return result;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Đã xảy ra lỗi khi lọc cây trồng.");
                throw;
            }
        }

        private IQueryable<PlantInfo> ApplyKeywordSearch(
            IQueryable<PlantInfo> query,
            string? keywords
        )
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                return query;
            }

            var searchTerms = keywords.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var term in searchTerms)
            {
                var searchTerm = term.ToLower();
                query = query.Where(p =>
                    (p.ScientificName != null && p.ScientificName.ToLower().Contains(searchTerm))
                    || (p.CommonName != null && p.CommonName.ToLower().Contains(searchTerm))
                    || (p.Description != null && p.Description.ToLower().Contains(searchTerm))
                    || (p.PlantType != null && p.PlantType.TypeName.ToLower().Contains(searchTerm))
                    || p.PlantRegions.Any(pr => pr.Region.Name.ToLower().Contains(searchTerm))
                    || p.PlantUsages.Any(pu => pu.Usage.Name.ToLower().Contains(searchTerm))
                );
            }

            return query;
        }

        public async Task<PlantDto> GetPlantDetailAsync(string plantId)
        {
            LoggerHelper.Info($"Bắt đầu lấy chi tiết cây trồng với ID: {plantId}.");
            try
            {
                var plant = await _context
                    .PlantInfos.Where(p => p.PlantId == plantId)
                    .ProjectTo<PlantDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (plant == null)
                {
                    LoggerHelper.Warn(
                        $"Không tìm thấy cây trồng với ID: {plantId}. Trả về một DTO rỗng."
                    );
                    return new PlantDto();
                }

                LoggerHelper.Info($"Lấy chi tiết thành công cho cây trồng ID: {plantId}.");
                return plant;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"Đã xảy ra lỗi khi lấy chi tiết cho cây trồng ID: {plantId}."
                );
                throw;
            }
        }

        public async Task<PlantInfo?> GetByIdForUpdateAsync(string plantId)
        {
            LoggerHelper.Info($"Bắt đầu lấy cây trồng để cập nhật với ID: {plantId}.");
            try
            {
                var plant = await _context
                    .PlantInfos.Include(p => p.CareInfo)
                    .Include(p => p.PlantRegions)
                    .Include(p => p.PlantSoils)
                    .Include(p => p.PlantClimates)
                    .Include(p => p.PlantUsages)
                    .Include(p => p.PlantImages)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.PlantId == plantId);

                if (plant == null)
                {
                    LoggerHelper.Warn($"Không tìm thấy cây trồng để cập nhật với ID: {plantId}.");
                }
                else
                {
                    LoggerHelper.Info($"Lấy thành công cây trồng để cập nhật với ID: {plantId}.");
                }
                return plant;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"Đã xảy ra lỗi khi lấy cây trồng để cập nhật với ID: {plantId}."
                );
                throw;
            }
        }

        public async Task CreateAsync(PlantInfo plant)
        {
            LoggerHelper.Info($"Bắt đầu tạo cây trồng mới với ID: {plant.PlantId}.");
            try
            {
                await _context.PlantInfos.AddAsync(plant);
                await _context.SaveChangesAsync();
                LoggerHelper.Info($"Tạo thành công cây trồng với ID: {plant.PlantId}.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"Đã xảy ra lỗi khi tạo cây trồng với ID: {plant.PlantId}.");
                throw;
            }
        }

        public async Task UpdateAsync(PlantInfo plant)
        {
            LoggerHelper.Info($"Bắt đầu lưu các thay đổi cho cây trồng ID: {plant.PlantId}.");
            try
            {
                await _context.SaveChangesAsync();
                LoggerHelper.Info($"Lưu thay đổi thành công cho cây trồng ID: {plant.PlantId}.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"Đã xảy ra lỗi khi cập nhật cây trồng ID: {plant.PlantId}."
                );
                throw;
            }
        }

        public async Task<PlantInfo?> GetByIdAsync(string id)
        {
            LoggerHelper.Info($"Bắt đầu tìm cây trồng theo ID: {id}.");
            try
            {
                var plant = await _context.PlantInfos.FindAsync(id);
                if (plant == null)
                {
                    LoggerHelper.Warn($"Không tìm thấy cây trồng với ID: {id}.");
                }
                else
                {
                    LoggerHelper.Info($"Tìm thấy thành công cây trồng với ID: {id}.");
                }
                return plant;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"Đã xảy ra lỗi khi tìm cây trồng theo ID: {id}.");
                throw;
            }
        }

        public async Task DeleteAsync(PlantInfo plant)
        {
            if (plant == null)
            {
                LoggerHelper.Error("Đã cố gắng xóa một đối tượng cây trồng bị null.");
                throw new ArgumentNullException(nameof(plant));
            }

            LoggerHelper.Info($"Bắt đầu xóa cây trồng với ID: {plant.PlantId}.");
            try
            {
                _context.PlantInfos.Remove(plant);
                await _context.SaveChangesAsync();
                LoggerHelper.Info($"Xóa thành công cây trồng với ID: {plant.PlantId}.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"Đã xảy ra lỗi khi xóa cây trồng với ID: {plant.PlantId}.");
                throw;
            }
        }
    }
}
