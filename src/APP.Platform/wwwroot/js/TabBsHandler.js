document.addEventListener('DOMContentLoaded', () => {
    $(".button-tab").click(function () {
        const modal = $(this).closest(".modal");
        const selectedTab = modal.find(".active-tab");
        const activeContent = $(selectedTab.data("bsTarget"));
        const targetTab = $(this);
        const targetContent = $(targetTab.data("bsTarget"));

        activeContent.removeClass("active");
        selectedTab.removeClass("action active-tab");
        targetTab.addClass("action active-tab");
        targetContent.addClass("active");
    });
});