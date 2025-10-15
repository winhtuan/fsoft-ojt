// diagnosis-page/diagnosisApi.js - Xử lý tất cả các tương tác với API
export class DiagnosisApi {
  constructor() {
    this.apiUrl = "/api/PlantDiagnosis/detect";
  }

  /**
   * Gửi ảnh lên server để chẩn đoán
   */
  async diagnose(file) {
    try {
      const formData = new FormData();
      formData.append("image", file);

      const response = await fetch(this.apiUrl, {
        method: "POST",
        body: formData,
      });

      // Parse JSON response
      let json;
      try {
        json = await response.json();
      } catch (error) {
        throw new Error("Lỗi định dạng dữ liệu trả về từ server!");
      }

      // Kiểm tra response có thành công không
      if (!response.ok || json.success === false) {
        throw new Error(json.message || "Đã xảy ra lỗi khi chẩn đoán!");
      }

      return json.data || json;
    } catch (error) {
      if (error.message) {
        throw error;
      }
      throw new Error("Không thể kết nối đến server!");
    }
  }

  /**
   * Parse và xử lý dữ liệu trả về từ API
   */
  parseResult(data) {
    // Xử lý trường hợp API trả về mảng
    if (Array.isArray(data)) {
      data = data[0];
    }

    const result = data?.result || data;

    if (!result) {
      return {
        success: false,
        message: "Không có dữ liệu kết quả.",
      };
    }

    // Kiểm tra có phải cây trồng không
    const isPlantCheck = this._checkIsPlant(result);
    if (!isPlantCheck.isPlant) {
      return {
        success: false,
        message: "Không nhận diện được cây trồng.",
      };
    }

    // Trích xuất thông tin
    return {
      success: true,
      data: {
        isHealthy: this._getHealthStatus(result),
        species: this._getSpeciesInfo(result),
        diseases: this._getDiseases(result, data),
      },
    };
  }

  /**
   * Kiểm tra có phải cây trồng không
   */
  _checkIsPlant(result) {
    let isPlant = null;

    if (typeof result.is_plant === "boolean") {
      isPlant = result.is_plant;
    } else if (result.is_plant && typeof result.is_plant === "object") {
      isPlant = result.is_plant.binary === true;
    } else if (typeof result.is_plant_probability === "number") {
      isPlant = result.is_plant_probability >= 0.5;
    } else if (result.is_healthy !== undefined) {
      isPlant = true;
    }

    return { isPlant: isPlant === true };
  }

  /**
   * Lấy trạng thái sức khỏe
   */
  _getHealthStatus(result) {
    if (result.is_healthy?.binary === true) {
      return { healthy: true, message: "Cây có vẻ khỏe mạnh tổng thể." };
    } else if (result.is_healthy?.binary === false) {
      return { healthy: false, message: "Cây có dấu hiệu không khỏe." };
    }
    return null;
  }

  /**
   * Lấy thông tin loài
   */
  _getSpeciesInfo(result) {
    const suggestions =
      result.classification?.suggestions || result.suggestions || [];
    if (suggestions.length === 0) return null;

    const top = suggestions[0];
    return {
      scientificName:
        top.details?.scientific_name || top.scientific_name || "(không có)",
      commonName:
        (top.details?.common_names && top.details.common_names[0]) ||
        top.name ||
        "(không có)",
      probability:
        top.probability != null ? Math.round(top.probability * 100) : null,
      alternatives: suggestions.slice(1, 4).map((s) => ({
        name:
          s.details?.scientific_name || s.scientific_name || s.name || "N/A",
        probability:
          s.probability != null ? Math.round(s.probability * 100) : null,
      })),
    };
  }

  /**
   * Lấy danh sách bệnh
   */
  _getDiseases(result, data) {
    let diseases =
      result.disease?.suggestions ||
      result.health_assessment?.diseases ||
      result.diseases ||
      data?.health_assessment?.diseases ||
      [];

    if (diseases.length === 0) return [];

    // Sắp xếp theo xác suất và lấy top 5
    return diseases
      .slice()
      .sort((a, b) => (b.probability ?? 0) - (a.probability ?? 0))
      .slice(0, 5)
      .map((d) => ({
        name: d.name || d.disease || "Không rõ",
        probability:
          d.probability != null ? Math.round(d.probability * 100) : null,
        description:
          d.details?.description ||
          d.disease_details?.description ||
          d.description ||
          "",
        treatment:
          d.details?.treatment ||
          d.recommendation ||
          d.disease_details?.treatment ||
          "",
      }));
  }
}
