/* Swiper */
var swiper = new Swiper('.tailor-works')



var swiper2 = new Swiper('#category-1', {
    slidesPerView: 4,
    spaceBetween: 30,
    centeredSlides: true,

    pagination: {
        el: '.swiper-pagination',
        type: 'progressbar',
    },
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev'
    }
});

var swiper3 = new Swiper('#off', {
    slidesPerView: 2,
    spaceBetween: 3,
    pagination: {
        el: '.swiper-pagination',
        type: 'progressbar',
    },
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev'
    },
});

var swiper3 = new Swiper('#off-2', {
    slidesPerView: 2,
    spaceBetween: 3,
    pagination: {
        el: '.swiper-pagination',
        type: 'progressbar',
    },
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev'
    },
});

var swiper4 = new Swiper('#slider-1', {
    slidesPerView: 2,
    spaceBetween: 10,
    coverflowEffect: {
        rotate: 40,
        slideShadows: false,
    },
    pagination: {
        el: '.swiper-pagination',
        type: 'progressbar',
    },
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev'
    },
});

var swiper5 = new Swiper('#slider-2', {
    slidesPerView: 2,
    spaceBetween: 10,
    coverflowEffect: {
        rotate: 40,
        slideShadows: false,
    },
    pagination: {
        el: '.swiper-pagination',
        type: 'progressbar',
    },
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev'
    },
});

var swiper6 = new Swiper('.main-slider', {
    grabCursor: true,
    centeredSlides: true,
    loop:true,
    spaceBetween: 30,
    slidesPerView: "auto",
    coverflowEffect: {
        rotate: 20,
        stretch: 0,
        depth: 350,
        modifier: 1,
        slideShadows: true
    },

});



$(function () {
    new WOW().init();
});


/* Like And Favorite */

//function fav(item) {
//    if ($(item).children('svg').hasClass('text-white')) {
//        $(item).children('svg').animate().addClass('text-yellow');
//        $(item).children('svg').attr('transform', 'scale(1.3)');
//        $(item).children('svg').animate().removeClass('text-white');
//    }
//    else if ($(item).children('svg').hasClass('text-yellow')) {
//        $(item).children('svg').animate().addClass('text-white');
//        $(item).children('svg').attr('transform', 'scale(1)');
//        $(item).children('svg').animate().removeClass('text-yellow');
//    }

//}



/* Loader */
//var loader = document.getElementById('loader')
//    , border = document.getElementById('border')
//    , α = 0
//    , π = Math.PI
//    , t = 120;

//(function draw() {
//    α++;
//    α %= 360;
//    var r = (α * π / 180)
//        , x = Math.sin(r) * 125
//        , y = Math.cos(r) * - 125
//        , mid = (α > 180) ? 1 : 0
//        , anim = 'M 0 0 v -125 A 125 125 1 '
//            + mid + ' 1 '
//            + x + ' '
//            + y + ' z';
    //[x,y].forEach(function( d ){
    //  d = Math.round( d * 1e3 ) / 1e3;
    //});

    //loader.setAttribute('d', anim);
    //border.setAttribute('d', anim);

    //setTimeout(draw, t); // Redraw
//})();


/* Search */
const input = document.getElementById("search-input");
const searchBtn = document.getElementById("search-btn");

const expand = () => {
    searchBtn.classList.toggle("close");
    input.classList.toggle("square");
};



/* Sum Price */

$(window).on('scroll', () => {
    if ($(window).scrollTop() > 10) {
        $('.sumPrice').addClass('activeMenu');
    }
    else {
        $('.sumPrice').removeClass('activeMenu');
    }
});

$('.mtk-meter span').on('change', () => {
    alert();
});

/* Raise Info */
function raise(elem) {
    //e.preventDefault();
    if ($('.product-info p').attr('data-height') === '0') {
        $('.product-info p').css('height', 'auto');
    }
}