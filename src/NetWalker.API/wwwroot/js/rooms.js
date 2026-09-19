const RoomStatusMap = {
    0: { text: "Ожидание хоста", class: "badge-warning" },
    1: { text: "Лобби", class: "badge-lobby" },
    2: { text: "В игре", class: "badge-success" },
    3: { text: "Завершена", class: "badge-finished" },
    4: { text: "Заброшена", class: "badge-abandoned" }
};


document.addEventListener("DOMContentLoaded", () => {
    loadActiveRooms();

    const createNavBtn = document.getElementById("create-room-trigger");
    const modal = document.getElementById("create-room-modal");
    const closeModalBtn = document.getElementById("btn-close-modal");
    const submitCreateBtn = document.getElementById("btn-submit-create");

    if (createNavBtn) {
        createNavBtn.addEventListener("click", (e) => {
            e.preventDefault();
            if (modal) modal.style.display = "flex";
        });
    }

    if (closeModalBtn) {
        closeModalBtn.addEventListener("click", () => {
            if (modal) modal.style.display = "none";
        });
    }

    if (submitCreateBtn) {
        submitCreateBtn.addEventListener("click", async () => {
            const maxPlayersVal = document.getElementById("max-players-input").value;
            if (modal) modal.style.display = "none";
            await CreateRoom(parseInt(maxPlayersVal, 10) || 4);
            await loadActiveRooms();
        });
    }
});
async function CreateRoom(maxPlayers) {
    try {
        const response = await fetch("/api/room/create-room", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                maxPlayers: maxPlayers,
                fromCli: false
            })
        });

        const contentType = response.headers.get("content-type");
        const isJson = contentType && contentType.includes("application/json");

        if (!response.ok) {
            let msg = "Произошла ошибка при создании комнаты";
            if (isJson) {
                const errData = await response.json();
                msg = errData?.detail || msg;
            } else {
                msg = await response.text();
            }
            console.error(msg);
            alert(msg);
            return;
        }

        const roomData = await response.json();
        if (roomData.ticketCode) {
            alert(`Комната создана! Код сессии: ${roomData.sessionCode}\nTicket Code: ${roomData.ticketCode}`);
        } else {
            alert(`Комната создана! Код сессии: ${roomData.sessionCode}`);
        }

    } catch (error) {
        console.error("Критическая ошибка сети или сервера:", error);
    }
}

async function GetRoomByCode(code) {
    if (!code || code.length !== 6) {
        console.error("Код должен состоять ровно из 6 символов");
        return null;
    }
    try {
        const response = await fetch(`/api/room/${code}`);
        const contentType = response.headers.get("content-type");
        const isJson = contentType && contentType.includes("application/json");

        if (!response.ok) {
            let msg = "Комната не найдена";
            if (isJson) {
                const errData = await response.json();
                msg = errData?.detail || msg;
            }
            console.error(msg);
            return null;
        }

        return await response.json();
    } catch (error) {
        console.error("Критическая ошибка сети или сервера:", error);
        return null;
    }
}

async function GetActiveRooms() {
    try {
        const response = await fetch("/api/room/active-rooms");
        const contentType = response.headers.get("content-type");
        const isJson = contentType && contentType.includes("application/json");

        if (!response.ok) {
            let msg = "Не удалось загрузить активные комнаты";
            if (isJson) {
                const errData = await response.json();
                msg = errData?.detail || msg;
            }
            console.error(msg);
            return [];
        }

        return await response.json();
    } catch (error) {
        console.error("Критическая ошибка сети или сервера:", error);
        return [];
    }
}

async function loadActiveRooms() {
    const rooms = await GetActiveRooms();
    const grid = document.getElementById("room-grid");
    const activeSessionsStat = document.getElementById("stat-active-sessions");

    if (!grid) return;

    grid.innerHTML = "";

    if (!rooms || rooms.length === 0) {
        grid.innerHTML = `<p style="color: var(--text-secondary);">Активных комнат пока нет.</p>`;
        if (activeSessionsStat) activeSessionsStat.textContent = "0";
        return;
    }

    if (activeSessionsStat) {
        activeSessionsStat.textContent = rooms.length;
    }

    rooms.forEach(room => {
        const card = document.createElement("div");
        card.className = "room-card";
        const code = room.sessionCode || room.code || "XXXXX";

        const statusObj = RoomStatusMap[room.status] ?? { text: "Неизвестно", class: "badge-secondary" };

        card.innerHTML = `
        <div class="room-card-header">
            <h4>Код: <code>${code}</code></h4>
            <span class="room-badge ${statusObj.class}">${statusObj.text}</span>
        </div>
        <p>Макс. игроков: ${room.maxPlayers}<br>Создана: ${new Date(room.createdTime).toLocaleTimeString()}</p>
        <button class="btn btn-primary join-room-btn" data-code="${code}">Присоединиться</button>
    `;
        grid.appendChild(card);

        document.querySelectorAll(".join-room-btn").forEach(btn => {
            btn.addEventListener("click", async (e) => {
                const code = e.target.getAttribute("data-code");

                try {
                    const response = await fetch(`/api/room/join-room/${code}`, {
                        method: "POST"
                    });

                    if (!response.ok) {
                        const errData = await response.json();
                        alert(errData?.detail || "Не удалось присоединиться к комнате");
                        return;
                    }

                    window.location.href = `/room/${code}`;
                } catch (error) {
                    console.error("Ошибка сети:", error);
                }
            });
        });
    });
}