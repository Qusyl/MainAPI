document.addEventListener("DOMContentLoaded", () => {

    const token = localStorage.getItem("token");
    const authButton = document.getElementById("authButton");
    const paymentText = document.getElementById("paymentText");
    const resultDiv = document.getElementById("result");
    const form = document.getElementById("PaymentForm");
    const submitButton = document.querySelector(".pay-btn");

    if (!form) return;

    if (token && authButton) {
        authButton.innerText = "Профиль";
    }

    let currentPaymentKey = crypto.randomUUID();
    console.log("Current Key:", currentPaymentKey);

    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        submitButton.disabled = true;

        const amount = document.getElementById("amount").value;
        const currency = document.getElementById("currency").value;
        const payment = {
            amount: parseFloat(amount),
            currency: currency,
            provider: "A",
            idempotencyKey: currentPaymentKey
        };

        paymentText.innerHTML = "Отправка платежа...";

        try {
            const response = await fetch(
                "http://localhost:5000/api/payment/send",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": "Bearer " + token
                    },
                    body: JSON.stringify(payment)
                }
            );

            const data = await response.json();
            console.log("RESPONSE:", data);
            console.log("Response status:", response.status);

         
            if (response.status === 403) {
                if (data.decision === "Suspicious") {
                   
                    payment.isHumanVerified = false;
                    await showVerificationModal(payment, submitButton);
                    return;
                } else if (data.decision === "Deny") {
                    showDenied(data.Error || "Мошенническая активность обнаружена");
                    submitButton.disabled = false;
                    return;
                }
            }
            
        
            if (response.ok) {
                renderPaymentInfo(data);

                if (data.status === "Pending") {
                    showPending();
                    pollPaymentStatus(data.paymentId);
                } else if (data.status === "Accepted") {
                    showAccepted();
                    currentPaymentKey = crypto.randomUUID();
                    console.log("NEW KEY:", currentPaymentKey);
                    submitButton.disabled = false;
                } else if (data.status === "Cancelled") {
                    showCancelled();
                    currentPaymentKey = crypto.randomUUID();
                    console.log("NEW KEY:", currentPaymentKey);
                    submitButton.disabled = false;
                } else if (data.status === "Unknown") {
                    showUnknown(data.errorMessage);
                    currentPaymentKey = crypto.randomUUID();
                    submitButton.disabled = false;
                } else {
                    showUnknown("Неизвестный статус платежа: " + data.status);
                    submitButton.disabled = false;
                }
            } else {
               
                resultDiv.innerHTML = `<div style="color:red;">${data.error || data.message || "Ошибка платежа"}</div>`;
                submitButton.disabled = false;
            }

        } catch (error) {
            console.error(error);
            resultDiv.innerHTML = `<div style="color:red;">Ошибка соединения с сервером</div>`;
            submitButton.disabled = false;
        }
    });

    async function pollPaymentStatus(paymentId) {
        let attempts = 0;
        const maxAttempts = 30;
        const interval = setInterval(async () => {
            attempts++;
            try {
                const response = await fetch(
                    `http://localhost:5000/api/payment/status/${paymentId}`,
                    {
                        headers: {
                            "Authorization": "Bearer " + localStorage.getItem("token")
                        }
                    }
                );
                const data = await response.json();
                renderPaymentInfo(data);

                if (data.status === "Accepted") {
                    showAccepted();
                    currentPaymentKey = crypto.randomUUID();
                    console.log("NEW KEY:", currentPaymentKey);
                    clearInterval(interval);
                    const submitButton = document.querySelector(".pay-btn");
                    if (submitButton) submitButton.disabled = false;
                } else if (data.status === "Cancelled") {
                    showCancelled();
                    currentPaymentKey = crypto.randomUUID();
                    console.log("NEW KEY:", currentPaymentKey);
                    clearInterval(interval);
                    const submitButton = document.querySelector(".pay-btn");
                    if (submitButton) submitButton.disabled = false;
                } else if (data.status === "Unknown") {
                    showUnknown(data.errorMessage);
                    clearInterval(interval);
                    const submitButton = document.querySelector(".pay-btn");
                    if (submitButton) submitButton.disabled = false;
                }

                if (attempts >= maxAttempts) {
                    clearInterval(interval);
                    resultDiv.innerHTML += `<div style="color:orange;">Превышено время ожидания платежа</div>`;
                    const submitButton = document.querySelector(".pay-btn");
                    if (submitButton) submitButton.disabled = false;
                }

            } catch (error) {
                clearInterval(interval);
                console.error(error);
                resultDiv.innerHTML += `<div style="color:red;">Ошибка проверки статуса</div>`;
                const submitButton = document.querySelector(".pay-btn");
                if (submitButton) submitButton.disabled = false;
            }
        }, 10000);
    }

    function renderPaymentInfo(data) {
        const statusText = getStatusText(data.status);
        paymentText.innerHTML = `
            <div><strong>PaymentId:</strong> ${data.paymentId}</div>
            <div><strong>Status:</strong> ${statusText}</div>
            <div><strong>Provider:</strong> ${data.provider}</div>
            <div><strong>IdempotencyKey:</strong> ${currentPaymentKey}</div>
        `;
    }

    function getStatusText(status) {
        const statusMap = {
            "Pending": "⏳ В обработке",
            "Accepted": "✅ Принят",
            "Suspicious": "⚠️ Подозрительный",
            "Cancelled": "❌ Отменен",
            "Unknown": "❓ Неизвестно"
        };
        return statusMap[status] || status;
    }

    function showPending() {
        const icon = document.getElementById("paymentStatusIcon");
        if (!icon) return;
        icon.innerHTML = `<div class="status-spinner"></div>`;
    }

    function showAccepted() {
        const icon = document.getElementById("paymentStatusIcon");
        if (!icon) return;
        icon.innerHTML = `<div class="status-success">✅</div>`;
        showNotification("Платеж успешно принят!", "success");
    }

    function showCancelled() {
        const icon = document.getElementById("paymentStatusIcon");
        if (!icon) return;
        icon.innerHTML = `<div class="status-failed">❌</div>`;
        showNotification("Платеж отменен", "error");
    }

    function showDenied(errorMessage) {
        const icon = document.getElementById("paymentStatusIcon");
        if (icon) {
            icon.innerHTML = `<div class="status-failed">🚫</div>`;
        }
        
        resultDiv.innerHTML = `
            <div style="color: #dc3545; padding: 10px; background: #f8d7da; border-radius: 4px; border-left: 4px solid #dc3545;">
                <strong>❌ Платеж отклонен</strong>
                <div style="margin-top: 5px; font-size: 14px;">${errorMessage || "Мошенническая активность обнаружена"}</div>
            </div>
        `;
        
        showNotification(errorMessage || "Платеж отклонен системой антифрод", "error");
    }

    function showUnknown(errorMessage) {
        const icon = document.getElementById("paymentStatusIcon");
        const paymentText = document.getElementById("paymentText");
        
        if (icon) {
            icon.innerHTML = `<div class="status-unknown">❓</div>`;
        }
        
        const message = errorMessage || "Платеж не может быть обработан. Пожалуйста, попробуйте позже или свяжитесь с поддержкой.";
        
        if (paymentText) {
            paymentText.innerHTML += `
                <div style="margin-top: 15px; padding: 10px; background: #fff3cd; border-left: 4px solid #ffc107; border-radius: 4px;">
                    <div style="color: #856404; font-weight: bold;">⚠️ Внимание!</div>
                    <div style="color: #856404; margin-top: 5px;">${message}</div>
                    <button class="retry-btn" style="margin-top: 10px; padding: 5px 15px; background: #ffc107; border: none; border-radius: 4px; cursor: pointer;">
                        Попробовать снова
                    </button>
                </div>
            `;
        }
        
      
        const retryBtn = document.querySelector(".retry-btn");
        if (retryBtn) {
            retryBtn.onclick = () => {
                if (paymentText) {
                    paymentText.innerHTML = "Повторная отправка...";
                }
                form.dispatchEvent(new Event("submit"));
            };
        }
        
        showNotification(message, "warning");
    }

    async function showVerificationModal(payment, submitButton) {
        const modal = document.getElementById("verificationModal");
        
        if (!modal) {
            console.error("Modal not found!");
            submitButton.disabled = false;
            showNotification("Ошибка: модальное окно не найдено", "error");
            return;
        }

        console.log("Showing modal...");
        modal.classList.remove("hidden");
        
       
        console.log("Modal classes:", modal.className);
        console.log("Modal display:", window.getComputedStyle(modal).display);

    
        const verifyButton = document.getElementById("verifyButton");
        const newVerifyButton = verifyButton.cloneNode(true);
        verifyButton.parentNode.replaceChild(newVerifyButton, verifyButton);
        
    
        const humanCheck = document.getElementById("humanCheck");
        if (humanCheck) humanCheck.checked = false;

        return new Promise((resolve) => {
            newVerifyButton.onclick = async () => {
                const checked = document.getElementById("humanCheck").checked;
                
                if (!checked) {
                    showNotification("Пожалуйста, подтвердите, что вы не бот", "warning");
                    return;
                }

                console.log("Verification confirmed, closing modal...");
                modal.classList.add("hidden");
                payment.isHumanVerified = true;
                
                await resendPayment(payment, submitButton);
                resolve();
            };
        });
    }

    async function resendPayment(payment, submitButton) {
        try {
            
            const updatedPayment = {
                ...payment,
                isHumanVerified: true
            };
            
            const response = await fetch(
                "http://localhost:5000/api/payment/send",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": "Bearer " + localStorage.getItem("token")
                    },
                    body: JSON.stringify(updatedPayment)
                }
            );

            const data = await response.json();
            console.log("Resend response:", data);
            console.log("Resend status:", response.status);
            
         
            if (response.status === 403) {
                if (data.decision === "Suspicious") {
                    
                    await showVerificationModal(payment, submitButton);
                    return;
                } else if (data.decision === "Deny") {
                    showDenied(data.Error || "Мошенническая активность обнаружена");
                    submitButton.disabled = false;
                    return;
                }
            }
            
            if (response.ok) {
                renderPaymentInfo(data);
                
                if (data.status === "Accepted") {
                    showAccepted();
                    currentPaymentKey = crypto.randomUUID();
                    console.log("NEW KEY:", currentPaymentKey);
                    submitButton.disabled = false;
                } else if (data.status === "Suspicious") {
                    await showVerificationModal(payment, submitButton);
                } else if (data.status === "Unknown") {
                    showUnknown(data.errorMessage);
                    submitButton.disabled = false;
                } else if (data.status === "Cancelled") {
                    showCancelled();
                    submitButton.disabled = false;
                } else {
                    submitButton.disabled = false;
                }
            } else {
                resultDiv.innerHTML = `<div style="color:red;">${data.error || data.message || "Ошибка при повторной отправке"}</div>`;
                submitButton.disabled = false;
            }
        } catch (error) {
            console.error("Resend error:", error);
            resultDiv.innerHTML = `<div style="color:red;">Ошибка соединения при повторной отправке</div>`;
            submitButton.disabled = false;
            showNotification("Ошибка соединения с сервером", "error");
        }
    }

    function showNotification(message, type = "info") {
       
        const oldNotifications = document.querySelectorAll(".notification");
        oldNotifications.forEach(notif => notif.remove());
        
      
        const notification = document.createElement("div");
        notification.className = `notification notification-${type}`;
        
        const icon = type === "success" ? "✅" : type === "error" ? "❌" : type === "warning" ? "⚠️" : "ℹ️";
        
        notification.innerHTML = `
            <div class="notification-content">
                <span class="notification-icon">${icon}</span>
                <span class="notification-message">${message}</span>
            </div>
            <button class="notification-close">&times;</button>
        `;
        
  
        document.body.appendChild(notification);
        
    
        setTimeout(() => notification.classList.add("show"), 10);
        
      
        const closeBtn = notification.querySelector(".notification-close");
        if (closeBtn) {
            closeBtn.onclick = () => {
                notification.classList.remove("show");
                setTimeout(() => notification.remove(), 300);
            };
        }
        
   
        setTimeout(() => {
            if (notification.parentNode) {
                notification.classList.remove("show");
                setTimeout(() => notification.remove(), 300);
            }
        }, 5000);
    }
});