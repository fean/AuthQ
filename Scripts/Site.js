window.accountMenuOpen = false;
$('.menu-account').click(function () {
    if (!window.accountMenuOpen) {
        $('.menu-account').addClass('open');
        window.accountMenuOpen = true;
    } else {
        $('.menu-account').removeClass('open');
        window.accountMenuOpen = false;
    }
});
$('body').click(function (e) {
    if (window.accountMenuOpen && !($(e.target) == $('menu-account') || $('.menu-account').has($(e.target)))) {
        $('.menu-account').removeClass('open');
        window.accountMenuOpen = false;
    }
});