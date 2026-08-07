const logoutBtn = document.querySelector("#logout-btn");

logoutBtn.addEventListener("click", async () => {
    try {
        const response = await fetch("/api/auth/logout", {
            method: "POST"
        });

        if (response.ok) {
            window.location.href = "/auth";
        } else {
            console.error("Ошибка при выходе из системы");
        }
    } catch (error) {
        console.error("Сетевая ошибка:", error);
    }
});
