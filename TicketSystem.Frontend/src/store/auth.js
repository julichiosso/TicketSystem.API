import { defineStore } from 'pinia';
import axios from 'axios';

export const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5134/api';

export const useAuthStore = defineStore('auth', {
    state: () => {
        const token = localStorage.getItem('token');
        if (token) {
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        }
        return {
            user: JSON.parse(localStorage.getItem('user')) || null,
            token: token || null,
            loading: false,
            error: null
        };
    },
    getters: {
        isAuthenticated: (state) => !!state.token,
        isAdmin: (state) => state.user?.rol === 'Administrador' || state.user?.rol === 2,
        isOperador: (state) => state.user?.rol === 'Operador' || state.user?.rol === 1 || state.user?.rol === 'Administrador' || state.user?.rol === 2
    },
    actions: {
        async login(email, password) {
            this.loading = true;
            this.error = null;
            try {
                const response = await axios.post(`${API_URL}/auth/login`, { email, password });
                this.token = response.data.token;
                this.user = response.data.usuario;
                localStorage.setItem('token', this.token);
                localStorage.setItem('user', JSON.stringify(this.user));
                axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;
                return true;
            } catch (err) {
                console.error('Login error:', err.response?.data || err.message);
                this.error = err.response?.data?.message ||
                    err.response?.data?.details ||
                    err.response?.statusText ||
                    err.message ||
                    'Error de autenticación';
                return false;
            } finally {
                this.loading = false;
            }
        },
        async register(userData) {
            this.loading = true;
            this.error = null;
            try {
                await axios.post(`${API_URL}/auth/register`, userData);
                return true;
            } catch (err) {
                console.error('Register error:', err.response?.data || err);
                this.error = err.response?.data?.message || err.message || 'Error en el registro';
                return false;
            } finally {
                this.loading = false;
            }
        },
        async forgotPassword(email) {
            this.loading = true;
            this.error = null;
            try {
                const response = await axios.post(`${API_URL}/auth/forgot-password`, { email });
                return { success: true, message: response.data.message };
            } catch (err) {
                this.error = err.response?.data?.message || 'Error al solicitar recuperación';
                return { success: false, message: this.error };
            } finally {
                this.loading = false;
            }
        },
        async resetPassword(data) {
            this.loading = true;
            this.error = null;
            try {
                const response = await axios.post(`${API_URL}/auth/reset-password`, data);
                return { success: true, message: response.data.message };
            } catch (err) {
                this.error = err.response?.data?.message || 'Error al restablecer contraseña';
                return { success: false, message: this.error };
            } finally {
                this.loading = false;
            }
        },
        logout() {
            this.user = null;
            this.token = null;
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            delete axios.defaults.headers.common['Authorization'];
        }
    }
});

