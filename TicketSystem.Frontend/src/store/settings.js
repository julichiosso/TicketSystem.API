import { defineStore } from 'pinia';

export const useSettingsStore = defineStore('settings', {
    state: () => ({
        themeColor: localStorage.getItem('themeColor') || 'light',
        uiDensity: localStorage.getItem('uiDensity') || 'comfortable',
        language: localStorage.getItem('language') || 'es',
        notificationsEnabled: localStorage.getItem('notificationsEnabled') !== 'false'
    }),
    actions: {
        setThemeColor(color) {
            this.themeColor = color;
            localStorage.setItem('themeColor', color);
            this.applyTheme();
        },
        setUIDensity(density) {
            this.uiDensity = density;
            localStorage.setItem('uiDensity', density);
            this.applyDensity();
        },
        setLanguage(lang) {
            this.language = lang;
            localStorage.setItem('language', lang);
        },
        toggleNotifications() {
            this.notificationsEnabled = !this.notificationsEnabled;
            localStorage.setItem('notificationsEnabled', this.notificationsEnabled);
        },
        applyTheme() {
            const isDark = this.themeColor === 'dark';
            document.documentElement.classList.toggle('dark-theme', isDark);

            // Also toggle Tailwind's dark mode class if they use 'class' strategy
            document.documentElement.classList.toggle('dark', isDark);
        },
        applyDensity() {
            document.documentElement.setAttribute('data-density', this.uiDensity);
        },
        initSettings() {
            this.applyTheme();
            this.applyDensity();
        }
    },
    getters: {
    }
});
