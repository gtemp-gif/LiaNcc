// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const API_BASE_URL = 'https://localhost:5001/api';

// Utility per ottenere il token JWT se presente
function getAuthToken() {
    return localStorage.getItem('jwt_token');
}

// Utility per configurare gli headers fetch
function getAuthHeaders() {
    const headers = {
        'Content-Type': 'application/json'
    };

    const token = getAuthToken();
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    return headers;
}

// Intercetta form prenotazione
document.addEventListener('DOMContentLoaded', () => {

    // Reservation Form
    const bookingForm = document.querySelector('#booking-form form');
    if (bookingForm) {
        bookingForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const inputs = bookingForm.querySelectorAll('input, select, textarea');
            const data = {
                fullName: inputs[0].value,
                email: inputs[1].value,
                serviceDate: inputs[2].value ? new Date(inputs[2].value).toISOString() : new Date().toISOString(),
                serviceType: inputs[3].value,
                message: inputs[4].value
            };

            try {
                const response = await fetch(`${API_BASE_URL}/Bookings`, {
                    method: 'POST',
                    headers: getAuthHeaders(),
                    body: JSON.stringify(data)
                });

                if (response.ok) {
                    alert('Prenotazione inviata con successo!');
                    bookingForm.reset();
                } else {
                    alert('Errore durante l\'invio della prenotazione.');
                }
            } catch (error) {
                console.error('Error submitting booking:', error);
                alert('Errore di connessione al server.');
            }
        });
    }

    // Contact Form
    const contactForm = document.querySelector('#contatti form');
    if (contactForm) {
        contactForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const inputs = contactForm.querySelectorAll('input, textarea');
            const data = {
                fullName: inputs[0].value,
                email: inputs[1].value,
                message: inputs[2].value
            };

            try {
                const response = await fetch(`${API_BASE_URL}/ContactMessages`, {
                    method: 'POST',
                    headers: getAuthHeaders(),
                    body: JSON.stringify(data)
                });

                if (response.ok) {
                    alert('Messaggio inviato con successo!');
                    contactForm.reset();
                } else {
                    alert('Errore durante l\'invio del messaggio.');
                }
            } catch (error) {
                console.error('Error submitting contact message:', error);
                alert('Errore di connessione al server.');
            }
        });
    }

});

// Helper login fittizio, espandi in base alla vera logica
async function login(email, password) {
    try {
        const response = await fetch(`${API_BASE_URL}/Auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        if (response.ok) {
            const data = await response.json();
            localStorage.setItem('jwt_token', data.token);
            // Salvare altri info se necessario (ruoli, fullName, ecc)
            return true;
        }
        return false;
    } catch (error) {
        console.error('Login error:', error);
        return false;
    }
}
