import { createApp } from 'vue';
import { createPinia } from 'pinia';
import router from './router';
import App from './App.vue';
import './style.css';
import axios from 'axios';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);

// Axios Interceptor for 401 Unauthorized globally
axios.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response && error.response.status === 401) {
            // Clear local storage and let the store sync on next reload, or call store directly
            const { useAuthStore } = require('./store/auth');
            const authStore = useAuthStore(pinia);
            authStore.logout();

            // Only redirect if not already on login to avoid loops
            if (router.currentRoute.value.path !== '/' && router.currentRoute.value.path !== '/login') {
                router.push('/?expired=true');
            }
        }
        return Promise.reject(error);
    }
);

import { useSettingsStore } from './store/settings';
const settings = useSettingsStore(pinia);
settings.initSettings();

app.mount('#app');
