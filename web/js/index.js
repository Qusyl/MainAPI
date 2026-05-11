document
    .getElementById("PaymentForm")
    .addEventListener("submit", async (e) => {
        e.preventDefault();
        const amount = document.getElementById("amount").value;
        const currency = document.getElementById("currency").value;
        const resultDiv = document.getElementById("result");
        const payment = {
            amount: parseFloat(amount),
            currency: currency,
            provider: "A",
            idempotencyKey: crypto.randomUUID()
        };

        try {
            resultDiv.innerHTML = "Отправка платежа...";
            const response = await fetch(
                "http://localhost:5000/api/payment/send",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(payment)
                }
            );
            const data = await response.json();
            if (response.ok) {

                resultDiv.innerHTML = `
                    <div>
                        <strong>PaymentId:</strong>
                        ${data.paymentId}
                    </div>
                    <div>
                        <strong>Status:</strong>
                        ${data.status}
                    </div>
                    <div>
                        <strong>Time:</strong>
                        ${data.occuredOn}
                    </div>
                `;
            }
            else {
                resultDiv.innerHTML = `
                    <div style="color:red;">
                        Ошибка: ${data.message}
                    </div>
                `;
            }
        } catch (error) {
            console.error(error);
            resultDiv.innerHTML = `
                <div style="color:red;">
                    Backend недоступен
                </div>
            `;
        }
    });
const token = localStorage.getItem("token");

const authButton = document.getElementById("authButton");

if(token){

    authButton.innerText = "Профиль";

    authButton.href = "#";
}