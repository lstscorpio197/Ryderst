// Mobile menu
$('#mobileMenuBtn').on('click', () => {
    this.toggleMobileMenu();
});

//Hiện popup tại trang chủ
//Trường hợp muốn tắt popup, thay đổi giá trị showPopup thành false
const showPopup = true;
$('#popup-content img').off('click').on('click', function () {
    //Thay đổi đường dẫn ở đây
    let urlTarget = '/tin-tuc/black-friday-2025-ryder-n12';
    window.location.href = urlTarget;
})

function toggleMobileMenu() {
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