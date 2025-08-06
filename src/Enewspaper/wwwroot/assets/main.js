(function ($) {
    "use strict";
/*--
    Menu Sticky
-----------------------------------*/
var windows = $(window);
var sticky = $('.header-sticky');


/*--
    Mobile Menu
------------------------*/
$('.mobile-menu-wrap').meanmenu({
    meanScreenWidth: '767',
    meanMenuContainer: '.mobile-menu',
    meanMenuClose: '<span class="menu-close"></span>',
    meanMenuOpen: '<span class="menu-bar"></span>',
    meanRevealPosition: 'left',
    meanMenuCloseSize: '0',
});

/*-- 
    Header Search
--------------------------------------------*/
var searchToggle = $('.header-search-toggle');
var searchForm = $('.header-search-form');

searchForm.hide();
/*-- Search Toggle --*/
searchToggle.on('click', function(){
    if( searchToggle.hasClass('open') ) {
        searchForm.animate({
            width: "toggle",
        });
        $(this).removeClass('open').find('i').removeClass('fa-close').addClass('fa-search');
    }else{
        searchForm.animate({
            width: "toggle",
        });
        $(this).addClass('open').find('i').removeClass('fa-search').addClass('fa-close');
    }
});

/*-- 
    Breaking News Ticker
--------------------------------------------*/
$('.breaking-news-ticker').newsTicker({
    row_height: 40,
    max_rows: 1,
    speed: 600,
    duration: 5000,
    prevButton:  $('.news-ticker-prev'),
    nextButton:  $('.news-ticker-next'),
});
    
/*--
    Slick Slider
-----------------------------------*/

/*-- Post Carousel --*/
$('.post-carousel-1').slick({
    autoplay: true,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 1,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
    responsive: [
        {
          breakpoint: 350,
          settings: {
            arrows: false,
          }
        }
    ]
});

/*-- Popular Post Slider --*/
$('.popular-post-slider').slick({
    arrows: false,
    autoplay: true,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 3,
    responsive: [
        {
          breakpoint: 1200,
          settings: {
            slidesToShow: 2,
          }
        },
        {
          breakpoint: 992,
          settings: {
            slidesToShow: 2,
          }
        },
        {
          breakpoint: 768,
          settings: {
            slidesToShow: 1,
          }
        }
    ]
});

/*-- Five Row Post Carousel --*/
$('.five-row-post-carousel').slick({
    autoplay: false,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 1,
    rows: 5,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
    responsive: [
        {
          breakpoint: 992,
          settings: {
            slidesToShow: 2,
            rows: 4,
          }
        },
        {
          breakpoint: 768,
          settings: {
            slidesToShow: 1,
          }
        }
    ]
});
    
/*-- Four Row Post Carousel --*/
$('.four-row-post-carousel').slick({
    autoplay: false,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 1,
    rows: 4,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
});

/*-- Three Row Post Carousel --*/
$('.three-row-post-carousel').slick({
    autoplay: false,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 1,
    rows: 3,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
});

/*-- Two Row Post Carousel --*/
$('.two-row-post-carousel').slick({
    autoplay: false,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 1,
    rows: 2,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
});

/*-- Sidebar Post Carousel --*/
$('.sidebar-post-carousel').slick({
    autoplay: true,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 1,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
});

/*-- Video Post Slider --*/
$('.video-post-slider').slick({
    arrows: true,
    autoplay: true,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 3,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
});

/*-- Four Column Post Carousel --*/
$('.four-column-post-carousel').slick({
    arrows: false,
    autoplay: true,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 4,
    responsive: [
        {
          breakpoint: 1199,
          settings: {
            slidesToShow: 3,
          }
        },
        {
          breakpoint: 991,
          settings: {
            slidesToShow: 2,
          }
        },
        {
          breakpoint: 767,
          settings: {
            slidesToShow: 1,
          }
        }
    ]
});
    
/*-- Three Column Post Carousel --*/
$('.three-column-post-carousel').slick({
    arrows: true,
    autoplay: true,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 3,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
    responsive: [
        {
          breakpoint: 1199,
          settings: {
            slidesToShow: 3,
          }
        },
        {
          breakpoint: 991,
          settings: {
            slidesToShow: 2,
          }
        },
        {
          breakpoint: 767,
          settings: {
            slidesToShow: 1,
          }
        }
    ]
});

/*-- Two Column Post Carousel --*/
$('.two-column-post-carousel').slick({
    arrows: true,
    autoplay: true,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 2,
    prevArrow: '<button type="button" class="slick-prev"><i class="fa fa-angle-left"></i></button>',
    nextArrow: '<button type="button" class="slick-next"><i class="fa fa-angle-right"></i></button>',
    responsive: [
        {
          breakpoint: 767,
          settings: {
            slidesToShow: 1,
          }
        }
    ]
});

/*-- Full Width Instagram Carousel --*/
$('.fullwidth-instagram-carousel').slick({
    arrows: false,
    autoplay: true,
    autoplaySpeed: 5000,
    pauseOnFocus: false,
    pauseOnHover: false,
    infinite: true,
    slidesToShow: 5,
    responsive: [
        {
          breakpoint: 1199,
          settings: {
            slidesToShow: 4,
          }
        },
        {
          breakpoint: 991,
          settings: {
            slidesToShow: 3,
          }
        },
        {
          breakpoint: 767,
          settings: {
            slidesToShow: 2,
          }
        },
        {
          breakpoint: 350,
          settings: {
            slidesToShow: 1,
          }
        }
    ]
});
    
/*--
    Youtube Video Playlist
--------------------------------*/  
$("#unix").ycp({
    apikey : 'AIzaSyCj2GrDSBy6ISeGg3aWUM4mn3izlA1wgt0',
    playlist : 6,
    autoplay : true,
    related : true
});


/*--
    Scroll Up
-----------------------------------*/
$.scrollUp({
    easingType: 'linear',
    scrollSpeed: 900,
    animation: 'fade',
    scrollText: '<i class="fa fa-angle-up"></i>',
});

/*--
    Ajax Contact Form JS
-----------------------------------*/
$(function () {
    // Get the form.
    var form = $('#contact-form');
    // Get the messages div.
    var formMessages = $('.form-message');
    // Set up an event listener for the contact form.
    $(form).submit(function (e) {
        // Stop the browser from submitting the form.
        e.preventDefault();
        // Serialize the form data.
        var formData = $(form).serialize();
        // Submit the form using AJAX.
        $.ajax({
            type: 'POST',
            url: $(form).attr('action'),
            data: formData,
        })
        .done(function (response) {
        // Make sure that the formMessages div has the 'success' class.
        $(formMessages).removeClass('error');
        $(formMessages).addClass('success');

        // Set the message text.
        $(formMessages).text(response);

        // Clear the form.
        $('#contact-form input,#contact-form textarea').val('');
        })
        .fail(function (data) {
        // Make sure that the formMessages div has the 'error' class.
        $(formMessages).removeClass('success');
        $(formMessages).addClass('error');

        // Set the message text.
        if (data.responseText !== '') {
            $(formMessages).text(data.responseText);
        } else {
            $(formMessages).text(
                'Oops! An error occured and your message could not be sent.'
            );
        }
        });
    });
});

/*--
    Magnific Video Popup
--------------------------------*/
var imagePopup = $('.image-popup');
imagePopup.magnificPopup({
    type: 'image',
    tLoading: 'Loading image #%curr%...',
    gallery: {
        enabled: true,
    },
});
var videoPopup = $('.video-popup');
videoPopup.magnificPopup({
    type: 'iframe',
    mainClass: 'mfp-fade',
    removalDelay: 160,
    preloader: false,
    zoom: {
        enabled: true,
    }
});
    
    
})(jQuery);	