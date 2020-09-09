'use strict';

(function ($) {
    $(window).on('load', function () {
        if ($('body').hasClass('horizontal-side-menu') && $(window).width() > 768) {
            if ($('body').hasClass('layout-container')) {
                $('.side-menu .side-menu-body').wrap('<div class="container"></div>');
            } else {
                $('.side-menu .side-menu-body').wrap('<div class="container"></div>');
            }
            setTimeout(function () {
                $('.side-menu .side-menu-body > ul').append('<li><a href="#"><span>Other</span></a><ul></ul></li>');
            }, 100);
            $('.side-menu .side-menu-body > ul > li').each(function () {
                var index = $(this).index(),
                    $this = $(this);
                if (index > 7) {
                    setTimeout(function () {
                        $('.side-menu .side-menu-body > ul > li:last-child > ul').append($this.clone());
                        $this.addClass('d-none');
                    }, 100);
                }
            });
        }
    });

    $(function () {
        var url = window.location;
        var element = $('div.navigation-menu-group a').filter(function () {
            return this.href == url;
        }).addClass('active').parent().parent().addClass('open');//.addClass('active').parent().addClass('in').parent();
        console.log(element);
        //if (element.is('li')) {
        //    element.addClass('active');
        //}
    });

    //setTimeout(function () {
    //    (toastr.options = {
    //        timeOut: 2e3,
    //        progressBar: !0,
    //        showMethod: "slideDown",
    //        hideMethod: "slideUp",
    //        showDuration: 200,
    //        hideDuration: 200,
    //        positionClass: "toast-top-center"
    //    }),
    //        toastr.success("خوش آمدی رکسانا رادمهر."),
    //        t(".theme-switcher").removeClass("open");
    //}, 500);
})(jQuery);