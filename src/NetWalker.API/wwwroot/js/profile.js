const userIdElement = document.querySelector("#user-id");
const userCreatedTime = document.querySelector("#user-created-time");

const statTotalGames = document.querySelector("#stat-total-games");
const statTotalPlaytime = document.querySelector("#stat-total-playtime");
const statLongestSession = document.querySelector("#stat-longest-session");
const changePasswordForm = document.querySelector("#change-password-form");
const errorMessageEl = document.querySelector("#profile-error-message");
const infoMessageEl = document.querySelector("#profile-info-message");

const profileSkeletonElements = [userIdElement, userCreatedTime, statTotalGames, statTotalPlaytime, statLongestSession];

profileSkeletonElements.forEach(el => {
    if (el) el.classList.add("skeleton");
});

function validatePassword(value) {
    if (!value) return "Пароль обязателен";
    if (value.length < 8) return "Пароль должен быть минимум 8 символов";
    if (!/[A-Z]/.test(value)) return "Пароль должен содержать хотя бы одну заглавную букву";
    if (!/[0-9]/.test(value)) return "Пароль должен содержать хотя бы одну цифру";
    return null;
}

function updatePasswordRequirements(value) {
    const container = document.getElementById("password-requirements");
    if (!container) return;
    const reqLength = container.querySelector('#req-length');
    const reqUpper = container.querySelector('#req-upper');
    const reqDigit = container.querySelector('#req-digit');
    if (reqLength) reqLength.classList.toggle('valid', value.length >= 8);
    if (reqUpper) reqUpper.classList.toggle('valid', /[A-Z]/.test(value));
    if (reqDigit) reqDigit.classList.toggle('valid', /[0-9]/.test(value));
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

document.addEventListener("DOMContentLoaded", () => {
    const newPasswordInput = document.querySelector("#new-password");
    const confirmInput = document.querySelector("#confirm-password");
    const pwdReqs = document.getElementById("password-requirements");
    const confirmHint = document.getElementById("confirm-hint");

    if (newPasswordInput && pwdReqs) {
        newPasswordInput.addEventListener('focus', () => {
            pwdReqs.style.display = 'block';
        });
        newPasswordInput.addEventListener('blur', () => {
            setTimeout(() => { pwdReqs.style.display = 'none'; }, 200);
        });
        newPasswordInput.addEventListener('input', (e) => {
            updatePasswordRequirements(e.target.value);
            if (confirmInput && confirmInput.value) {
                if (e.target.value !== confirmInput.value) {
                    confirmHint.textContent = "Пароли не совпадают";
                    confirmHint.classList.add('error');
                    confirmInput.classList.add('error');
                } else {
                    confirmHint.textContent = "Пароли совпадают";
                    confirmHint.classList.remove('error');
                    confirmHint.classList.add('success');
                    confirmInput.classList.remove('error');
                    confirmInput.classList.add('success');
                }
            }
        });
    }

    if (confirmInput) {
        confirmInput.addEventListener('input', () => {
            if (!newPasswordInput) return;
            if (confirmInput.value !== newPasswordInput.value) {
                confirmHint.textContent = "Пароли не совпадают";
                confirmHint.classList.add('error');
                confirmHint.classList.remove('success');
                confirmInput.classList.add('error');
                confirmInput.classList.remove('success');
            } else if (confirmInput.value) {
                confirmHint.textContent = "Пароли совпадают";
                confirmHint.classList.remove('error');
                confirmHint.classList.add('success');
                confirmInput.classList.remove('error');
                confirmInput.classList.add('success');
            } else {
                confirmHint.textContent = "";
                confirmHint.classList.remove('error', 'success');
                confirmInput.classList.remove('error', 'success');
            }
        });
    }

    if (changePasswordForm) {
        changePasswordForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            errorMessageEl.style.display = "none";
            errorMessageEl.textContent = "";
            infoMessageEl.style.display = "none";
            infoMessageEl.textContent = "";

            const currentPassword = document.querySelector("#current-password").value;
            const newPassword = document.querySelector("#new-password").value;
            const confirmPassword = document.querySelector("#confirm-password").value;
            const submitBtn = document.querySelector("#change-pwd-btn");

            if (!currentPassword) {
                errorMessageEl.textContent = "Введите текущий пароль";
                errorMessageEl.style.display = "flex";
                return;
            }

            const pwdError = validatePassword(newPassword);
            if (pwdError) {
                errorMessageEl.textContent = pwdError;
                errorMessageEl.style.display = "flex";
                return;
            }

            if (newPassword !== confirmPassword) {
                errorMessageEl.textContent = "Новый пароль и подтверждение не совпадают";
                errorMessageEl.style.display = "flex";
                return;
            }

            setButtonLoading(submitBtn, true);

            try {
                const response = await fetch("/api/auth/change-password", {
                    method: "POST",
                    headers: {"Content-Type": "application/json"},
                    body: JSON.stringify({
                        oldPassword: currentPassword,
                        newPassword: newPassword
                    })
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    const msg = errorData?.detail || "Произошла ошибка";
                    errorMessageEl.textContent = msg;
                    errorMessageEl.style.display = "flex";
                    if (typeof showToast === 'function') {
                        showToast(msg, "error", "Ошибка смены пароля");
                    }
                } else {
                    changePasswordForm.reset();
                    infoMessageEl.textContent = "Пароль успешно изменён";
                    infoMessageEl.style.display = "flex";
                    if (confirmInput) {
                        confirmInput.classList.remove('success');
                        confirmHint.textContent = "";
                        confirmHint.classList.remove('success');
                    }
                    if (typeof showToast === 'function') {
                        showToast("Пароль успешно изменён", "success", "Готово");
                    }
                }
            } catch (err) {
                console.error("Сетевая ошибка:", err);
                const msg = "Ошибка соединения с сервером";
                errorMessageEl.textContent = msg;
                errorMessageEl.style.display = "flex";
                if (typeof showToast === 'function') {
                    showToast(msg, "error", "Сеть");
                }
            } finally {
                setButtonLoading(submitBtn, false);
            }
        });
    }
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