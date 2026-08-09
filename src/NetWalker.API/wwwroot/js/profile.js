const userIdElement = document.querySelector("#user-id");
const userCreatedTime = document.querySelector("#user-created-time");

const statTotalGames = document.querySelector("#stat-total-games");
const statTotalPlaytime = document.querySelector("#stat-total-playtime");
const statLongestSession = document.querySelector("#stat-longest-session");

const profileSkeletonElements = [userIdElement, userCreatedTime, statTotalGames, statTotalPlaytime, statLongestSession];

profileSkeletonElements.forEach(el => {
    if (el) el.classList.add("skeleton");
});

window.handleProfileData = function(profile) {
    const { id, createdTime, statsResponse } = profile;

    if (userIdElement && id) {
        userIdElement.textContent = id;
    }

    if (userCreatedTime && createdTime) {
        const date = new Date(createdTime);
        userCreatedTime.textContent = date.toLocaleString("ru-RU", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit"
        });
    }

    if (statsResponse) {
        const { totalGames, totalPlayTime, longestSession } = statsResponse;

        if (statTotalGames) statTotalGames.textContent = totalGames ?? 0;
        if (statTotalPlaytime) statTotalPlaytime.textContent = totalPlayTime ?? 0;
        if (statLongestSession) statLongestSession.textContent = longestSession ?? 0;
    }

    profileSkeletonElements.forEach(el => {
        if (el) el.classList.remove("skeleton");
    });
};