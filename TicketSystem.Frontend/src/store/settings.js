import { defineStore } from 'pinia';

export const useSettingsStore = defineStore('settings', {
  state: () => ({
    themeMode: localStorage.getItem('themeMode') || 'light', // 'light' | 'dark'
    themeColor: localStorage.getItem('themeColor') || 'blue',
    uiDensity: localStorage.getItem('uiDensity') || 'comfortable',
    language: localStorage.getItem('language') || 'es',
    notificationsEnabled: localStorage.getItem('notificationsEnabled') !== 'false'
  }),

  getters: {
    isDark: (state) => state.themeMode === 'dark',
    themeClasses: (state) => {
      const colors = {
        blue: 'from-blue-500 to-blue-700',
        indigo: 'from-indigo-500 to-indigo-700',
        emerald: 'from-emerald-500 to-emerald-700',
        rose: 'from-rose-500 to-rose-700',
        violet: 'from-violet-500 to-violet-700',
      };
      return colors[state.themeColor] || colors.blue;
    },
    accentColor: (state) => {
      const colors = {
        blue: '#3b82f6',
        indigo: '#6366f1',
        emerald: '#10b981',
        rose: '#f43f5e',
        violet: '#8b5cf6',
      };
      return colors[state.themeColor] || colors.blue;
    }
  },

  actions: {
    setThemeMode(mode) {
      this.themeMode = mode;
      localStorage.setItem('themeMode', mode);
      this.applyTheme();
    },

    toggleThemeMode() {
      this.setThemeMode(this.themeMode === 'light' ? 'dark' : 'light');
    },

    setThemeColor(color) {
      this.themeColor = color;
      localStorage.setItem('themeColor', color);
      this.applyAccentColor();
    },

    setUIDensity(density) {
      this.uiDensity = density;
      localStorage.setItem('uiDensity', density);
      document.documentElement.setAttribute('data-density', density);
    },

    setLanguage(lang) {
      this.language = lang;
      localStorage.setItem('language', lang);
    },

    toggleNotifications() {
      this.notificationsEnabled = !this.notificationsEnabled;
      localStorage.setItem('notificationsEnabled', this.notificationsEnabled);
    },

    applyAccentColor() {
      const colors = {
        blue: '59 130 246',
        indigo: '99 102 241',
        emerald: '16 185 129',
        rose: '244 63 94',
        violet: '139 92 246',
      };
      const val = colors[this.themeColor] || colors.blue;
      document.documentElement.style.setProperty('--accent-primary', val);
    },

    applyTheme() {
      const isDark = this.themeMode === 'dark';
      document.documentElement.classList.toggle('dark', isDark);
      document.documentElement.setAttribute('data-theme', this.themeMode);
    },

    initSettings() {
      this.applyTheme();
      this.applyAccentColor();
      document.documentElement.setAttribute('data-density', this.uiDensity);
    }
  }
});