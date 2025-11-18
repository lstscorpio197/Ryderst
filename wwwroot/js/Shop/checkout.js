$checkout = {
    init: function () {
        $checkout.initValidate();
        $checkout.applyCoupon();
        $checkout.changeQuantity();
        $checkout.checkout();
    },
    self: $('.checkout-container'),
    form: $('#checkoutForm'),
    initValidate: function () {
        $checkout.form.validate({
            rules: {
                FullName: {
                    required: true
                },
                Phone: {
                    required: true
                },
                Address: {
                    required: true,
                }
            },
            messages: {
                FullName: {
                    required: 'Vui lòng nhập họ tên'
                },
                Phone: {
                    required: 'Vui lòng nhập số điện thoại'
                },
                Address: {
                    required: 'Vui lòng nhập địa chỉ',
                }
            }
        })
    },
    applyCoupon: function () {
        $checkout.self.find('.apply-coupon').off('click').on('click', function () {
            $this = $(this);
            let couponCode = $checkout.self.find('[name=CouponCode]').val().trim() || '';
            if (couponCode == '') {
                $checkout.self.find('#DiscountValue').text(0);
                $checkout.self.find('#TotalValue').text($checkout.self.find('#OriginPrice').text());
                return false;
            }
            $this.prop('disabled', true);
            var getResponse = AjaxConfigHelper.SendRequestToServer(`/Checkout/ApplyCoupon`, "POST", { couponCode: couponCode });
            getResponse.then((res) => {
                $this.prop('disabled', false);
                if (res.IsOk) {
                    $checkout.self.find('#DiscountValue').text(res.Body.Data.DiscoutValue);
                    $checkout.self.find('#TotalValue').text(res.Body.Data.TotalValue);
                }
                else {
                    $checkout.self.find('#DiscountValue').text(0);
                    $checkout.self.find('#TotalValue').text($checkout.self.find('#OriginPrice').text());
                    toastWarning(res.Body.Description);
                }
            })
        })
    },
    drawItems: function (data) {

        if (data.length === 0) {
            $checkout.self.find('.product-summary').html(`
                <div style="text-align: center; padding: 2rem; color: #64748b;">
                    <p>Giỏ hàng trống</p>
                    <p style="font-size: 0.9rem; margin-top: 0.5rem;">Hãy thêm sản phẩm vào giỏ hàng!</p>
                </div>
            `);
            $checkout.self.find('#OriginPrice').text(0);
            $checkout.self.find('#DiscountValue').text(0);
            $checkout.self.find('#TotalValue').text($checkout.self.find('#OriginPrice').text());
            return;
        }

        let itemsHtml = data.map(item => `
            <li data-product-id="${item.ProductId}">
                    <div class="product-thumb">
                        <img src="${item.ImageUrl}" alt="${item.ProductName}" />
                    </div>
                    <div class="product-name">${item.ProductName}</div>
                    <div class="quantity-control">
                        <button class="qty-decrease" data-id="${item.ProductVariantId}">–</button>
                        <input type="number" class="qty-input" value="${item.Quantity}" min="1" data-id="${item.ProductVariantId}" />
                        <button class="qty-increase" data-id="${item.ProductVariantId}">+</button>
                    </div>
                    <div class="product-price">${item.PriceTxt} ₫</div>
                    <div class="remove-item" data-id="${item.ProductVariantId}"><i class="icon-delete-ico-tsd"></i></div>
                </li>
        `).join('');
        $checkout.self.find('.product-summary').html(itemsHtml);

        $checkout.changeQuantity();
    },
    changeQuantity: function () {
        $checkout.self.find('.qty-decrease').off('click').on('click', function () {
            $input = $(this).siblings('.qty-input');
            let val = $input.val().trim() || 0;
            $input.val(Number(val) - 1).trigger('change');
        })
        $checkout.self.find('.qty-increase').off('click').on('click', function () {
            $input = $(this).siblings('.qty-input');
            let val = $input.val().trim() || 0;
            $input.val(Number(val) + 1).trigger('change');
        })
        $checkout.self.find('.qty-input').off('change').on('change', function () {
            let quantity = $(this).val();
            let productVariantId = $(this).data('id');

            var getResponse = AjaxConfigHelper.SendRequestToServer(`/Cart/ChangeQuantityItem`, "POST", { productVariantId: productVariantId, quantity: quantity });
            getResponse.then((res) => {
                if (res.IsOk) {
                    $checkout.drawItems(res.Body.Data);
                    $checkout.self.find('#OriginPrice').text(res.Body.Description);
                    $checkout.self.find('#DiscountValue').text(0);
                    $checkout.self.find('#TotalValue').text(res.Body.Description2);

                    $cart.renderCartItems(res.Body.Data, res.Body.Description);
                }
                else {
                    toastWarning(res.Body.Description);
                }
            })
        })

        $checkout.self.find('.remove-item').off('click').on('click', function () {
            let id = $(this).data('id') || 0;
            let dataSend = { productVariantId: id };

            var getResponse = AjaxConfigHelper.SendRequestToServer(`/Cart/RemoveItem`, "POST", dataSend);
            getResponse.then((res) => {
                if (res.IsOk) {
                    $checkout.drawItems(res.Body.Data);
                    $checkout.self.find('#OriginPrice').text(res.Body.Description);
                    $checkout.self.find('#DiscountValue').text(0);
                    $checkout.self.find('#TotalValue').text(res.Body.Description2);

                    $cart.renderCartItems(res.Body.Data, res.Body.Description);
                }
                else {
                }
            })
        })
    },
    checkout: function () {
        $checkout.self.find('#btnCheckout').off('click').on('click', function () {
            if (!$checkout.form.valid()) {
                return false;
            }
            let data = GetFormDataToObject($checkout.form);
            if (!data)
                return false;
            data.CouponCode = $checkout.self.find('[name=CouponCode]').val().trim();

            $.blockUI({ message: null });
            var getResponse = AjaxConfigHelper.SendRequestToServer(`/Checkout/Checkout`, "POST", data);
            getResponse.then((res) => {
                if (res.IsOk) {
                    $.unblockUI({ message: null });
                    toastSuccess(res.Body.Description);
                    $cart.renderCartItems([], 0);
                    $checkout.drawItems([]);
                }
                else {
                    $.unblockUI({ message: null });
                    toastWarning(res.Body.Description);
                }
            })
            
        })
    }
}
$checkout.init();