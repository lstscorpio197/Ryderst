const $search = $("#searchInput");
const $range = $("#priceRange");
const $sort = $("#sortSelect");
const $priceValue = $("#priceValue");
function formatVND(value) {
    return Number(value).toLocaleString("vi-VN") + " ₫";
}

function updatePriceLabel() {
    $priceValue.text("Tối đa: " + formatVND($range.val()));
}

function filterAndSort() {
    const keyword = $search.val().toLowerCase().trim();
    const maxprice = parseInt($range.val(), 10);
    const sort = $sort.val();

    window.location = `/product?keyword=${keyword}&maxprice=${maxprice}&sort=${sort}`;
}


$search.on("change", filterAndSort);
$range.on("input change", function () {
    updatePriceLabel();
    filterAndSort();
});
$sort.on("change", filterAndSort);