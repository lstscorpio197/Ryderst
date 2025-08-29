$('.product-card').off('click').on('click', function(){
    let name = $(this).data('name');
    let id = $(this).data('product');
    window.location = `/${name}-p${id}`;
})