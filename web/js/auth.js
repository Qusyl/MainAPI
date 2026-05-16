const loginTab = document.getElementById("loginTab");
    const registerTab = document.getElementById("registerTab");

    const loginForm = document.getElementById("loginForm");
    const registerForm = document.getElementById("registerForm");

    const result = document.getElementById("result");
    loginTab.addEventListener("click", () => {

        loginTab.classList.add("active");
        registerTab.classList.remove("active");

        loginForm.classList.remove("hidden");
        registerForm.classList.add("hidden");
    });

    registerTab.addEventListener("click", () => {

        registerTab.classList.add("active");
        loginTab.classList.remove("active");

        registerForm.classList.remove("hidden");
        loginForm.classList.add("hidden");
    });
    loginForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const email =
        document.getElementById("loginEmail").value;
    const password =
        document.getElementById("loginPassword").value;
    const loginRequest = {
        email,
        password
    };

    try {
        result.innerHTML =
            "Авторизация...";
        const response = await fetch(
            "http://localhost:5000/api/auth/login",
            {
                method: "POST",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify(loginRequest)
            }
        );
        const data = await response.json();
	console.log("RESPONSE:", data);
        if(response.ok){
            localStorage.setItem("token", data.token);
            result.innerHTML = `
                <span style="color:green;">
                    Авторизация успешна
                </span>
            `;
            console.log("TOKEN:", data.token);
        }
        else{

            result.innerHTML = `
                <span style="color:red;">
                    Ошибка: ${data.error}
                </span>
            `;
        }

    } catch(error){
        console.error(error);
        result.innerHTML = `
            <span style="color:red;">
                Backend недоступен
            </span>
        `;
    }
});
    registerForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const email =
        document.getElementById("registerEmail").value;
    const password =
        document.getElementById("registerPassword").value;

    const confirmPassword =
        document.getElementById("registerConfirmPassword").value;

    if(password !== confirmPassword){

        result.innerHTML =
            "<span style='color:red;'>Пароли не совпадают</span>";

        return;
    }
    const registerRequest = {
        email,
        password
    };
    try {
        result.innerHTML =
            "Регистрация...";
        const response = await fetch(
            "http://localhost:5000/api/auth/register",
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(registerRequest)
            }
        );
        const data = await response.json();
        if(response.ok){
            localStorage.setItem("token", data.token);
            result.innerHTML = `
                <span style="color:green;">
                    Регистрация успешна
                </span>
            `;

            console.log("TOKEN:", data.token);
        }
        else{
            result.innerHTML = `
                <span style="color:red;">
                    Ошибка: ${data.error}
                </span>
            `;
        }
    } catch(error){
        console.error(error);
        result.innerHTML = `
            <span style="color:red;">
                Backend недоступен
            </span>
        `;
    }
});
document.querySelector('.return-btn').addEventListener('click', function() {
    window.location.href = '../html/index.html';
});