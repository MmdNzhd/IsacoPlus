// -----swiper----
var swiper = new Swiper(".swiper-containerTitr", {
  spaceBetween: 30,
  loop: true,
  pagination: {
    el: ".swiper-pagination",
    clickable: true,
  },
});
//----swiper2---
var swiper = new Swiper(".swiper-container2", {
  slidesPerView: 3,
  loop: true,
  spaceBetween: 30,
  freeMode: true,
  pagination: {
    el: ".swiper-pagination",
    clickable: true,
  },
  breakpoints: {
    // when window width is <= 499px
    0: {
      slidesPerView: 1,
      spaceBetweenSlides: 30,
    },
    600: {
      slidesPerView: 1,
      spaceBetweenSlides: 40,
    },

    1000: {
      slidesPerView: 3,
      spaceBetweenSlides: 40,
    },
  },
});

var swiper = new Swiper(".swiper-container4", {
  spaceBetween: 30,
  loop: true,
  freeMode: true,
  pagination: {
    el: ".swiper-pagination",
    clickable: true,
  },
  breakpoints: {
    // when window width is <= 499px
    0: {
      slidesPerView: 1,
      spaceBetweenSlides: 30,
    },
    600: {
      slidesPerView: 1,
      spaceBetweenSlides: 40,
    },

    1000: {
      slidesPerView: 4,
      spaceBetweenSlides: 40,
    },
  },
});

//
var swiper = new Swiper(".swiper-Landing2", {
  slidesPerView: 4,
  loop: true,
  spaceBetween: 30,
  pagination: {
    el: ".swiper-pagination",
    clickable: true,
  },
  breakpoints: {
    // when window width is <= 499px
    0: {
      slidesPerView: 1,
      spaceBetweenSlides: 30,
    },
    800: {
      slidesPerView: 3,
      spaceBetweenSlides: 40,
    },

    1300: {
      slidesPerView: 4,
      spaceBetweenSlides: 40,
    },
  },
});
var swiper = new Swiper(".swiper-containerBlog2", {
  slidesPerView: 4,
  loop: true,
  spaceBetween: 30,
  pagination: {
    el: ".swiper-pagination",
    clickable: true,
  },
  breakpoints: {
    // when window width is <= 499px
    0: {
      slidesPerView: 1,
      spaceBetweenSlides: 30,
    },
    800: {
      slidesPerView: 3,
      spaceBetweenSlides: 40,
    },

    1300: {
      slidesPerView: 4,
      spaceBetweenSlides: 40,
    },
  },
});

//
var swiper = new Swiper(".swiper-containerBlog", {
  slidesPerView: 3,
  loop: true,
  spaceBetween: 30,
  pagination: {
    el: ".swiper-pagination",
    clickable: true,
  },
  breakpoints: {
    // when window width is <= 499px
    0: {
      slidesPerView: 1,
      spaceBetweenSlides: 30,
    },
    800: {
      slidesPerView: 2,
      spaceBetweenSlides: 40,
    },

    1300: {
      slidesPerView: 3,
      spaceBetweenSlides: 40,
    },
  },
});
