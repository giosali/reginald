window.addEventListener("DOMContentLoaded", menuIconClick)

function menuIconClick() {
    var menuIcon = document.querySelector(".header__container__menu-icon");
    var menu = document.querySelector(".header__mobile-nav");
    var body = document.querySelector("body");

    menuIcon.addEventListener("click", e => {
        var isActive = "is-active";
        var noScroll = "no-scroll";
        menuIcon.classList.toggle(isActive);
        menu.classList.toggle(isActive);
        body.classList.toggle(noScroll);
    });
}