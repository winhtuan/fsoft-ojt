function filterPlants() {
  const search = document.getElementById("searchInput").value.toLowerCase();
  // Add logic for offcanvas filters here (e.g., get checked values and filter plants array accordingly)
  // For prototype, just search for now

  let filteredPlants = plants;

  if (search) {
    filteredPlants = filteredPlants.filter(
      (plant) =>
        plant.commonName.toLowerCase().includes(search) ||
        plant.scientificName.toLowerCase().includes(search) ||
        plant.description.toLowerCase().includes(search)
    );
  }

  renderTable(filteredPlants);
}

// Đợi cho toàn bộ cây DOM được tải xong rồi mới chạy mã
document.addEventListener("DOMContentLoaded", function () {
  // 1. Tìm tất cả các nút bấm là lựa chọn filter
  const filterOptions = document.querySelectorAll(".filter-option");

  // 2. Lặp qua từng nút và gán sự kiện 'click' cho nó
  filterOptions.forEach(function (option) {
    option.addEventListener("click", function () {
      // Lấy ra phần tử được click (chính là 'this')
      const clickedOption = this;

      // 3. Lấy văn bản và giá trị từ lựa chọn được click
      const selectedText = clickedOption.textContent;
      const selectedValue = clickedOption.dataset.filterValue; // Lấy giá trị từ data-filter-value

      // 4. Tìm nút dropdown chính tương ứng
      // - đi ngược lên cây DOM tìm thẻ cha có class '.filter-dropdown'
      const dropdownContainer = clickedOption.closest(".filter-dropdown");
      // - từ thẻ cha đó, tìm xuống nút bấm có class '.dropdown-toggle'
      const mainButton = dropdownContainer.querySelector(".dropdown-toggle");

      // 5. Cập nhật văn bản cho nút chính
      if (selectedValue === "") {
        // Nếu người dùng chọn "Tất cả" (có value là rỗng)
        // Lấy lại văn bản gốc từ thuộc tính data-placeholder mà chúng ta đã thêm ở Bước 1
        mainButton.textContent = mainButton.dataset.placeholder;
      } else {
        // Nếu chọn một lựa chọn khác
        mainButton.textContent = selectedText;
      }

      // (Trong tương lai, bạn sẽ dùng selectedValue và clickedOption.dataset.filterName ở đây
      // để gọi API và lọc dữ liệu cho bảng)
      console.log(
        `Filter: ${clickedOption.dataset.filterName}, Value: ${selectedValue}`
      );
    });
  });
});
