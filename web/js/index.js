document.addEventListener("DOMContentLoaded", () => {

    const token = localStorage.getItem("token");

    const authButton =
        document.getElementById("authButton");

    if (token && authButton) {

        authButton.innerText = "Профиль";
        authButton.href = "#";
    }

    const form =
        document.getElementById("PaymentForm");

    const resultDiv =
        document.getElementById("result");

    const submitButton =
        document.querySelector(".pay-btn");

    if (!form) return;

    // =========================
    // ONE PAYMENT KEY
    // =========================

    let currentPaymentKey =
        crypto.randomUUID();

    console.log(
        "Current Key:",
        currentPaymentKey
    );

    // =========================
    // SUBMIT
    // =========================

    form.addEventListener("submit", async (e) => {

        e.preventDefault();

        submitButton.disabled = true;

        const amount =
            document.getElementById("amount").value;

        const currency =
            document.getElementById("currency").value;

        const payment = {

            amount: parseFloat(amount),

            currency: currency,

            provider: "A",

            // ВАЖНО:
            // один key для повторных запросов

            idempotencyKey:
                currentPaymentKey
        };

        resultDiv.innerHTML =
            "Отправка платежа...";

        try {

            const response = await fetch(
                "http://localhost:5000/api/payment/send",
                {
                    method: "POST",

                    headers: {
                        "Content-Type":
                            "application/json",

                        "Authorization":
                            "Bearer " + token
                    },

                    body: JSON.stringify(payment)
                }
            );

            const data =
                await response.json();

            console.log("RESPONSE:", data);

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
                        <strong>Provider:</strong>
                        ${data.provider}
                    </div>

                    <div>
                        <strong>IdempotencyKey:</strong>
                        ${currentPaymentKey}
                    </div>
                `;

                // =========================
                // CREATE NEW KEY
                // =========================

                // Только после финального статуса

                if (
                    data.status === "Accepted" ||
                    data.status === "Rejected"
                ) {

                    currentPaymentKey =
                        crypto.randomUUID();

                    console.log(
                        "NEW KEY:",
                        currentPaymentKey
                    );
                }

                // Если Pending:
                // key НЕ меняем
            }

            else {

                resultDiv.innerHTML = `
                    <div style="color:red;">
                        ❌ Ошибка
                    </div>
                `;
            }

        } catch (error) {

            console.error(error);

            resultDiv.innerHTML = `
                <div style="color:red;">
                    ❌ Backend недоступен
                </div>
            `;
        }

        finally {

            submitButton.disabled = false;
        }
    });
});

