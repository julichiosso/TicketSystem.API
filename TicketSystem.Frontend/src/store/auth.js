import { defineStore } from 'pinia';
import axios from 'axios';
export const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5134/api';
let isRefreshing = false;
let failedQueue = [];
const processQueue = (error, token = null) => {
  failedQueue.forEach(prom => {
    if (error) prom.reject(error);
    else prom.resolve(token);
  });
  failedQueue = [];
};
axios.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;
    const status = error.response?.status;
    if (status === 401 && !originalRequest._retry) {
      const refreshToken = localStorage.getItem('refreshToken');
      if (!refreshToken) {
        useAuthStore().forceLogout('Tu sesión ha expirado.');
        return Promise.reject(error);
      }
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then(token => {
          originalRequest.headers['Authorization'] = `Bearer ${token}`;
          return axios(originalRequest);
        }).catch(err => Promise.reject(err));
      }
      originalRequest._retry = true;
      isRefreshing = true;
      try {
        const response = await axios.post(`${API_URL}/auth/refresh`, { refreshToken });
        const { token, refreshToken: newRefreshToken } = response.data;
        localStorage.setItem('token', token);
        localStorage.setItem('refreshToken', newRefreshToken);
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        const store = useAuthStore();
        store.token = token;
        processQueue(null, token);
        originalRequest.headers['Authorization'] = `Bearer ${token}`;
        return axios(originalRequest);
      } catch (err) {
        processQueue(err, null);
        useAuthStore().forceLogout('Tu cuenta fue dada de baja.');
        return Promise.reject(err);
      } finally {
        isRefreshing = false;
      }
    }
    if (status === 500) {
      const store = useAuthStore();
      if (store.token) {
        try {
          await axios.get(`${API_URL}/auth/me`);
        } catch (meError) {
          if (meError.response?.status === 401) {
            store.forceLogout('Tu cuenta fue dada de baja.');
          }
        }
      }
    }
    return Promise.reject(error);
  }
);
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
    isOperador: (state) =>
      state.user?.rol === 'Operador' ||
      state.user?.rol === 1 ||
      state.user?.rol === 'Administrador' ||
      state.user?.rol === 2
  },
  actions: {
    forceLogout(message) {
      this.user = null;
      this.token = null;
      this.error = message || 'Sesión cerrada.';
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
      delete axios.defaults.headers.common['Authorization'];
      window.location.href = '/login?mensaje=' + encodeURIComponent(message || '');
    },
    async login(email, password) {
      this.loading = true;
      this.error = null;
      try {
        const response = await axios.post(`${API_URL}/auth/login`, { email, password });
        this.token = response.data.token;
        this.user = response.data.usuario;
        localStorage.setItem('token', this.token);
        localStorage.setItem('refreshToken', response.data.refreshToken);
        localStorage.setItem('user', JSON.stringify(this.user));
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;
        return true;
      } catch (err) {
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
    async logout() {
      try {
        await axios.post(`${API_URL}/auth/logout`);
      } catch {
      } finally {
        this.user = null;
        this.token = null;
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        delete axios.defaults.headers.common['Authorization'];
      }
    }
  }
});