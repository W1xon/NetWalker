(async function checkAuth() {
    try {
        const response = await fetch("/api/auth/me");
        if (!response.ok) {
            window.location.replace("/auth");
        }
    } catch (err) {
        window.location.replace("/auth");
    }
})();


const logoutBtn = document.querySelector("#logout-btn");

if (logoutBtn) {
    logoutBtn.addEventListener("click", async () => {
        try {
            const response = await fetch("/api/auth/logout", {
                method: "POST"
            });

            if (response.ok) {
                window.location.replace("/");
            }
        } catch (error) {
            console.error("Сетевая ошибка:", error);
        }
    });
}