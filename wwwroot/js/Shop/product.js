$product = {
    init: function () {
        this.initGiaoDien();
        this.changeQuantity();
        this.selectAttribute();
        this.addToCard();

    },
    self: $('.productDetail-page'),
    id: $('.productDetail-page').data('id'),
    initImg: function () {
        $('#lightSlider').lightSlider({
            gallery: true,
            item: 1,
            loop: true,
            slideMargin: 0,
            thumbItem: 8,
            auto: false,
            speed: 1000,
        });
    },
    initRelated: function () {
        let id = $('.productDetail-page').find('.related-products').data('id');
        let cateId = $('.productDetail-page').find('.related-products').data('cateid');
        var getResponse = AjaxConfigHelper.SendRequestToServer(`/Product/GetRelatedItems`, "POST", { id: id, cateId: cateId });
        getResponse.then((res) => {
            if (res.IsOk) {
                const $relatedItems = $('.productDetail-page').find('.related-products .swiper-wrapper');

                let data = res.Body.Data || [];
                let itemsHtml = data.map(item => `
                    <div class="swiper-slide">
                        <div class="product-card">
                            <img src="${item.ImageUrl}" alt="${item.Name}" />
                            <h4>${item.Name}</h4>
                            <p>${item.Price} đ</p>
                        </div>
                    </div>
                `).join('');
                $relatedItems.html(itemsHtml);

                var swiper = new Swiper(".relatedSwiper", {
                    slidesPerView: 2,
                    spaceBetween: 20,
                    navigation: {
                        nextEl: ".swiper-button-next",
                        prevEl: ".swiper-button-prev",
                    },
                    loop: true,
                    breakpoints: {
                        1024: {
                            slidesPerView: 4,
                        },
                        768: {
                            slidesPerView: 2,
                        },
                        480: {
                            slidesPerView: 1,
                        }
                    }
                });
            }
            else {

            }
        })
    },
    initGiaoDien: function () {
        this.initImg();
        this.initRelated();
    },
    changeQuantity: function () {
        $product.self.find('.quantity-area .btn-add').off('click').on('click', function () {
            let quantity = $product.self.find('#quantity').val().trim() || 0;
            let newValue = Number(quantity) + 1;
            $product.self.find('#quantity').val(newValue);
        })
        $product.self.find('.quantity-area .btn-subtract').off('click').on('click', function () {
            let quantity = $product.self.find('#quantity').val().trim() || 0;
            let newValue = Number(quantity) > 1 ? Number(quantity) - 1 : 1;
            $product.self.find('#quantity').val(newValue);
        })
        $product.self.find('#quantity').on('keypress', function (e) {
            var charCode = e.which ? e.which : e.keyCode;

            // Cho phép: 0-9, không cho phép ký tự khác
            if (charCode < 48 || charCode > 57) {
                e.preventDefault();
            }
        })
        $product.self.find('#quantity').on('change', function (e) {
            let value = $(this).val().trim();
            if (value == '')
                $(this).val(1);
        })
    },
    selectAttribute: function () {
        $product.self.find('.attribute .select-attr-value').off('click').on('click', function () {
            let value = $(this).data('value');
            let $attr = $(this).closest('.attribute');

            $attr.find('.select-attr-value.active').removeClass('active');
            $(this).addClass('active');

            $attr.data('value', value);
        })
    },
    getFullAttribute: function () {
        let data = [];
        let isValid = true;
        $product.self.find('.attribute').each(function (i, e) {
            let id = $(e).data('id');
            let name = $(e).data('name');
            let value = Number($(e).data('value')) || 0;

            if (value == 0) {
                isValid = false;
                toastWarning(`Vui lòng chọn ${name}`);
            }
            else {
                data.push(value);
            }
           
        })
        return isValid ? data : false;
    },
    addToCard: function () {
        $product.self.find('#add-to-cart').off('click').on('click', function () {
            let attrs = $product.getFullAttribute();
            if (!attrs)
                return false;
            let quantity = $product.self.find('#quantity').val();

            let dataSend = { ProductId: $product.id, Quantity: quantity, ProductAttributeValueIds: attrs };

            var getResponse = AjaxConfigHelper.SendRequestToServer(`/Cart/AddToCard`, "POST", dataSend);
            getResponse.then((res) => {
                if (res.IsOk) {
                    $cart.renderCartItems(res.Body.Data, res.Body.Description);
                    toastSuccess('Đã thêm sản phẩm vào giỏ hàng!')
                }
                else {

                }
            })
            
        })
    },

}

$product.init();

