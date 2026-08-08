(async function checkAuth() {
    if (window.location.pathname.startsWith("/auth")) return;

    try {
        const response = await fetch("/api/auth/me");
        if (!response.ok) {
            window.location.replace("/auth");
        }
    } catch (err) {
        window.location.replace("/auth");
    }
})();

document.addEventListener("DOMContentLoaded", () => {
    const logoutBtn = document.querySelector("#logout-btn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", async () => {
            try {
                const response = await fetch("/api/auth/logout", { method: "POST" });
                if (response.ok) {
                    window.location.replace("/");
                }
            } catch (error) {
                console.error("Сетевая ошибка:", error);
            }
        });
    }

    const authForm = document.querySelector("#auth-form");
    const toggleModeBtn = document.querySelector("#toggle-mode-btn");

    if (authForm && toggleModeBtn) {
        const authTitle = document.querySelector("#auth-title");
        const submitBtn = document.querySelector("#submit-btn");
        const errorMessageDiv = document.querySelector("#error-message");
        const toggleModeText = document.querySelector("#toggle-mode-text");

        let isLoginMode = true;

        toggleModeBtn.addEventListener("click", (e) => {
            e.preventDefault();
            isLoginMode = !isLoginMode;
            errorMessageDiv.textContent = "";

            if (isLoginMode) {
                authTitle.textContent = "Вход в систему";
                submitBtn.textContent = "Войти";
                toggleModeBtn.textContent = "Зарегистрироваться";
                toggleModeText.textContent = "Ещё нет аккаунта? ";
            } else {
                authTitle.textContent = "Регистрация";
                submitBtn.textContent = "Создать аккаунт";
                toggleModeBtn.textContent = "Войти";
                toggleModeText.textContent = "Уже есть аккаунт? ";
            }
        });

        authForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            submitBtn.disabled = true;
            errorMessageDiv.textContent = "";

            const endpoint = isLoginMode ? "/api/auth/login" : "/api/auth/register";

            try {
                const formData = new FormData(authForm);
                const data = Object.fromEntries(formData.entries());

                const response = await fetch(endpoint, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(data)
                });

                if (!response.ok) {
                    const errorData = await response.json().catch(() => null);
                    errorMessageDiv.textContent = errorData?.detail || "Произошла ошибка";
                    return;
                }
                window.location.href = "/dashboard";

            } catch (err) {
                console.error("Сетевая ошибка:", err);
                errorMessageDiv.textContent = "Ошибка соединения с сервером";
            } finally {
                submitBtn.disabled = false;
            }
        });
    }
});