const sidebarUserNick = document.querySelector("#sidebar-user-nick");
const userNick = document.querySelector("#user-nick");
const userIdElement = document.querySelector("#user-id")
const userCreatedTime = document.querySelector("#user-created-time");

const statTotalGames = document.querySelector("#stat-total-games");
const statTotalPlaytime = document.querySelector("#stat-total-playtime");
const statLongestSession = document.querySelector("#stat-longest-session");

document.addEventListener("DOMContentLoaded", GetProfile);

async function GetProfile() {
    try {
       const response = await fetch("/api/User/profile", { method: "GET" });

        if (response.ok) {
            const profile = await response.json();
            
            const { id, nick, createdTime, statsResponse } = profile;

            if (userIdElement && id) {
                userIdElement.textContent = id;
            }

            if (userNick) userNick.textContent = nick;
            if (sidebarUserNick) sidebarUserNick.textContent = nick;

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
        } else {
            console.error(`Ошибка сервера: ${response.status}`);
        }
    } catch (error) {
        console.error("Сетевая ошибка при получении профиля:", error);
    }
}
