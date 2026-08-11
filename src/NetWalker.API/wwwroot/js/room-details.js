const RoomStatusMap = {
    0: { text: "Ожидание хоста", class: "badge-warning" },
    1: { text: "Лобби", class: "badge-lobby" },
    2: { text: "В игре", class: "badge-success" },
    3: { text: "Завершена", class: "badge-finished" },
    4: { text: "Заброшена", class: "badge-abandoned" }
};

document.addEventListener("DOMContentLoaded", () => {
    const pathParts = window.location.pathname.split("/");
    const roomCode = pathParts[pathParts.length - 1];

    if (!roomCode || roomCode.length !== 6) {
        alert("Неверный код комнаты в URL");
        window.location.href = "/dashboard";
        return;
    }

    GetRoomDetails(roomCode);
});

async function GetRoomDetails(code) {
    try {
        const response = await fetch(`/api/room/${code}/details`, {
            method: "GET"
        });

        if (!response.ok) {
            const errData = await response.json();
            const errMessage = errData?.detail || "Не удалось получить детали комнаты";
            alert(errMessage);
            window.location.href = "/dashboard";
            return;
        }

        const room = await response.json();
        renderRoomDetails(room);

    } catch (error) {
        console.error("Критическая ошибка сети:", error);
    }
}

function renderRoomDetails(room) {
    document.getElementById("room-code-title").textContent = room.sessionCode;

    const statusObj = RoomStatusMap[room.status] ?? { text: "Неизвестно", class: "badge-secondary" };
    const badge = document.getElementById("room-status-badge");
    badge.textContent = statusObj.text;
    badge.className = `room-badge ${statusObj.class}`;

    document.getElementById("stat-max-players").textContent = room.maxPlayers;
    document.getElementById("stat-current-players").textContent = room.players ? room.players.length : 0;
    document.getElementById("stat-created-time").textContent = new Date(room.createdTime).toLocaleTimeString();

    const grid = document.getElementById("players-grid");
    grid.innerHTML = "";

    if (!room.players || room.players.length === 0) {
        grid.innerHTML = `<p style="color: var(--text-secondary);">В комнате пока нет игроков.</p>`;
        return;
    }

    room.players.forEach(player => {
        const card = document.createElement("div");
        card.className = "room-card";

        card.innerHTML = `
            <div class="room-card-header">
                <h4>${player.username}</h4>
                <span class="room-badge ${player.isHost ? 'badge-success' : 'badge-lobby'}">
                    ${player.isHost ? 'Хост' : 'Игрок'}
                </span>
            </div>
            <p style="font-size: 0.8rem; color: var(--text-secondary);">ID: ${player.id}</p>
        `;

        grid.appendChild(card);
    });
}