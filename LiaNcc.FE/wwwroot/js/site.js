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

// Inizializzazione globale per tutti i calendari VIP del sito
document.addEventListener('DOMContentLoaded', function() {
    // Cerca tutti gli input con la classe "luxury-datepicker" e li attiva
    flatpickr(".luxury-datepicker", {
        locale: "it",           
        dateFormat: "d/m/Y",    
        minDate: "today",       
        disableMobile: "true"   
    });
});