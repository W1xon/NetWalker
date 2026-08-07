const loginForm = document.querySelector("#login-form");
loginForm.addEventListener("submit", Login);


async function  Login(e){
    e.preventDefault();
    const submitBtn = loginForm.querySelector('button[type = "submit"]');
    submitBtn.disabled = true;
    
    try {
        const formData = new FormData(loginForm);
        const data = Object.fromEntries(formData.entries());
        const response = await fetch("/api/auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        });
        const errorMessageDiv = document.querySelector("#error-message");
        if (!response.ok) {
            const errorData = await response.json().catch(() => null);
            errorMessageDiv.textContent = errorData?.detail || "Произошла ошибка при входе";
            return;
        }
        console.log("Успешно");
        
    }
    catch(err){
        console.log("Сетевая ошибка:", err);
    }
    finally {
        submitBtn.disabled = false;
    }
}