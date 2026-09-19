document.addEventListener("DOMContentLoaded", () => {
    const leaveBtn = document.getElementById("leave-room-btn");
    if (leaveBtn) {
        leaveBtn.addEventListener("click", async () => {
            await LeaveRoom();
        });
    }

    const pathParts = window.location.pathname.split("/");
    const roomCode = pathParts[pathParts.length - 1];

    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/room/chat")
        .build();

    const sendBtn = document.getElementById("sendBtn");
    if (sendBtn) {
        sendBtn.addEventListener("click", function () {
            var message = document.getElementById("message").value;
            hubConnection.invoke("Send", roomCode, message)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        });
    }

    hubConnection.on("ReceiveMessage", function(message) {
        let messageElement = document.createElement("p");
        messageElement.textContent = message;
        document.getElementById("chatroom").appendChild(messageElement);
    });

    hubConnection.on("PlayerJoined", function(player) {
        const grid = document.getElementById("players-grid");
        const card = document.createElement("div");
        card.className = "room-card";
        card.dataset.playerId = player.id;
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

    hubConnection.on("PlayerLeaved", function(name) {
        const grid = document.getElementById("players-grid");
        const playerCard = Array.from(grid.children).find(card =>
            card.querySelector("h4").textContent.trim() === name.trim()
        );

        if (playerCard) {
            grid.removeChild(playerCard);
        }
    });

    hubConnection.start()
        .then(function () {
            hubConnection.invoke("JoinGroup", roomCode);
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
});

async function LeaveRoom(){
    const code = window.location.pathname.split("/").pop();
    try {
        const response = await fetch(`/api/room/leave-room/${code}`, {
            method: "POST"
        });

        if (!response.ok) {
            const errData = await response.json();
            alert(errData?.detail || "Не удалось покинуть комнату");
            return;
        }

        window.location.href = "/dashboard";
    } catch (error) {
        console.error("Ошибка сети:", error);
    }
}