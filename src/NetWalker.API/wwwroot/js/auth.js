function showToast(message, type = 'info', title = '') {
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    const icons = { success: '✓', error: '✕', info: 'ℹ' };
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
        <span class="toast-icon">${icons[type] || icons.info}</span>
        <div class="toast-content">
            ${title ? `<div class="toast-title">${title}</div>` : ''}
            <div class="toast-message">${message}</div>
        </div>
        <button class="toast-close" onclick="this.parentElement.remove()">×</button>
    `;

    container.appendChild(toast);

    setTimeout(() => {
        toast.classList.add('toast-out');
        toast.addEventListener('animationend', () => toast.remove());
    }, 4000);
}

const VALIDATION = {
    username: {
        min: 3,
        max: 32,
        pattern: /^[a-zA-Z0-9_-]+$/,
        messages: {
            empty: 'Имя не может быть пустым',
            length: 'Длина имени должна быть от 3 до 32 символов',
            pattern: 'Имя может содержать только латиницу, цифры, _ и -'
        }
    },
    password: {
        min: 8,
        hasUpper: /[A-Z]/,
        hasDigit: /[0-9]/,
        messages: {
            empty: 'Пароль обязателен',
            min: 'Пароль должен быть минимум 8 символов',
            upper: 'Пароль должен содержать хотя бы одну заглавную букву',
            digit: 'Пароль должен содержать хотя бы одну цифру'
        }
    }
};

function validateUsername(value) {
    const v = VALIDATION.username;
    if (!value) return v.messages.empty;
    if (value.length < v.min || value.length > v.max) return v.messages.length;
    if (!v.pattern.test(value)) return v.messages.pattern;
    return null;
}

function validatePassword(value) {
    const v = VALIDATION.password;
    if (!value) return v.messages.empty;
    if (value.length < v.min) return v.messages.min;
    if (!v.hasUpper.test(value)) return v.messages.upper;
    if (!v.hasDigit.test(value)) return v.messages.digit;
    return null;
}

function updatePasswordRequirements(value, containerId = 'password-requirements') {
    const container = document.getElementById(containerId);
    if (!container) return;

    const reqLength = container.querySelector('#req-length');
    const reqUpper = container.querySelector('#req-upper');
    const reqDigit = container.querySelector('#req-digit');

    if (reqLength) reqLength.classList.toggle('valid', value.length >= 8);
    if (reqUpper) reqUpper.classList.toggle('valid', /[A-Z]/.test(value));
    if (reqDigit) reqDigit.classList.toggle('valid', /[0-9]/.test(value));
}

function setInputError(input, message) {
    if (!input) return;
    input.classList.add('error');
    input.classList.remove('success');
    const group = input.closest('.form-group');
    if (group) {
        let hint = group.querySelector('.hint-text');
        if (!hint) {
            hint = document.createElement('p');
            hint.className = 'hint-text error';
            group.appendChild(hint);
        }
        hint.textContent = message;
        hint.classList.add('error');
        hint.style.display = 'block';
    }
}

function clearInputError(input) {
    if (!input) return;
    input.classList.remove('error');
    const group = input.closest('.form-group');
    if (group) {
        const hint = group.querySelector('.hint-text');
        if (hint) {
            hint.textContent = '';
            hint.classList.remove('error');
            hint.style.display = 'none';
        }
    }
}

function setButtonLoading(btn, loading = true) {
    if (!btn) return;
    const textSpan = btn.querySelector('span');
    if (loading) {
        btn.disabled = true;
        btn.dataset.originalText = textSpan ? textSpan.textContent : btn.textContent;
        if (textSpan) {
            textSpan.innerHTML = '<span class="spinner" style="border-color: rgba(0,0,0,0.2); border-top-color: #000;"></span>';
        }
    } else {
        btn.disabled = false;
        if (textSpan && btn.dataset.originalText) {
            textSpan.textContent = btn.dataset.originalText;
        }
    }
}

(async function initApp() {
    if (window.location.pathname.startsWith("/auth")) return;

    try {
        const response = await fetch("/api/User/profile");
        if (!response.ok) {
            window.location.replace("/auth");
            return;
        }

        const profile = await response.json();
        if (profile.nick) {
            document.querySelectorAll("#sidebar-user-nick, #user-nick, .user-nick").forEach(el => {
                el.textContent = profile.nick;
                el.classList.remove("skeleton");
            });
        }

        if (typeof window.handleProfileData === "function") {
            window.handleProfileData(profile);
        }
    } catch (err) {
        console.error("Ошибка инициализации профиля:", err);
        window.location.replace("/auth");
    }
})();

document.addEventListener("DOMContentLoaded", () => {
    const logoutBtn = document.querySelector("#logout-btn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", async () => {
            if (!confirm("Вы уверены, что хотите выйти?")) return;
            try {
                const response = await fetch("/api/auth/logout", { method: "POST" });
                if (response.ok) {
                    showToast("Вы успешно вышли из системы", "success", "До встречи!");
                    setTimeout(() => window.location.replace("/"), 600);
                } else {
                    showToast("Не удалось выполнить выход", "error", "Ошибка");
                }
            } catch (error) {
                console.error("Сетевая ошибка:", error);
                showToast("Ошибка соединения с сервером", "error", "Сеть");
            }
        });
    }

    const authForm = document.querySelector("#auth-form");
    const toggleModeBtn = document.querySelector("#toggle-mode-btn");

    if (authForm && toggleModeBtn) {
        const authTitle = document.querySelector("#auth-title");
        const authSubtitle = document.querySelector("#auth-subtitle");
        const submitBtn = document.querySelector("#submit-btn");
        const errorMessageDiv = document.querySelector("#error-message");
        const toggleModeText = document.querySelector("#toggle-mode-text");
        const usernameInput = document.querySelector("#username");
        const passwordInput = document.querySelector("#password");
        const pwdReqs = document.getElementById("password-requirements");

        let isLoginMode = true;

        toggleModeBtn.addEventListener("click", (e) => {
            e.preventDefault();
            isLoginMode = !isLoginMode;
            errorMessageDiv.style.display = "none";
            errorMessageDiv.textContent = "";
            clearInputError(usernameInput);
            clearInputError(passwordInput);

            if (isLoginMode) {
                authTitle.textContent = "Вход в систему";
                authSubtitle.textContent = "Введите данные для доступа к панели управления";
                submitBtn.querySelector('span').textContent = "Войти";
                toggleModeBtn.textContent = "Зарегистрироваться";
                toggleModeText.textContent = "Ещё нет аккаунта? ";
            } else {
                authTitle.textContent = "Регистрация";
                authSubtitle.textContent = "Создайте новый аккаунт для игры";
                submitBtn.querySelector('span').textContent = "Создать аккаунт";
                toggleModeBtn.textContent = "Войти";
                toggleModeText.textContent = "Уже есть аккаунт? ";
            }
        });

        if (passwordInput && pwdReqs) {
            passwordInput.addEventListener('focus', () => {
                if (!isLoginMode) pwdReqs.style.display = 'block';
            });
            passwordInput.addEventListener('blur', () => {
                setTimeout(() => { pwdReqs.style.display = 'none'; }, 200);
            });
            passwordInput.addEventListener('input', (e) => {
                updatePasswordRequirements(e.target.value);
            });
        }

        if (usernameInput) {
            usernameInput.addEventListener('input', () => {
                clearInputError(usernameInput);
            });
            usernameInput.addEventListener('blur', () => {
                const err = validateUsername(usernameInput.value.trim());
                if (err) setInputError(usernameInput, err);
            });
        }

        authForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            errorMessageDiv.style.display = "none";
            errorMessageDiv.textContent = "";
            clearInputError(usernameInput);
            clearInputError(passwordInput);

            const username = usernameInput.value.trim();
            const password = passwordInput.value;

            const usernameError = validateUsername(username);
            if (usernameError) {
                setInputError(usernameInput, usernameError);
                return;
            }

            if (!isLoginMode) {
                const passwordError = validatePassword(password);
                if (passwordError) {
                    setInputError(passwordInput, passwordError);
                    return;
                }
            } else if (!password) {
                setInputError(passwordInput, "Введите пароль");
                return;
            }

            setButtonLoading(submitBtn, true);
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
                    const msg = errorData?.detail || "Произошла ошибка";
                    errorMessageDiv.textContent = msg;
                    errorMessageDiv.style.display = "flex";
                    showToast(msg, "error", isLoginMode ? "Ошибка входа" : "Ошибка регистрации");
                    return;
                }

                showToast(
                    isLoginMode ? "Добро пожаловать!" : "Аккаунт успешно создан!",
                    "success",
                    isLoginMode ? "Вход выполнен" : "Регистрация завершена"
                );
                setTimeout(() => {
                    window.location.href = "/dashboard";
                }, 600);

            } catch (err) {
                console.error("Сетевая ошибка:", err);
                const msg = "Ошибка соединения с сервером";
                errorMessageDiv.textContent = msg;
                errorMessageDiv.style.display = "flex";
                showToast(msg, "error", "Сеть");
            } finally {
                setButtonLoading(submitBtn, false);
            }
        });
    }
});
