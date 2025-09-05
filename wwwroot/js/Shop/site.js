// Mobile menu
$('#mobileMenuBtn').on('click', () => {
    this.toggleMobileMenu();
});

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