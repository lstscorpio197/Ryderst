// Main application logic với jQuery
class FashionStore {
    constructor() {
        this.currentCategory = 'all';
        this.currentSort = 'default';
        this.filteredProducts = [...products];
        this.init();
    }

    init() {
        this.renderCategories();
        this.renderProducts();
        this.bindEvents();
    }

    bindEvents() {
        // Navigation links
        $('.nav-link').on('click', (e) => {
            e.preventDefault();
            const category = $(e.currentTarget).data('category');
            this.filterByCategory(category);
            this.updateActiveNavLink($(e.currentTarget));
        });

        // Sort select
        $('#sortSelect').on('change', (e) => {
            this.currentSort = $(e.currentTarget).val();
            this.sortProducts();
        });

        // Category cards
        $(document).on('click', '.category-card', (e) => {
            const categoryId = $(e.currentTarget).data('category');
            this.filterByCategory(categoryId);
            this.updateActiveNavLink($(`.nav-link[data-category="${categoryId}"]`));
        });

        // Product cards
        $(document).on('click', '.product-card', (e) => {
            if (!$(e.target).hasClass('add-to-cart-btn')) {
                const productId = parseInt($(e.currentTarget).data('product'));
                location.href = `abc-p${productId}`;
                /*this.showProductModal(productId);*/
            }
        });

        // Add to cart buttons (quick add)
        $(document).on('click', '.add-to-cart-btn', (e) => {
            e.stopPropagation();
            const productId = parseInt($(e.currentTarget).closest('.product-card').data('product'));
            const product = products.find(p => p.id === productId);

            const defaultSize = product.sizes[0];
            const defaultColor = product.colors[0];

            cart.addItem(product, defaultSize, defaultColor);
        });

        // Modal events
        $('#modalClose').on('click', () => {
            this.closeModal();
        });

        $('#productModal').on('click', (e) => {
            if ($(e.target).is('#productModal')) {
                this.closeModal();
            }
        });

        // Modal add to cart
        $(document).on('click', '.add-to-cart-modal-btn', (e) => {
            const productId = parseInt($(e.currentTarget).data('product'));
            const product = products.find(p => p.id === productId);

            const selectedSize = $('.option-btn.selected[data-type="size"]').data('value');
            const selectedColor = $('.option-btn.selected[data-type="color"]').data('value');

            if (!selectedSize || !selectedColor) {
                alert('Vui lòng chọn size và màu sắc!');
                return;
            }

            cart.addItem(product, selectedSize, selectedColor);
            this.closeModal();
        });

        // Option buttons
        $(document).on('click', '.option-btn', (e) => {
            const $btn = $(e.currentTarget);
            const type = $btn.data('type');
            $(`.option-btn[data-type="${type}"]`).removeClass('selected');
            $btn.addClass('selected');
        });

        // Search functionality
        $('#searchBtn').on('click', () => {
            this.toggleSearch();
        });

        // CTA button
        $('.cta-btn').on('click', () => {
            $('html, body').animate({
                scrollTop: $('.products').offset().top
            }, 800);
        });

        // Mobile menu
        $('#mobileMenuBtn').on('click', () => {
            console.log(1234);
            this.toggleMobileMenu();
        });

        // Smooth scrolling for anchor links
        $('a[href^="#"]').on('click', function (e) {
            e.preventDefault();
            const target = $(this.getAttribute('href'));
            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top
                }, 800);
            }
        });
    }

    renderCategories() {
        const $categoriesGrid = $('#categoriesGrid');
        const categoriesHtml = categories.map(category => `
            <div class="category-card fade-in" data-category="${category.id}">
                <img src="${category.image}" alt="${category.name}" class="category-image">
                <div class="category-info">
                    <h3>${category.name}</h3>
                </div>
            </div>
        `).join('');

        $categoriesGrid.html(categoriesHtml);
    }

    renderProducts() {
        const $productsGrid = $('#productsGrid');

        if (this.filteredProducts.length === 0) {
            $productsGrid.html(`
                <div style="grid-column: 1 / -1; text-align: center; padding: 3rem; color: #64748b;">
                    <p style="font-size: 1.2rem;">Không tìm thấy sản phẩm nào</p>
                    <p style="margin-top: 0.5rem;">Hãy thử thay đổi bộ lọc hoặc danh mục khác</p>
                </div>
            `);
            return;
        }

        const productsHtml = this.filteredProducts.map(product => `
            <div class="product-card fade-in" data-product="${product.id}">
                <div style="position: relative;">
                    <img src="${product.image}" alt="${product.name}" class="product-image">
                    ${product.isOnSale ? '<div class="product-badge badge-sale">SALE</div>' : ''}
                    ${product.isNew ? '<div class="product-badge badge-new">NEW</div>' : ''}
                </div>
                <div class="product-info">
                    <h3 class="product-name">${product.name}</h3>
                    <div class="product-price">
                        <span class="current-price">${formatPrice(product.price)}</span>
                        ${product.originalPrice ? `<span class="original-price">${formatPrice(product.originalPrice)}</span>` : ''}
                    </div>
                    <div class="product-rating">
                        <span class="stars">${generateStars(product.rating)}</span>
                        <span class="rating-text">(${product.reviews} đánh giá)</span>
                    </div>
                    <button class="add-to-cart-btn">Thêm vào giỏ</button>
                </div>
            </div>
        `).join('');

        $productsGrid.html(productsHtml);

        // Trigger fade-in animation
        setTimeout(() => {
            $('.product-card, .category-card').addClass('fade-in');
        }, 100);
    }

    filterByCategory(category) {
        this.currentCategory = category;

        if (category === 'all') {
            this.filteredProducts = [...products];
        } else {
            this.filteredProducts = products.filter(product => product.category === category);
        }

        this.sortProducts();
    }

    sortProducts() {
        switch (this.currentSort) {
            case 'price-low':
                this.filteredProducts.sort((a, b) => a.price - b.price);
                break;
            case 'price-high':
                this.filteredProducts.sort((a, b) => b.price - a.price);
                break;
            case 'rating':
                this.filteredProducts.sort((a, b) => b.rating - a.rating);
                break;
            case 'newest':
                this.filteredProducts.sort((a, b) => (b.isNew ? 1 : 0) - (a.isNew ? 1 : 0));
                break;
            default:
                // Default order
                break;
        }

        this.renderProducts();
    }

    updateActiveNavLink($activeLink) {
        $('.nav-link').removeClass('active');
        $activeLink.addClass('active');
    }

    showProductModal(productId) {
        const product = products.find(p => p.id === productId);
        if (!product) return;

        const $modalBody = $('#modalBody');
        $modalBody.html(`
            <div class="product-detail">
                <div>
                    <img src="${product.image}" alt="${product.name}" class="product-detail-image">
                </div>
                <div class="product-detail-info">
                    <h2>${product.name}</h2>
                    <div class="product-detail-price">
                        <span class="current-price">${formatPrice(product.price)}</span>
                        ${product.originalPrice ? `<span class="original-price">${formatPrice(product.originalPrice)}</span>` : ''}
                    </div>
                    <div class="product-rating">
                        <span class="stars">${generateStars(product.rating)}</span>
                        <span class="rating-text">(${product.reviews} đánh giá)</span>
                    </div>
                    <p class="product-description">${product.description}</p>
                    
                    <div class="product-options">
                        <div class="option-group">
                            <label class="option-label">Kích thước:</label>
                            <div class="option-buttons">
                                ${product.sizes.map(size => `
                                    <button class="option-btn" data-type="size" data-value="${size}">${size}</button>
                                `).join('')}
                            </div>
                        </div>
                        
                        <div class="option-group">
                            <label class="option-label">Màu sắc:</label>
                            <div class="option-buttons">
                                ${product.colors.map(color => `
                                    <button class="option-btn" data-type="color" data-value="${color}">${color}</button>
                                `).join('')}
                            </div>
                        </div>
                    </div>
                    
                    <button class="add-to-cart-modal-btn" data-product="${product.id}">
                        Thêm vào giỏ hàng
                    </button>
                </div>
            </div>
        `);

        $('#productModal').addClass('open');
        $('body').css('overflow', 'hidden');
    }

    closeModal() {
        $('#productModal').removeClass('open');
        $('body').css('overflow', '');
    }

    toggleSearch() {
        const searchTerm = prompt('Tìm kiếm sản phẩm:');
        if (searchTerm) {
            this.searchProducts(searchTerm);
        }
    }

    searchProducts(term) {
        const searchTerm = term.toLowerCase();
        this.filteredProducts = products.filter(product =>
            product.name.toLowerCase().includes(searchTerm) ||
            product.description.toLowerCase().includes(searchTerm)
        );

        this.renderProducts();

        $('.section-title').first().text(`Kết quả tìm kiếm: "${term}"`);
        $('.nav-link').removeClass('active');
    }

    toggleMobileMenu() {
        const $nav = $('.nav');
        if ($nav.is(':visible')) {
            $nav.hide();
        } else {
            $nav.show().css({
                'position': 'absolute',
                'top': '100%',
                'left': '0',
                'right': '0',
                'background': 'white',
                'flex-direction': 'column',
                'padding': '1rem',
                'box-shadow': '0 4px 20px rgba(0, 0, 0, 0.1)',
                'z-index': '1000'
            });
        }
    }
}

// Initialize the application when DOM is ready
$(document).ready(() => {
    new FashionStore();
});