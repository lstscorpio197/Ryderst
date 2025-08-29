$category = {
    init: function () {
        this.clickProduct();
        this.changeSort();
    },
    self: $('section.products'),
    id: $('section.products').data('id'),
    clickProduct: function () {
        $category.self.find('.product-card').off('click').on('click', function () {
            let id = $(this).data('product');
            let name = $(this).data('name');
            window.location = `/${name}-p${id}`;
        })
    },
    changeSort: function () {
        $category.self.find('#sortSelect').off('change').on('change', function () {
            let value = $(this).val();
            console.log(window.location);
            let pathName = window.location.pathname;
            if (pathName.indexOf('page') > -1) {
                window.location = pathName + `&sort=${value}`;
            }
            else {

            }
        })
    },
    getLocation: function (page, sort) {
        return jQuery.param.querystring(window.location.href, 'valueA=321&valueB=123');
    }
}

$category.init();