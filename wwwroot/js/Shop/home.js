

(function () {
    // ===== New products slider (10 items) =====
    var owlNew = $("#newProducts");
    owlNew.owlCarousel({
        loop: true,            // tới cuối nối về đầu
        margin: 22,            // giống khoảng cách Rhodi
        nav: false,            // nếu muốn hiện nút thì true
        dots: false,
        autoplay: true,
        autoplayTimeout: 3000, // 3s
        autoplayHoverPause: true, // hover thì dừng (tuỳ bạn)
        smartSpeed: 650,       // tốc độ trượt
        slideBy: 1,            // mỗi lần đi 1 sản phẩm
        mouseDrag: true,       // kéo chuột như vuốt
        touchDrag: true,       // vuốt mobile
        pullDrag: true,
        freeDrag: false,

        responsive: {
            0: { items: 2 },  // ✅ mobile hiển thị 2 sp
            576: { items: 2 },
            768: { items: 3 },
            992: { items: 4 }   // desktop tối đa 4
        }
    });

    // Custom next
    $(".np-next").click(function () {
        owlNew.trigger("next.owl.carousel");
    });

    // Custom prev
    $(".np-prev").click(function () {
        owlNew.trigger("prev.owl.carousel");
    });


    // ===== Hot products slider (14 items) =====
    var owlHot = $("#hotProducts");
    owlHot.owlCarousel({
        loop: true,            // tới cuối nối về đầu
        margin: 22,            // giống khoảng cách Rhodi
        nav: true,            // nếu muốn hiện nút thì true
        dots: false,
        autoplay: true,
        autoplayTimeout: 3000, // 3s
        autoplayHoverPause: true, // hover thì dừng (tuỳ bạn)
        smartSpeed: 650,       // tốc độ trượt
        slideBy: 1,            // mỗi lần đi 1 sản phẩm
        mouseDrag: true,       // kéo chuột như vuốt
        touchDrag: true,       // vuốt mobile
        pullDrag: true,
        freeDrag: false,

        responsive: {
            0: { items: 2 },  // ✅ mobile hiển thị 2 sp
            576: { items: 2 },
            768: { items: 3 },
            992: { items: 3 }
        }
    });

})();