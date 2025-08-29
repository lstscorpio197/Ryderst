// Cart functionality với jQuery

$cart = {
    init() {
        this.getData();
        this.bindEvents();
    },
    isOpen : false,
    isNew: false,
    getData: function () {
        if ($cart.isNew == true)
            return;
        var getResponse = AjaxConfigHelper.SendRequestToServer(`/Cart/GetCart`, "GET", null);
        getResponse.then((res) => {
            if (res.IsOk) {
                $cart.renderCartItems(res.Body.Data, res.Body.Description);
            }
            else {

            }
        })
    },
    bindEvents() {
        // Cart toggle
        $('#cartBtn').on('click', () => {
            this.toggleCart();
        });

        $('#closeCart').on('click', () => {
            this.closeCart();
        });

        // Checkout button
        //$('#checkoutBtn').on('click', () => {
        //    this.checkout();
        //});

        // Close cart when clicking outside
        $(document).on('click', (e) => {
            const $cartSidebar = $('#cartSidebar');
            const $cartBtn = $('#cartBtn');

            if (this.isOpen &&
                !$cartSidebar.is(e.target) &&
                $cartSidebar.has(e.target).length === 0 &&
                !$cartBtn.is(e.target) &&
                $cartBtn.has(e.target).length === 0) {
                this.closeCart();
            }
        });
    },
    toggleCart() {
        if (this.isOpen) {
            this.closeCart();
        } else {
            this.openCart();
        }
    },
    openCart() {
        this.isOpen = true;
        $('#cartSidebar').addClass('open');
        $('body').css('overflow', 'hidden');
    },
    closeCart() {
        this.isOpen = false;
        $('#cartSidebar').removeClass('open');
        $('body').css('overflow', '');
    },

    renderCartItems(data, total) {
        const $cartItemsContainer = $('#cartItems');
        const $cartTotal = $('#cartTotal');
        const $cartCountElement = $('#cartCount');
        $cartCountElement.text(data.length);
        $cartCountElement.css('display', data.length > 0 ? 'flex' : 'none');

        if (data.length === 0) {
            $cartItemsContainer.html(`
                <div style="text-align: center; padding: 2rem; color: #64748b;">
                    <p>Giỏ hàng trống</p>
                    <p style="font-size: 0.9rem; margin-top: 0.5rem;">Hãy thêm sản phẩm vào giỏ hàng!</p>
                </div>
            `);
            $cartTotal.text('0 ₫');
            return;
        }

        let itemsHtml = data.map(item => `
            <div class="cart-item">
                <img src="${item.ImageUrl}" alt="" class="cart-item-image">
                <div class="cart-item-info">
                    <div class="cart-item-name">${item.ProductName}</div>
                    <div class="cart-item-details">
                        Size:  | Màu: 
                    </div>
                    <div class="cart-item-price">${item.PriceTxt} ₫</div>
                    <div class="quantity-controls">
                        <button class="quantity-btn decrease-btn" data-id="${item.ProductVariantId}">-</button>
                        <input type="number" class="quantity quantity-input" value="${item.Quantity}" min="1" 
                               data-id="${item.ProductVariantId}">
                        <button class="quantity-btn increase-btn" data-id="${item.ProductVariantId}">+</button>
                        <button class="quantity-btn remove-btn" data-id="${item.ProductVariantId}"
                                style="margin-left: 0.5rem; background: #fee2e2; color: #dc2626;">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                <polyline points="3,6 5,6 21,6"></polyline>
                                <path d="m19,6v14a2,2 0 0,1 -2,2H7a2,2 0 0,1 -2,-2V6m3,0V4a2,2 0 0,1 2,-2h4a2,2 0 0,1 2,2v2"></path>
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        `).join('');

        $cartItemsContainer.html(itemsHtml);
        $cartTotal.text(`${total} ₫`);

        // Bind quantity control events
        $('.decrease-btn').off('click').on('click', function (){    //-
            $input = $(this).siblings('.quantity-input');
            let val = $input.val().trim() || 0;
            $input.val(Number(val) - 1).trigger('change');
        });

        $('.increase-btn').off('click').on('click', function () {    //+
            $input = $(this).siblings('.quantity-input');
            let val = $input.val().trim() || 0;
            $input.val(Number(val) + 1).trigger('change');
        });

        $('.quantity-input').off('change').on('change', function () {
            const $input = $(this);
            const id = parseInt($input.data('id'));
            const quantity = parseInt($input.val());

            $cart.updateQuantity(id, quantity);
        });

        $('.remove-btn').on('click', (e) => {
            const $btn = $(e.currentTarget);
            const id = parseInt($btn.data('id'));
            $cart.removeItem(id);
        });

        $cart.isNew = true;
    },
    removeItem: function (id) {
        let dataSend = {productVariantId: id};

        var getResponse = AjaxConfigHelper.SendRequestToServer(`/Cart/RemoveItem`, "POST", dataSend);
        getResponse.then((res) => {
            if (res.IsOk) {
                $cart.renderCartItems(res.Body.Data, res.Body.Description);
            }
            else {
            }
        })
    },
    updateQuantity: function (id, quantity) {
        var getResponse = AjaxConfigHelper.SendRequestToServer(`/Cart/ChangeQuantityItem`, "POST", { productVariantId: id, quantity: quantity });
        getResponse.then((res) => {
            if (res.IsOk) {
                $cart.renderCartItems(res.Body.Data, res.Body.Description);
            }
            else {
                toastWarning(res.Body.Description);
            }
        })
    }
}

$cart.init();

//class Cart {
//    constructor() {
//        //this.items = JSON.parse(localStorage.getItem('cart')) || [];
//        //this.isOpen = false;
//        //this.init();
//    }

//    init() {
//        this.updateCartCount();
//        this.bindEvents();
//    }

//    bindEvents() {
//        // Cart toggle
//        $('#cartBtn').on('click', () => {
//            this.toggleCart();
//        });

//        $('#closeCart').on('click', () => {
//            this.closeCart();
//        });

//        // Checkout button
//        $('#checkoutBtn').on('click', () => {
//            this.checkout();
//        });

//        // Close cart when clicking outside
//        $(document).on('click', (e) => {
//            const $cartSidebar = $('#cartSidebar');
//            const $cartBtn = $('#cartBtn');

//            if (this.isOpen &&
//                !$cartSidebar.is(e.target) &&
//                $cartSidebar.has(e.target).length === 0 &&
//                !$cartBtn.is(e.target) &&
//                $cartBtn.has(e.target).length === 0) {
//                this.closeCart();
//            }
//        });
//    }

//    addItem(product, selectedSize, selectedColor) {
//        const existingItemIndex = this.items.findIndex(item =>
//            item.id === product.id &&
//            item.selectedSize === selectedSize &&
//            item.selectedColor === selectedColor
//        );

//        if (existingItemIndex > -1) {
//            this.items[existingItemIndex].quantity += 1;
//        } else {
//            this.items.push({
//                ...product,
//                quantity: 1,
//                selectedSize,
//                selectedColor
//            });
//        }

//        this.saveCart();
//        this.updateCartCount();
//        this.renderCartItems();
//        this.showNotification('Đã thêm sản phẩm vào giỏ hàng!');
//    }

//    removeItem(productId, selectedSize, selectedColor) {
//        this.items = this.items.filter(item =>
//            !(item.id === productId &&
//                item.selectedSize === selectedSize &&
//                item.selectedColor === selectedColor)
//        );

//        this.saveCart();
//        this.updateCartCount();
//        this.renderCartItems();
//    }

//    updateQuantity(productId, selectedSize, selectedColor, quantity) {
//        if (quantity <= 0) {
//            this.removeItem(productId, selectedSize, selectedColor);
//            return;
//        }

//        const itemIndex = this.items.findIndex(item =>
//            item.id === productId &&
//            item.selectedSize === selectedSize &&
//            item.selectedColor === selectedColor
//        );

//        if (itemIndex > -1) {
//            this.items[itemIndex].quantity = quantity;
//            this.saveCart();
//            this.updateCartCount();
//            this.renderCartItems();
//        }
//    }

//    clearCart() {
//        this.items = [];
//        this.saveCart();
//        this.updateCartCount();
//        this.renderCartItems();
//    }

//    toggleCart() {
//        if (this.isOpen) {
//            this.closeCart();
//        } else {
//            this.openCart();
//        }
//    }

//    openCart() {
//        this.isOpen = true;
//        $('#cartSidebar').addClass('open');
//        this.renderCartItems();
//        $('body').css('overflow', 'hidden');
//    }

//    closeCart() {
//        this.isOpen = false;
//        $('#cartSidebar').removeClass('open');
//        $('body').css('overflow', '');
//    }

//    renderCartItems(data) {
//        const $cartItemsContainer = $('#cartItems');
//        const $cartTotal = $('#cartTotal');
//        if (data.length === 0) {
//            $cartItemsContainer.html(`
//                <div style="text-align: center; padding: 2rem; color: #64748b;">
//                    <p>Giỏ hàng trống</p>
//                    <p style="font-size: 0.9rem; margin-top: 0.5rem;">Hãy thêm sản phẩm vào giỏ hàng!</p>
//                </div>
//            `);
//            $cartTotal.text('0₫');
//            return;
//        }

//        let itemsHtml = data.map(item => `
//            <div class="cart-item">
//                <img src="${item.ImageUrl}" alt="" class="cart-item-image">
//                <div class="cart-item-info">
//                    <div class="cart-item-name">${item.ProductName}</div>
//                    <div class="cart-item-details">
//                        Size:  | Màu: 
//                    </div>
//                    <div class="cart-item-price">${item.PriceTxt}</div>
//                    <div class="quantity-controls">
//                        <button class="quantity-btn decrease-btn" data-id="${item.ProductVariantId}">-</button>
//                        <input type="number" class="quantity quantity-input" value="${item.Quantity}" min="1" 
//                               data-id="${item.ProductVariantId}">
//                        <button class="quantity-btn increase-btn" data-id="${item.ProductVariantId}">+</button>
//                        <button class="quantity-btn remove-btn" data-id="${item.ProductVariantId}"
//                                style="margin-left: 0.5rem; background: #fee2e2; color: #dc2626;">
//                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
//                                <polyline points="3,6 5,6 21,6"></polyline>
//                                <path d="m19,6v14a2,2 0 0,1 -2,2H7a2,2 0 0,1 -2,-2V6m3,0V4a2,2 0 0,1 2,-2h4a2,2 0 0,1 2,2v2"></path>
//                            </svg>
//                        </button>
//                    </div>
//                </div>
//            </div>
//        `).join('');

//        $cartItemsContainer.html(itemsHtml);

//        // Bind quantity control events
//        $('.decrease-btn').on('click', (e) => {
//            const $btn = $(e.currentTarget);
//            const id = parseInt($btn.data('id'));
//            const size = $btn.data('size');
//            const color = $btn.data('color');
//            const currentQuantity = parseInt($btn.siblings('.quantity-input').val());
//            this.updateQuantity(id, size, color, currentQuantity - 1);
//        });

//        $('.increase-btn').on('click', (e) => {
//            const $btn = $(e.currentTarget);
//            const id = parseInt($btn.data('id'));
//            const size = $btn.data('size');
//            const color = $btn.data('color');
//            const currentQuantity = parseInt($btn.siblings('.quantity-input').val());
//            this.updateQuantity(id, size, color, currentQuantity + 1);
//        });

//        $('.quantity-input').on('change', (e) => {
//            const $input = $(e.currentTarget);
//            const id = parseInt($input.data('id'));
//            const size = $input.data('size');
//            const color = $input.data('color');
//            const quantity = parseInt($input.val());
//            this.updateQuantity(id, size, color, quantity);
//        });

//        $('.remove-btn').on('click', (e) => {
//            const $btn = $(e.currentTarget);
//            const id = parseInt($btn.data('id'));
//            const size = $btn.data('size');
//            const color = $btn.data('color');
//            this.removeItem(id, size, color);
//        });
//    }

//    updateCartCount() {
//        const count = this.items.reduce((sum, item) => sum + item.quantity, 0);
//        const $cartCountElement = $('#cartCount');
//        $cartCountElement.text(count);
//        $cartCountElement.css('display', count > 0 ? 'flex' : 'none');
//    }

//    saveCart() {
//        localStorage.setItem('cart', JSON.stringify(this.items));
//    }

//    checkout() {
//        if (this.items.length === 0) {
//            this.showNotification('Giỏ hàng trống!', 'error');
//            return;
//        }

//        const total = this.items.reduce((sum, item) => sum + (item.price * item.quantity), 0);
//        const confirmed = confirm(`Xác nhận thanh toán ${formatPrice(total)}?`);

//        if (confirmed) {
//            this.showNotification('Đặt hàng thành công! Cảm ơn bạn đã mua hàng.', 'success');
//            this.clearCart();
//            this.closeCart();
//        }
//    }

//    showNotification(message, type = 'success') {
//        const $notification = $(`
//            <div class="notification ${type}" style="
//                position: fixed;
//                top: 20px;
//                right: 20px;
//                background: ${type === 'success' ? '#10b981' : '#ef4444'};
//                color: white;
//                padding: 1rem 1.5rem;
//                border-radius: 8px;
//                box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
//                z-index: 9999;
//                animation: slideIn 0.3s ease-out;
//            ">${message}</div>
//        `);

//        $('body').append($notification);

//        setTimeout(() => {
//            $notification.css('animation', 'slideOut 0.3s ease-out');
//            setTimeout(() => {
//                $notification.remove();
//            }, 300);
//        }, 3000);

//        // Add slideOut animation if not exists
//        if (!$('#notification-styles').length) {
//            $('head').append(`
//                <style id="notification-styles">
//                    @keyframes slideOut {
//                        to {
//                            transform: translateX(100%);
//                            opacity: 0;
//                        }
//                    }
//                </style>
//            `);
//        }
//    }
//}

//// Initialize cart
//const cart = new Cart();