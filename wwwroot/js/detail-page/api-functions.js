// detail-page/api-functions.js
export async function fetchPlantData(plantId) {
  try {
    const response = await fetch(`/api/Plant/${plantId}`);
    if (!response.ok) {
      if (response.status === 404)
        throw new Error("Không tìm thấy thông tin cây trồng này.");
      else if (response.status === 400)
        throw new Error("Yêu cầu không hợp lệ.");
      else throw new Error("Có lỗi xảy ra khi tải dữ liệu.");
    }
    return await response.json();
  } catch (error) {
    if (error.message) throw error;
    throw new Error(
      "Không thể kết nối đến máy chủ. Vui lòng kiểm tra kết nối internet."
    );
  }
}
