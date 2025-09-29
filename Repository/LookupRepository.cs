using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.Helper;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public class LookupRepository : ILookupRepository
    {
        private readonly AppDbContext _context;

        public LookupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlantType>> GetAllPlantTypesAsync()
        {
            try
            {
                var data = await _context
                    .PlantTypes.AsNoTracking()
                    .OrderBy(pt => pt.TypeName)
                    .ToListAsync();
                LoggerHelper.Info($"Lấy thành công {data.Count} loại cây trồng từ database.");
                return data;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("Lỗi khi lấy danh sách loại cây trồng từ database.");
                LoggerHelper.Error(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Region>> GetAllRegionsAsync()
        {
            try
            {
                var data = await _context.Regions.AsNoTracking().OrderBy(r => r.Name).ToListAsync();
                LoggerHelper.Info($"Lấy thành công {data.Count} vùng miền từ database.");
                return data;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("Lỗi khi lấy danh sách vùng miền từ database.");
                LoggerHelper.Error(ex);
                throw;
            }
        }

        public async Task<IEnumerable<SoilType>> GetAllSoilTypesAsync()
        {
            try
            {
                var data = await _context
                    .SoilTypes.AsNoTracking()
                    .OrderBy(s => s.Name)
                    .ToListAsync();
                LoggerHelper.Info($"Lấy thành công {data.Count} loại đất từ database.");
                return data;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("Lỗi khi lấy danh sách loại đất từ database.");
                LoggerHelper.Error(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Climate>> GetAllClimatesAsync()
        {
            try
            {
                var data = await _context
                    .Climates.AsNoTracking()
                    .OrderBy(c => c.Name)
                    .ToListAsync();
                LoggerHelper.Info($"Lấy thành công {data.Count} loại khí hậu từ database.");
                return data;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("Lỗi khi lấy danh sách khí hậu từ database.");
                LoggerHelper.Error(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Usage>> GetAllUsagesAsync()
        {
            try
            {
                var data = await _context.Usages.AsNoTracking().OrderBy(u => u.Name).ToListAsync();
                LoggerHelper.Info($"Lấy thành công {data.Count} mục đích sử dụng từ database.");
                return data;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("Lỗi khi lấy danh sách mục đích sử dụng từ database.");
                LoggerHelper.Error(ex);
                throw;
            }
        }
    }
}
