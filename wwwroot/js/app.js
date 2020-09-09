
        // ---timer---

//        var timeInSecs;
//var ticker;

//function startTimer(secs) {
//timeInSecs = parseInt(secs);
//ticker = setInterval("tick()", 1000); 
//}

//function tick( ) {
//var secs = timeInSecs;
//if (secs > 0) {
//timeInSecs--; 
//}
//else {
//clearInterval(ticker);
//startTimer(5*60); // 4 minutes in seconds
//}

//var days= Math.floor(secs/86400); 
//secs %= 86400;
//var hours= Math.floor(secs/3600);
//secs %= 3600;
//var mins = Math.floor(secs/60);
//secs %= 60;
//var pretty = ( (days < 10 ) ? "0" : "" ) + days + ":" + ( (hours < 10 ) ? "0" : "" ) + hours + ":" + ( (mins < 10) ? "0" : "" ) + mins + ":" + ( (secs < 10) ? "0" : "" ) + secs;
////**********************************************************************/
//var elem1 = document.getElementById("baneertimer");
//if(elem1){
//elem1.innerHTML = pretty;
//}

//var elem2 = document.getElementById("timer-offer1");
//if(elem2){
//elem2.innerHTML = pretty;
//}
//var elem3 = document.getElementById("timer-offer2");
//if(elem3){
//elem3.innerHTML = pretty;
//}
//var elem4 = document.getElementById("timer-offer4");
//if(elem4){
//    elem4.innerHTML = pretty;
//}
//**********************************************************************/ 
//}

//startTimer(60*60); // 4 minutes in seconds

// --------------end timer----




// -----swiper----
var swiper = new Swiper('.swiper-containerTitr', {
    spaceBetween: 30,
    loop:true,
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
    },
});
  //----swiper2---
  var swiper = new Swiper('.swiper-container2', {
    slidesPerView: 4,
    loop:true,
    spaceBetween: 30,
    freeMode: true,
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
    },
    breakpoints: {
        // when window width is <= 499px
        0: {
            slidesPerView: 1,
            spaceBetweenSlides: 30
        },
        600: {
            slidesPerView: 2,
            spaceBetweenSlides: 40
        },
   
        1000: {
            slidesPerView: 4,
            spaceBetweenSlides: 40
        }
    }
});
// -----swiper 3---
var swiper = new Swiper('.swiper-containerBlog', {
    slidesPerView: 3,
    loop:true,
    spaceBetween: 30,
    pagination: {
      el: '.swiper-pagination',
      clickable: true,
    },
    breakpoints: {
        // when window width is <= 499px
        0: {
            slidesPerView: 1,
            spaceBetweenSlides: 30
        },
        800: {
            slidesPerView: 2,
            spaceBetweenSlides: 40
        },
   
        1300: {
            slidesPerView: 3,
            spaceBetweenSlides: 40
        }
    }
  });

// ---end swiper----

// ----swiper4---


var swiper = new Swiper('.swiper-container4', {
    slidesPerView: 6,
    spaceBetween: 30,
    loop:true,
    freeMode: true,
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
    },
    breakpoints: {
        // when window width is <= 499px
        0: {
            slidesPerView: 1,
            spaceBetweenSlides: 30
        },
        600: {
            slidesPerView: 2,
            spaceBetweenSlides: 40
        },
   
        1000: {
            slidesPerView: 6,
            spaceBetweenSlides: 40
        }
    }
});
