document.addEventListener("DOMContentLoaded", function () {
  // Lặp qua tất cả các nút "favorite"
  document.querySelectorAll(".product-card__favorite").forEach(function (btn) {
    const plantId = btn.getAttribute("data-plant-id");
    if (!plantId) return;

    // Kiểm tra trạng thái favorite khi load trang
    fetch(`/api/Favorite/is-favorite/${plantId}`)
      .then((res) => res.json())
      .then((data) => {
        if (data.success && data.isFavorite) {
          btn.classList.add("favorited");
        } else {
          btn.classList.remove("favorited");
        }
      })
      .catch(() => {
        // Fail silently
        btn.classList.remove("favorited");
      });

    // Gắn sự kiện toggle khi bấm vào nút favorite
    btn.addEventListener("click", async function (e) {
      e.preventDefault();
      try {
        const res = await fetch("/api/Favorite/toggle", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(plantId),
        });
        const data = await res.json();
        if (data.success) {
          // Toggle class để chuyển màu trái tim
          btn.classList.toggle("favorited", data.isFavorite);
        } else {
          alert("Không thể cập nhật trạng thái yêu thích.");
        }
      } catch (error) {
        alert("Vui lòng đăng nhập để sử dụng chức năng này!");
      }
    });
  });
});
